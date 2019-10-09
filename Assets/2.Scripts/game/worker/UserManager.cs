using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//클라이언트가 서버로부터 새로들어오는 유저를 처리할때 자신이 서버인경우 그냥 저장 근데 꼭 필요가 있을까?
public class UserManager : MonoBehaviour, IPacketDataReceiver, IEventListener
{

    //처음에 자신의 유저 정볼르 저장해놈

    public static UserManager instance;
    private Dictionary<byte , User> users;
    private Server server;

    //Request요청을 날릴리 절대 없음
    public bool IsRequesting => false;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        users = RoomManager.instance.RoomInfo.Personnel;

        server = GameObject.Find("net").GetComponentInChildren<Server>();

        PacketManager.instance.AddPacketDataReceiver(this);

        if (!users.ContainsKey(GameManager.instance.Me.ID))
            users.Add(GameManager.instance.Me.ID, GameManager.instance.Me);

    }
    private void Start()
    {
        AddListener();

    }

    private void RegisterUser(ClientJoinEvent e)
    {
        lock (users)
        {
            User user = e.User;
            users.Add(user.ID, user);
        }
    }

    private void ExitUser(ClientQuitEvent e)
    {
        lock (users)
        {
            users.Remove(e.User.ID);
        }
    }
 
    public void Receive(Packet packet)
    {
        User newUser;
        try
        {
            newUser = new User(packet.Body);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            PacketManager.instance.PutPacket(new Packet(Client.UserCode, Packet.Target.SERVER, Packet.Type.REQUEST_RE_SEND));
            return;
        }
        switch (packet.TypeCode)
        {
            case Packet.Type.REGISTER_CLIENT:
                EventManager.CallEvent(new ClientJoinEvent(null, newUser));
                break;
            case Packet.Type.EXIT_CLIENT:
                EventManager.CallEvent(new ClientQuitEvent(null, newUser));
                break;
        }
    }

    public bool CheckResponsible(Packet packet)
    {
        if (packet.TypeCode == Packet.Type.REGISTER_CLIENT || packet.TypeCode == Packet.Type.EXIT_CLIENT) return true;
        else return false;
    }

    public User GetUser(ServerClient client)
    {
        if (users.ContainsKey(client.ID))
            return users[client.ID];
        else
            return null;
    }
    private void Update()
    {
        
    }

    private void OnDestroy()
    {
        RemoveListener();
        PacketManager.instance.RemovePacketDataReceiver(this);

    }

    private void OnDisable()
    {
        RemoveListener();
        PacketManager.instance.RemovePacketDataReceiver(this);

    }

    public void AddListener()
    {
        EventManager.AddListener<ClientJoinEvent>(RegisterUser);
        EventManager.AddListener<ClientQuitEvent>(ExitUser);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<ClientJoinEvent>(RegisterUser);
        EventManager.RemoveListener<ClientQuitEvent>(ExitUser);
    }
}

