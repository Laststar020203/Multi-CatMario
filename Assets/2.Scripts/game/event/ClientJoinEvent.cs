using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class ClientJoinEvent : Event
{
    public static readonly List<GameEvent<ClientJoinEvent>> listeners = new List<GameEvent<ClientJoinEvent>>();

    private TcpClient client;
    private User user;

    public TcpClient Client { get { return client; } }
    public User User { get { return user; } }

    public ClientJoinEvent(TcpClient client, User user)
    {
        this.client = client;
        this.user = user;
    }
   
}

public class ClientQuitEvent : Event
{
    public static readonly List<GameEvent<ClientQuitEvent>> listeners = new List<GameEvent<ClientQuitEvent>>();

    private ServerClient client;
    private User user;

    public ServerClient Client { get { return client; } }
    public User User { get { return user; } }

    public ClientQuitEvent(ServerClient client, User user)
    {
        this.client = client;
        this.user = user;
    }


}
