﻿using System.Collections;
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


            Debug.Log("Connect Succes!");

            Destroy(this.gameObject, waitSecond);

            StartCoroutine(w());
        }
        catch(Exception e)
        {
            Debug.Log("Error Socket : " + e.Message);
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


    }

    private void Entrance()
    {
        EntranceSceneManager.instance.CreateRoomManager(room);
        LoadSceneManager.instance.NextSceneLoad();
        succues = true;

        Debug.Log(room.Title + "  " + room.HeadCount + " " + room.MaxUser);
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
