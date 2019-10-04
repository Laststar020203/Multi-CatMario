using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class Room : PacketData
{
    private readonly string title;
    private readonly byte maxUser;
    private List<User> personnel;
    private byte map;

    public string Title { get { return title; } }
    public List<User> Personnel { get { return personnel; } }
    public byte MaxUser { get { return maxUser; } }
    public int HeadCount { get { return personnel.Count; } }
    public byte Map { get { return map; } }

    public Room(string title, byte map, int maxUser)
    {
        this.title = title;
        personnel = new List<User>();
    }


    protected override byte[] Serialize()
    {
        // packetData : Title - MaxUser - HeadCount - Personnel - Map
        this.pData = new byte[18 + 1 + 1 + 16 * HeadCount + 1];
        byte[] title = Encoding.UTF8.GetBytes(this.title);
        Buffer.BlockCopy(title, 0, PData, 0, title.Length);
        this.pData[18] = maxUser;
        this.pData[19] = (byte)HeadCount;
        for(int i = 0; i < HeadCount; i++)
        {
            Buffer.BlockCopy(personnel[i].PData, 0, this.pData, 20 + i * 16, personnel[i].PData.Length);
        }
        this.pData[this.pData.Length - 1] = map;

        return pData;
    }

    protected override object Deserialize()
    {
        throw new System.NotImplementedException();
    }
}
