using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class ReceiveNewClientEvent : Event
{
    public static readonly List<GameEvent<ReceiveNewClientEvent>> listeners = new List<GameEvent<ReceiveNewClientEvent>>();

    private TcpClient client;
    public TcpClient Client { get { return client; } }

    public ReceiveNewClientEvent(TcpClient client)
    {
        this.client = client;
    }


}

public class ReceiveNewClientPacketEvent : Event
{
    public static readonly List<GameEvent<ReceiveNewClientPacketEvent>> listeners = new List<GameEvent<ReceiveNewClientPacketEvent>>();

    private Packet packet;
    public Packet Packet { get { return packet; } }

    public ReceiveNewClientPacketEvent(Packet packet)
    {
        this.packet = packet;
    }
}
