using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Room : PacketData
{
    private string title;
    private byte maxUser;
    private Dictionary<byte, User> personnel;
    private byte map;

    public string Title { get { return title; } }
    public Dictionary<byte, User> Personnel { get { return personnel; } set { personnel = value; } }
    public byte MaxUser { get { return maxUser; } }
    public int HeadCount { get { return personnel.Count; }}
    public byte Map { get { return map; } set { { map = value; } } }

    public Room(string title, byte map, byte maxUser)
    {
        this.title = title;
        personnel = new Dictionary<byte, User>();
        this.map = map;
        this.maxUser = maxUser;
    }

    public Room(byte[] data) : base(data)
    {
       
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Room)) return false;
        Room room = (Room) obj;

        Debug.Log("FF");

        if (this.title != room.Title || this.maxUser != room.MaxUser || this.HeadCount != room.HeadCount || this.map != room.Map) return false;

        Debug.Log("SF");

        foreach (byte b in room.Personnel.Keys)
        {
            Debug.Log("JE" + b);
            if (!this.personnel.ContainsKey(b))
                return false;
        }

        return true;
    }

    protected override byte[] Serialize()
    {
        // packetData : Title - MaxUser - HeadCount - Personnel - Map
        this.pData = new byte[21 + 17 * HeadCount];
        byte[] title = Encoding.UTF8.GetBytes(this.title);
        Buffer.BlockCopy(title, 0, pData, 0, title.Length);
        this.pData[18] = maxUser;
        this.pData[19] = (byte)HeadCount;

        int count = 0;
        foreach(User user in personnel.Values)
        {
            Buffer.BlockCopy(user.PData, 0, this.pData, 20 + count * 17, 17);
            count++;
        }

        this.pData[this.pData.Length - 1] = map;

        return pData;
    }

    protected override void Deserialize()
    {
        byte[] title = new byte[18];
        Buffer.BlockCopy(this.pData, 0, title, 0, title.Length);


        this.title = Encoding.UTF8.GetString(title);


        this.maxUser = this.pData[18];
        int headCount = this.pData[19];

        personnel = new Dictionary<byte, User>();


        for (int i = 0; i < headCount; i++)
        {
            byte[] user = new byte[17];
            Buffer.BlockCopy(this.pData, 20 + i * 17, user, 0 , 17);
            User userO = new User(user);
            personnel.Add(userO.ID, userO);
        }

        this.map = this.pData[this.pData.Length - 1];



    }
}
