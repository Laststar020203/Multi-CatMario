using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;


public class Packet
{
    private readonly byte sender;
    private readonly byte receiver;
    private readonly byte typecode;
    private readonly uint size;
    private readonly uint matchCode;
    private readonly byte[] head;
    private readonly byte[] body;
    private readonly byte[] packetData;


    public static class Target
    {
        public const byte SERVER = 0;
        public const byte ACCESS_REQUESTER = 1;
        public const byte ALL = 255;
    }
    public static class Type
    {
        public const byte REQUEST_ACCESS = 0x00; //접속 요청
        public const byte UNABLE_ACCESS = 0x01;
        public const byte SUCCESS_ACCESS = 0x02;
        public const byte FAIL = 0x04;
        public const byte HELLO = 0x05;
        public const byte OK = 0x07;
        public const byte REGISTER_CLIENT = 0x08;
        public const byte EXIT_CLIENT = 0x09;
        public const byte REQUEST_RE_SEND = 0x10;
        public const byte SYNC_PLAYER_POS_TOSERVER = 0x11;
        public const byte SYNC_PLAYER_DEAD = 0x12;
        public const byte SYNC_PLAYER_RESPAWN = 0x13;
        public const byte SYNC_PLAYER_ATTACK = 0x14;
        public const byte SYNC_PLAYER_POS_TOCLIENT = 0x15;
        public const byte SYNC_ROOM_MAP = 0x16;
        public const byte GAMESTART = 0x17;
        public const byte SYNC_CHAT = 0x18;
        public const byte SYNC_PLAYER_GOAL = 0x19;
        public const byte GAMEOVER = 0x20;
        public const byte KICK = 0x44;
        
    }

    public byte Sender { get { return sender; } }
    public byte Receiver { get { return receiver; } }
    public byte TypeCode { get { return typecode; } }
    public uint Size { get { return size; } }
    public uint MatchCode { get { return matchCode; } }
    public byte[] Head { get { return head; } }
    public byte[] Body { get { return body; } }
    public byte[] Data { get { return packetData; } }


    public Packet(byte sender, byte receiver, byte type) : this(sender, receiver, type, new byte[1] { 0})
    {
     
    }

    public Packet(byte sender, byte receiver, byte type, PacketData data): this(sender, receiver, type, data.PData)
    {

    }
    public Packet(byte sender, byte receiver, byte type, byte[] data)
    {

        this.sender = sender;
        this.receiver = receiver;
        this.head = new byte[11];


        this.typecode = type;
        head[0] = sender;
        head[1] = receiver;
        head[2] = type;


        this.size = Convert.ToUInt32(data.Length);

        Buffer.BlockCopy(BitConverter.GetBytes(size), 0, this.head, 3, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(matchCode), 0, this.head, 7, 4);

        this.body = data;

        this.packetData = new byte[11 + body.Length];

        Buffer.BlockCopy(head, 0, this.packetData, 0, head.Length);
        Buffer.BlockCopy(body, 0, this.packetData, 11, body.Length);


    }
}
