using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Net;
using System.Text;
using System.IO;
using UnityEngine.UI;

public class Server : MonoBehaviour, IEventListener
{

    private Dictionary<byte ,ServerClient> clients;
    private TcpListener server;
    private bool serverStarted;
    private List<Thread> packetReceiveThread;

    public float sendDelay = 0.001f;
    private int sendDelayM;
    private WaitForSeconds ws;

    private Thread sendThread;
    public Dictionary<byte, ServerClient> Clients { get { return clients; } }

    private void Awake()
    {
        clients = new Dictionary<byte, ServerClient>();
        packetReceiveThread = new List<Thread>();

        sendDelayM = (int)sendDelay * 1000;

    }

    private void Start()
    {
        AddListener();
    }

    public bool ServerStart(int port)
    {
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            StartListening();
            serverStarted = true;
            Debug.Log("Server has been started on port " + port.ToString());


            sendThread = new Thread(() => Delivery());
            sendThread.Start();

            return true;
        }
        catch(Exception e)
        {
            Debug.Log("Socket error: " + e.Message);


            return false;
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);

    }


    private void AcceptTcpClient(IAsyncResult ar) //Rock 걸어주기
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        TcpClient empclient = listener.EndAcceptTcpClient(ar);

        Debug.Log("ENter New CLient ");

        if (empclient == null) Debug.Log("Cleint empty"); 

        EventManager.CallEvent(new ReceiveNewClientEvent(empclient));
        

        StartListening();
    }

    private void RegisterClient(ClientJoinEvent e)
    {
        ServerClient client = new ServerClient(e.Client, e.User.ID);
        BroadCast(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.REGISTER_CLIENT, e.User));


        Send(client, new Packet(Packet.Target.SERVER, Packet.Target.ACCESS_REQUESTER, Packet.Type.OK, new byte[2] { 2, e.User.ID }), true);


        client.Tcp.ReceiveBufferSize = 500;
        client.Tcp.SendBufferSize = 500;

        clients.Add(client.ID, client);


        Thread thread = new Thread(() => ClientInComingPacket(client));
        thread.Start();

        packetReceiveThread.Add(thread);

    }

    /*
    private IEnumerator WaitBroadCast(ref bool canSend, Packet pakcet)
    {
        while (canSend) yield return null;
        BroadCast(pakcet);
    }
    */

    private void ClientInComingPacket(ServerClient client)
    {
   

        while (IsConnected(client.Tcp))
        {
            if (client.IsDataAvailable())
            {
                Stream e = client.getStream();
                Packet packet;
                //비동기? asyncRead
                PacketParser.Pasing(e, out packet);
                if (packet != null)
                {
                    if (packet.Receiver != Packet.Target.SERVER)
                        PacketManager.instance.PutPacket(packet);
                    else
                    {
                        if(packet.Receiver == Packet.Target.ALL)
                            PacketManager.instance.PutPacket(packet);
                        PacketManager.instance.GetPacket(packet);
                    }
                }
            }

        }

        EventManager.CallEvent(new ClientQuitEvent(client, UserManager.instance.GetUser(client)));

    }
    
    private void ClientJoinListening(ReceiveNewClientEvent e)
    {
        TcpClient client = e.Client;
        GameObject receiveObj = new GameObject("NewClientReceiver");
        NewClientReceiver clientReceiver = receiveObj.AddComponent<NewClientReceiver>();
        clientReceiver.Accept(client, 10.0f);
        receiveObj.transform.SetParent(this.gameObject.transform);
    }


    public bool IsConnected(TcpClient c)
    {
        try
        {
            if(c != null && c.Client != null && c.Client.Connected)
            {
                if(c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {

            return false;
        }
    }
    
    private void Delivery()
    {
        while (serverStarted) {
            try
            {
                Packet[] packets = PacketManager.instance.SendPacket;

                foreach (Packet p in packets)
                {
                    if (p.Receiver == Packet.Target.ALL)
                        BroadCast(p);
                    else
                    {
                        if (clients.ContainsKey(p.Receiver))
                            Send(clients[p.Receiver], p, false);
                    }
                }

                foreach (ServerClient c in clients.Values)
                {
                    c.getStream().Flush();
                }

                Thread.Sleep(sendDelayM);

            }catch(Exception e){
                Console.WriteLine(e);
            }
        }
    }

    public void Send(ServerClient client, Packet packet, bool Immediately)
    {
        Send(client.Tcp, packet, Immediately);
    }

    public void Send(TcpClient client, Packet packet, bool Immediately)
    {
        NetworkStream stream = client.GetStream();
        byte[] writerData = packet.Data;
        stream.Write(writerData, 0, writerData.Length);
        if (Immediately)
            stream.Flush();
    }

    public void BroadCast(Packet packet)
    {
        foreach(ServerClient client in clients.Values)
        {
            Send(client, packet, false);
        }
    }

    public void Kick(TcpClient c)
    {
        if (c == null) return;
        Send(c, new Packet(Packet.Target.SERVER, Packet.Target.ACCESS_REQUESTER, Packet.Type.KICK), true);
        c.GetStream().Close();
        c.Close();
    }

    public void Kick(ServerClient c)
    {
        if (c == null) return;
        if (clients.ContainsKey(c.ID))
        {
            Send(c, new Packet(Packet.Target.SERVER, c.ID, Packet.Type.KICK), true);
            EventManager.CallEvent(new ClientQuitEvent(c, UserManager.instance.GetUser(c)));

        }
   }

    private void ClientExit(ClientQuitEvent e)
    {

        if (e.User != null)
        {
            clients.Remove(e.Client.ID);
            //UserManager.instance.ExitUser(user);
            e.Client.Close();

            BroadCast(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.EXIT_CLIENT, e.User));
        }
    }



    public void ServerClose()
    {
        serverStarted = false;
        StopAllCoroutines();
        sendThread.Abort();

        foreach(ServerClient client in clients.Values)
        {
            Kick(client);
        }
        server.Stop();
    }

    private void OnApplicationQuit()
    {
        ServerClose();

    }

    private void OnDestroy()
    {
        ServerClose();
        RemoveListener();
    }

    private void OnDisable()
    {
        ServerClose();
        RemoveListener();
    }

    public void AddListener()
    {
        EventManager.AddListener<ReceiveNewClientEvent>(ClientJoinListening);
        EventManager.AddListener<ClientJoinEvent>(RegisterClient);
        EventManager.AddListener<ClientQuitEvent>(ClientExit);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<ReceiveNewClientEvent>(ClientJoinListening);
        EventManager.RemoveListener<ClientJoinEvent>(RegisterClient);
        EventManager.RemoveListener<ClientQuitEvent>(ClientExit);
    }
}

public class ServerClient
{
    private TcpClient tcp;
    private string clientIP;
    private NetworkStream ns;
    private byte id;
    
    public byte ID { get { return id; } }
    public TcpClient Tcp { get { return tcp; } }


    public ServerClient(TcpClient client, byte id)
    {
        this.tcp = client;
        this.id = id;
        this.ns = tcp.GetStream();
        
    }

    public Stream getStream() {
        return ns;
    }

    public bool IsDataAvailable()
    {
        return ns.DataAvailable;
    }

    public void Close()
    {
        ns.Close();
        tcp.Close();
    }

}
