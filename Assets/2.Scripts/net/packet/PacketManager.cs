using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;


public class PacketManager : MonoBehaviour
{

    public Queue<Packet> receivePacket;
    private Queue<Packet> sendPackets;

    private List<IPacketDataReceiver> packetReceiver;
    public static PacketManager instance;

    public List<IPacketDataReceiver> Receivers
    {
        get { return packetReceiver; }
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
        DontDestroyOnLoad(instance);

        receivePacket = new Queue<Packet>();
        sendPackets = new Queue<Packet>();
    }

    
    public void putPacket(Packet packet)
    {
        this.receivePacket.Enqueue(packet);
    }
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
