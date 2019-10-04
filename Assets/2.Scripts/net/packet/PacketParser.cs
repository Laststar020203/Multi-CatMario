using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.Net.Sockets;
using System.IO;

public class PacketParser
{
  
    public static void Pasing(Stream e, out Packet packet)
    {
        byte[] header = new byte[11];
        e.Read(header, 0, 11);
        byte sender = header[0];
        byte receiver = header[1];
        byte type = header[2];
        uint size = BitConverter.ToUInt32(header, 3);
        uint mathcode = BitConverter.ToUInt32(header, 7);


        byte[] body = new byte[size];
        e.Read(body, 0, Convert.ToInt32(size));

        packet = new Packet(sender, receiver, type,body);
        
    }
        /*
    //리틀 엔디안
    private static byte[] getPacketDataToByteArray(PacketData pd)
    {
        byte[] data = new byte[pd.Size + 1];
        Buffer.BlockCopy(data, 0, BitConverter.GetBytes(pd.Size), 0, 4);
        data[5] = pd.Type;
        Buffer.BlockCopy(data, 6, pd.Data, 0, pd.Data.Length);
        data[data.Length - 1] = (byte) '&'; //데이터의 끝을 나타냄

        return data;
    }
    */
    

}
