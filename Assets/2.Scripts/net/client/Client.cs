using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Threading;
public class Client : MonoBehaviour
{
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream ns;
    private bool isConnected;

    private static byte userCode = Packet.Target.ACCESS_REQUESTER;
    public static byte UserCode { get { return userCode; } set { userCode = value; } }

    public float sendDelay = 0.01f;
    private int sendDelayM;
    private Thread sendThread;

    public bool IsConnected { get { return isConnected; } }


    private void Awake()
    {
        sendDelayM = (int)sendDelay * 1000;
    }

    private void Start()
    {
        
    }

    public void ConnectToServer(string ip)
    {
        if (isConnected)
            return;

        socket = new TcpClient(ip, 5252);

        socket.ReceiveBufferSize = 500;
        socket.SendBufferSize = 500;

        ns = socket.GetStream();

        isConnected = true;

        if (sendThread != null)
        {
            sendThread.Abort();
        }
        sendThread = new Thread(() => Delivery());
        sendThread.Start();
    }

    void Update()
    {

        if (isConnected)
        {
            if (ns.DataAvailable)
            {
               
                Packet packet;
                PacketParser.Pasing(ns, out packet);
                if (packet != null)
                {
                    PacketManager.instance.GetPacket(packet);
                }
            }
        }

        if(isConnected && socket != null && !socket.Connected)
        {
            CloseSocket();
            GameManager.instance.NetExit();
        }
    }

    private void Delivery()
    {
        while (isConnected)
        {
            try
            {
                Packet[] ps = PacketManager.instance.SendPacket;
                foreach (Packet p in ps)
                {
                    Send(p, false);
                }
                ns.Flush();

                Thread.Sleep(sendDelayM);
            }catch(System.Exception e)
            {

            }
        }
    }

    private void Send(Packet packet, bool Immediately)
    {
        byte[] writerData = packet.Data;
        ns.Write(writerData, 0, writerData.Length);

        if (Immediately)
            ns.Flush();
    }

    public void CloseSocket()
    {
        if (!isConnected)
            return;

        sendThread.Abort();

        isConnected = false;
        StopAllCoroutines();

        if(ns != null)
        ns.Close();
        if(socket != null)
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }

}
