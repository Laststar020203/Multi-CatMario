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
        if(receivePacket.Count != 0)
        {
            Debug.Log(Encoding.UTF8.GetString(receivePacket.Dequeue().Body));
        }
    }
}
