using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, IPacketDataReceiver
{
    public Text output;
    public InputField input;
    private static ChatManager instance;
    private Dictionary<byte, User> users;

    private Encoding enc;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        users = RoomManager.instance.RoomInfo.Personnel;

        enc = Encoding.GetEncoding(51949);

    }

    private void Start()
    {
        PacketManager.instance.AddPacketDataReceiver(this);
        output.text = "";

    }

    public void SendMessagButton()
    {
       
        string msg = input.text;
        input.text = "";

        Append(GameManager.instance.Me, msg);
        PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_CHAT, enc.GetBytes(msg)));
    }

    private void Append(User user, string msg)
    {

        string name = user.Name;
        msg = "[" + name + "] : " + msg;
        output.text = output.text + "\n" + msg;
        Debug.Log(msg);
    }


    public void Receive(Packet packet)
    {
        try
        {
            Debug.Log("Y");
            string meg = enc.GetString(packet.Body);
            Append(RoomManager.instance.RoomInfo.Personnel[packet.Sender], meg);
            Debug.Log(meg);

        }catch(System.Exception e){
            GameManager.instance.ShowMessage(UnityEngine.Random.Range(0, 2) % 2 == 0 ? "패킷이 잘못전달되었습니다.. 개발자 일안하냐ㅏ!!"
                : "패킷이 잘못전달되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);
        }
    }

    public bool CheckResponsible(Packet packet)
    {
        if (packet.TypeCode == Packet.Type.SYNC_CHAT) return true;
        else return false;
    }

    private void OnDisable()
    {
        PacketManager.instance.RemovePacketDataReceiver(this);
    }

    private void OnDestroy()
    {
        PacketManager.instance.RemovePacketDataReceiver(this);


    }
}
