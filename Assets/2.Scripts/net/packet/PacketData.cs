using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PacketData
{
    protected byte[] pData;
    public byte[] PData { get {return Serialize();
        } }

    public PacketData()
    {

    }
    public PacketData(byte[] data)
    {
        pData = data;
        Deserialize();
    }

    protected abstract byte[] Serialize();
    protected abstract void Deserialize();
}
