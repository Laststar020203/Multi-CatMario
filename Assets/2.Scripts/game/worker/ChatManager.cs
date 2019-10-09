using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class ChatManager : MonoBehaviour, IPacketDataReceiver
{
    public Text output;
    public InputField input;
    private static ChatManager instance;
    private Dictionary<byte, User> users;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        users = RoomManager.instance.RoomInfo.Personnel;



    }

    private void Start()
    {
        PacketManager.instance.AddPacketDataReceiver(this);
        output.text = "";
        DontDestroyOnLoad(output.transform.parent.gameObject);
        DontDestroyOnLoad(input.transform.parent.gameObject);

    }

    public void SendMessagButton()
    {
        string msg = input.text;
        input.text = "";

        Append(GameManager.instance.Me, msg);
        PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_CHAT, Encoding.UTF8.GetBytes(msg)));
    }

    private void Append(User user, string msg)
    {
        msg = "[" + user.Name + "] : " + msg;
        output.text = output.text + "\n" + TextColor.Wear(user.CharacteID, msg);

    }


    public void Receive(Packet packet)
    {
        string meg = Encoding.UTF8.GetString(packet.Body);
        Debug.Log(meg);
        Append(RoomManager.instance.RoomInfo.Personnel[packet.Sender], meg);
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
