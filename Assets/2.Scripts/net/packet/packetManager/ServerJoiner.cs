using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServerJoiner : MonoBehaviour, IPacketDataReceiver, IRequester
{

    private Client client;
    private string ip;
    private float waitSecond;
    private Action end;
    private bool isRequesting;
    private bool succues;
    public bool IsRequesting => isRequesting;
    public byte RequestNumber => 2;
    private Room room;

    private void Start()
    {
        PacketManager.instance.AddPacketDataReceiver(this);
    }

    public void Join(string ip, float waitSecond, Action end)
    {
        this.client = EntranceSceneManager.instance.Client;
        this.end = end;
        succues = false;
        try
        {
            client.ConnectToServer(ip);
            this.ip = ip;
            this.waitSecond = waitSecond;



            Destroy(this.gameObject, waitSecond);

            StartCoroutine(w());
        }
        catch(Exception e)
        {
            Destroy(this.gameObject);
        }       
    }

    private IEnumerator w()
    {
        while (room == null) yield return null;
        PacketManager.instance.PutPacket(new Packet(Packet.Target.SERVER, Client.UserCode, Packet.Type.REQUEST_ACCESS, GameManager.instance.Me), this);

        isRequesting = true;
    }

    public void Receive(Packet packet)
    {
        try
        {
            switch (packet.TypeCode)
            {
                case Packet.Type.HELLO:
                    //room 임시 저장
                    this.room = new Room(packet.Body);
                    break;
                case Packet.Type.FAIL:
                    Destroy(this.gameObject);
                    break;

                case Packet.Type.OK:

                    byte myId = packet.Body[1];
                    GameManager.instance.Me.ID = myId;
                    Entrance();

                    break;
            }

        }catch(Exception e)
        {
            GameManager.instance.ShowMessage(UnityEngine.Random.Range(0, 2) % 2 == 0 ? "패킷이 잘못전달되었습니다.. 개발자 일안하냐ㅏ!!"
                : "패킷이 잘못전달되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);
        }


    }

    private void Entrance()
    {
        EntranceSceneManager.instance.CreateRoomManager(room);
        LoadSceneManager.instance.NextSceneLoad();
        succues = true;

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        StopCoroutine(w());

        if (!succues)
        {
            client.CloseSocket();
            end();
        }
    }

    private void OnDisable()
    {
        StopCoroutine(w());

        if (!succues)
        {
            client.CloseSocket();
            end();
        }
    }

    public bool CheckResponsible(Packet packet)
    {
        byte type = packet.TypeCode;
        if ((isRequesting && (type == Packet.Type.OK || type == Packet.Type.FAIL)) || type == Packet.Type.HELLO) return true;
        else return false;
    }
}
