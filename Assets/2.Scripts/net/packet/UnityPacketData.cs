using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class UnityPacketData : PacketData
{
    public UnityPacketData(byte[] data) : base(data)
    {
    }

    protected override object Deserialize()
    {
        throw new NotImplementedException();
    }

    protected override byte[] Serialize()
    {
        throw new NotImplementedException();
    }
}