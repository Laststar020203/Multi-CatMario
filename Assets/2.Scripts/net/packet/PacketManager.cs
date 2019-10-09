using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


public class PacketManager : MonoBehaviour
{

    private Queue<Packet> receivePacket;
    private Queue<Packet> sendPackets;

    private List<IPacketDataReceiver> packetReceiver;
    public static PacketManager instance;

    private Dictionary<byte, IRequester> requesterBuffer;


    public Queue<Packet> ReceivePacket { get { return receivePacket; } }
    public Packet[] SendPacket { get { return GetSendPacketes(); } }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
        DontDestroyOnLoad(instance);

        packetReceiver = new List<IPacketDataReceiver>();
        receivePacket = new Queue<Packet>();
        sendPackets = new Queue<Packet>();

        requesterBuffer = new Dictionary<byte, IRequester>();

    }

    private Packet[] GetSendPacketes()
    {
        Packet[] ps;

        ps = sendPackets.ToArray();
        sendPackets.Clear();

        return ps;
    }
    
    public void PutPacket(Packet packet)
    {
        this.sendPackets.Enqueue(packet);
    }

    public void PutPacket(Packet packet, IRequester requester)
    {
        PutPacket(packet);

        if (requesterBuffer.ContainsKey(requester.RequestNumber))
        {
            requesterBuffer.Remove(requester.RequestNumber);
        }

        requesterBuffer.Add(requester.RequestNumber, requester);
    }

    public void GetPacket(Packet packet)
    {
        this.receivePacket.Enqueue(packet);
    }

    public void AddPacketDataReceiver(IPacketDataReceiver receiver)
    {
        packetReceiver.Add(receiver);
    }

    public void RemovePacketDataReceiver(IPacketDataReceiver reciever)
    {
        packetReceiver.Remove(reciever);
    }
    

    // Update is called once per frame
    void Update()
    {

        if(receivePacket.Count != 0)
        {
            Packet packet = receivePacket.Dequeue();

            if(packet.TypeCode == Packet.Type.OK || packet.TypeCode == Packet.Type.FAIL)
            {

                byte code = packet.Body[0];
                IRequester rq = requesterBuffer[code];
                if(rq != null && rq.IsRequesting)
                {
                    IPacketDataReceiver pdr = (IPacketDataReceiver)rq;
                    pdr.Receive(packet);
                }

                requesterBuffer.Remove(code);
            }
            else
            {

                foreach (IPacketDataReceiver item in packetReceiver)
                {
                    if (item.CheckResponsible(packet))
                    {
                        item.Receive(packet);
                    }
                }
            }
        }

    }
}
