using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class Client : MonoBehaviour
{
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream ns;
    private Stream stream;

    private static byte userCode = Packet.Target.ACCESS_REQUESTER;
    public static byte UserCode { get { return userCode; } set { userCode = value; } }                                                                                                                                                                                                                           

    private void Start()
    {
        
    }

    public void ConnectToServer(string ip)
    {
        if (socketReady)
            return;
        try
        {
            socket = new TcpClient(ip, 5252);
            ns = socket.GetStream();
            stream = new BufferedStream(stream);
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
            //Send(new Packet(0, 0, Packet.Type.ACCESS_SUCCESS, "Connect!"));
            if (ns.DataAvailable)
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

        if(stream != null)
        stream.Close();
        if(socket != null)
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
