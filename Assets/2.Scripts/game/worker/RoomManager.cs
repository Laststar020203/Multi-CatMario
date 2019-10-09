﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 게임 맵 설정이라든지 이런것들을 담당
 * */
public class RoomManager : MonoBehaviour, IPacketDataReceiver, IEventListener
{
    public static RoomManager instance;

    private Room room;
    public Room RoomInfo { get { return room; }
        set {
            if(room == null)
            room = value;
        } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);

        PacketManager.instance.AddPacketDataReceiver(this);
        
    }

    private void Start()
    {
        AddListener();
    }

    private void UpdateMap(MapChangeEvent e)
    {
        this.room.Map = (byte)e.MapCode;
        if (GameManager.instance.Part == SocketPart.Server)
            PacketManager.instance.PutPacket(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.SYNC_ROOM_MAP, new byte[1] { this.room.Map }));

    }

    public void Receive(Packet packet)
    {
        try
        {
            if (packet.TypeCode == Packet.Type.SYNC_ROOM_MAP)
            {
                EventManager.CallEvent(new MapChangeEvent(packet.Body[0]));
            }
        }
        catch (System.Exception e)
        {

        }
    }

    public bool CheckResponsible(Packet packet)
    {
        if (packet.TypeCode == Packet.Type.SYNC_ROOM_MAP && GameManager.instance.Part == SocketPart.Client) return true;
        else return false;
    }

    public void AddListener()
    {
        EventManager.AddListener<MapChangeEvent>(UpdateMap);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<MapChangeEvent>(UpdateMap);
    }

    private void OnDestroy()
    {
        PacketManager.instance.RemovePacketDataReceiver(this);
        RemoveListener();
    }

    private void OnDisable()
    {
        PacketManager.instance.RemovePacketDataReceiver(this);
        RemoveListener();
    }
}
