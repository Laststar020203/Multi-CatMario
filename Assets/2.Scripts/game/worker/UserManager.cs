using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//클라이언트가 서버로부터 새로들어오는 유저를 처리할때 자신이 서버인경우 그냥 저장 근데 꼭 필요가 있을까?
public class UserManager : MonoBehaviour, IPacketDataReceiver
{

    //처음에 자신의 유저 정볼르 저장해놈

    private UserManager instance;
    private List<User> users;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
        DontDestroyOnLoad(instance);

        users = RoomManager.instance.RoomInfo.Personnel;
    }

    public void RegisterUser(User user)
    {
        users.Add(user);
        if (GameManager.instance.Part == SocketPart.Client)
            PacketManager.instance.putPacket(new Packet(Client.UserCode, Packet.Target.SERVER, Packet.Type.OK));


    }

    public void RegisterUser(List<User> user)
    {
        users.AddRange(users);
    }

    public void ExitUser(User user)
    {
        users.Remove(user);
        if (GameManager.instance.Part == SocketPart.Client)
            PacketManager.instance.putPacket(new Packet(Client.UserCode, Packet.Target.SERVER, Packet.Type.OK));
    }
 

    public bool CheckResponsible(byte type)
    {
        return false;
    }

    public void Receive(Packet packet)
    {
        
    }

}

