using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class PacketData
{

    //데이터는 Packet으로 정하고 그걸 묶는것은 buffer가 처리하도록 한다.
    /*
    private readonly uint size;
    private readonly byte receiver;
    private readonly byte type;
    private readonly byte[] data;


    public static class Types
    {
        public const byte UNABLE_ACCES = 0x01;
        public const byte ACCESS_SUCCESS = 0x02;
    }

    public uint Size { get { return size; } }
    public byte Receiver { get { return receiver; } }
    public byte Type { get { return type; } }
    public byte[] Data { get { return data; } }



    public PacketData(byte sender, byte receiver, byte type)
    {
        this.type = type;
        this.receiver = receiver;
        this.data = new byte[1];
        this.size = 6;
    }

    public PacketData(byte sender, byte receiver, byte type, string data)
    {
        this.type = type;
        this.receiver = receiver;
        this.data = Encoding.UTF8.GetBytes(data);
        size = Convert.ToUInt32(this.data.Length) + 5;
    }

    */
}