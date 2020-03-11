using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System;

//게임 스타트 중일때 막음
//이 클래스는 여러개여야 한다?

public class NewClientReceiver : MonoBehaviour
{
    private Room room;
    private Server server;
    private TcpClient client;

    private Thread thread;


    private User user;


    private void Awake()
    {
        room = RoomManager.instance.RoomInfo;
        server = GameObject.Find("net").GetComponentInChildren<Server>();
    }

    void Start()
    {

    }

    public void Accept(TcpClient client, float waitTime)
    {

        server.Send(client, new Packet(Packet.Target.SERVER, Packet.Target.ACCESS_REQUESTER, Packet.Type.HELLO, RoomManager.instance.RoomInfo), true);

        foreach (User u in RoomManager.instance.RoomInfo.Personnel.Values)
        {

        }


        if (room.HeadCount >= room.MaxUser) return;
        //.. 게임이 스타트 중일때 막는다.
        this.client = client;
        Destroy(this.gameObject, waitTime);

        

        thread = new Thread(() => ReceivePacket());
        thread.Start();
        
    }
    Queue<Packet> receiveQueue = new Queue<Packet>();

    private void ReceivePacket()
    {
        try
        {

            NetworkStream stream = client.GetStream();


            Packet packet;
            PacketParser.Pasing(stream, out packet);

            if (packet == null) return;

            if (!receiveQueue.Contains(packet))
                receiveQueue.Enqueue(packet);

          
        }catch(Exception e)
        {
            client.Close();
        }
    }

    private void PacketReview(Packet p)
    {
        Packet packet = p;
        if (packet.TypeCode == Packet.Type.REQUEST_ACCESS)
        {
            if (user != null)
            {
                server.Send(client, new Packet(Packet.Target.SERVER, Packet.Target.ACCESS_REQUESTER, Packet.Type.FAIL), true);
                server.Kick(client);

                return;
            }
            try
            {
                if (packet.Body.Length < 2) { 

                    Destroy(this.gameObject);
                    return;
                }
                byte id = getNewClientCode();
                user = new User(packet.Body, id);

                if (client == null) return;

                EventManager.CallEvent(new ClientJoinEvent(client, user));

                /*


                UserManager.instance.RegisterUser(user);

                */
            }
            catch (Exception e)
            {
                GameManager.instance.ShowMessage(UnityEngine.Random.Range(0, 2) % 2 == 0 ? "패킷이 잘못전달되었습니다.. 개발자 일안하냐ㅏ!!"
                : "패킷이 잘못전달되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);

                server.Send(client, new Packet(Packet.Target.SERVER, Packet.Target.ACCESS_REQUESTER, Packet.Type.FAIL), true);
                server.Kick(client);
            }

            Destroy(this.gameObject);
        }
    } 

    void Update()
    {
             
        if(client != null)
        {
            if (!server.IsConnected(client))
                Destroy(this.gameObject);
        }

        if(receiveQueue.Count != 0)
        {
            PacketReview(receiveQueue.Dequeue());
        }
   
    }

    private byte getNewClientCode()
    {
        while (true)
        {
            byte code = (byte) UnityEngine.Random.Range(3, 254);
            bool collect = true;
            foreach (ServerClient client in server.Clients.Values)
            {
                if (client.ID == code)
                    collect = false;
            }
            if (collect) return code;
        }

    }
    private void OnDestroy()
    {

        if(thread != null)
        thread.Abort();



    }

    private void OnDisable()
    {
        if (thread != null)
            thread.Abort();

    }

}
