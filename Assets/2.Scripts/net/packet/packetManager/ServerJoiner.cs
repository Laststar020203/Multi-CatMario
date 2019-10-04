using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerJoiner : MonoBehaviour, IPacketDataReceiver
{

    private Client client;
    private string ip;
    private float waitSecond;

    private bool running;
    CallBackTask task;

    Action fail;

    private void Start()
    {
        if(PacketManager.instance.Receivers == null)
        {
            
        }

        PacketManager.instance.Receivers.Add(this);
    }

    public void Join(string ip, float waitSecond, Action fail)
    {
        this.client = EntranceSceneManager.instance.Client;
        client.ConnectToServer(ip);

        //접속요청 
        PacketManager.instance.putPacket(new Packet(Packet.Target.SERVER, Client.UserCode , Packet.Type.REQUEST_ACCESS, GameManager.instance.Me));

        this.ip = ip;
        this.waitSecond = waitSecond;

        this.fail = fail;

        task = new CallBackTask(waitSecond, null, () =>
        {
            fail();
            Destroy(this);
        });
        task.Run();

    }

    public void Receive(Packet packet)
    {
        if (!running) return;

        switch (packet.TypeCode)
        {
            case Packet.Type.SUCCESS_ACCESS:
                fail();
                break;
            case Packet.Type.UNABLE_ACCESS:
                task.Stop();

                Debug.Log("Access_Success!");
                /*
                 * ...
                 */
                Entrance();
                break;
        }


    }

    public bool CheckResponsible(byte type)
    {
        if (type == Packet.Type.UNABLE_ACCESS || type == Packet.Type.SUCCESS_ACCESS) return true;
        else return false;
    }

    private void Entrance()
    {

    }

    private void OnDestroy()
    {
        PacketManager.instance.Receivers.Remove(this);

        fail();
        task.Stop();
    }
}
