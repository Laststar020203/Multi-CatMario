using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class Client : MonoBehaviour
{
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;

         
    private void Start()
    {
        //ConnectToServer("127.0.0.1", 5252);
    }

    public void ConnectToServer(int port)
    {
        if (socketReady)
            return;
        try
        {
            socket = new TcpClient("127.0.0.1", port);
            stream = socket.GetStream();
            socketReady = true;
            Debug.Log("Connect Succes!");

            
        }
        catch (System.Exception e)
        {
            Debug.Log("Socket Error : " + e.Message);
        }

    }

    void Update()
    {
        
        if (socketReady)
        {
            Send(new Packet(0, 0, Packet.Type.ACCESS_SUCCESS, "Connect!"));
            if (stream.DataAvailable)
            {
               
                Packet packet;
                PacketParser.Pasing(stream, out packet);
                if (packet != null)
                {
                    PacketManager.instance.putPacket(packet);
                }
            }
        }
    }

    private void Send(Packet packet)
    { 
        byte[] writerData = packet.Data;
        stream.Write(writerData, 0, writerData.Length);
    }

    private void CloseSocket()
    {
        if (!socketReady)
            return;

        stream.Close();
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }

}
