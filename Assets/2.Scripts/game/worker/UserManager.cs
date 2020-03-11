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
            //string msg = user.Name + " 님이 입장하였습니다!";
            GameManager.instance.ShowMessage(user.Name, 1.0f, MessageType.Commmon);
            GameManager.instance.ShowMessage("입장", 1.0f, MessageType.Commmon);

        }
    }

    private void ExitUser(ClientQuitEvent e)
    {
        lock (users)
        {
            
            users.Remove(e.User.ID);
            GameManager.instance.ShowMessage(e.User.Name, 1.0f, MessageType.Commmon);
            GameManager.instance.ShowMessage("퇴장", 1.0f, MessageType.Commmon);
        }
    }
 
    public void Receive(Packet packet)
    {
        try
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

        }catch(Exception e)
        {
            GameManager.instance.ShowMessage(UnityEngine.Random.Range(0, 2) % 2 == 0 ? "패킷이 잘못전달되었습니다.. 개발자 일안하냐ㅏ!! "
                : "패킷이 잘못전달되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);

            GameManager.instance.NetEscape();
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

