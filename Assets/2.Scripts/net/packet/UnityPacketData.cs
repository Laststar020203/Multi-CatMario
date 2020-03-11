using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class UnityPacketData : PacketData
{

    private Packet packet;
    private object odata;
    public object OData { get { return odata; } }


    private UnityPacketData(byte[] data) : base(data)
    { 

    }

    public UnityPacketData(byte[] data, Packet packet)
    {
        this.pData = data;
        this.packet = packet;
        Deserialize();
    }

    public UnityPacketData(object o)
    {
        this.odata = o;
    }
    public UnityPacketData(Vector2 v2) : this((object)v2) { }

    protected override void Deserialize()
    {
        switch (packet.TypeCode)
        {
            case Packet.Type.SYNC_PLAYER_POS:
                odata = ToVector2ByByte(0);
                break;
            case Packet.Type.SYNC_PLAYER_POS_TOCLIENT:
                if (this.pData.Length % 9 != 0) throw new IndexOutOfRangeException();

                int count = this.pData.Length / 9;
                Dictionary<byte, Vector2> d = new Dictionary<byte, Vector2>();
                for (int i = 0; i < count; i++)
                {
                    d.Add(this.pData[i * 9], ToVector2ByByte(i * 9 + 1));

                }
                this.odata = d;
                break;
        }
    }

    protected override byte[] Serialize()
    {
        if(odata is Vector2)
        {
            this.pData = new byte[8];
            ToByteByVector2((Vector2)odata, 0);
            return pData;

        }else if(odata is Dictionary<byte, Vector2>)
        {
            Dictionary<byte, Vector2> lv = (Dictionary<byte, Vector2>)odata;
            int count = lv.Count;
            this.pData = new byte[9 * count];

            int index = 0;
            foreach(byte b in lv.Keys)
            {
                this.pData[index * 9] = b;
                Vector2 v = lv[b];
                ToByteByVector2(v, index * 9 + 1);
                index++;  
            }
            return pData;
        }
        return null;
    }

    private void ToByteByVector2(Vector2 v , int startindex)
    {

        Vector2 v2 = v;
        byte[] x = BitConverter.GetBytes(v2.x);
        byte[] y = BitConverter.GetBytes(v2.y);

        Buffer.BlockCopy(x, 0, this.pData, startindex, x.Length);
        Buffer.BlockCopy(y, 0, this.pData, startindex + 4, y.Length);
    }

    private Vector2 ToVector2ByByte(int startIndex)
    {
        byte[] xb = new byte[4];
        byte[] yb = new byte[4];

        Buffer.BlockCopy(this.pData, startIndex, xb, 0, 4);
        Buffer.BlockCopy(this.pData, startIndex + 4, yb, 0, 4);

        float x = BitConverter.ToSingle(xb, 0);
        float y = BitConverter.ToSingle(yb, 0);

        return new Vector2(x, y);
    }
}