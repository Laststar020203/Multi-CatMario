using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 게임 맵 설정이라든지 이런것들을 담당
 * */
public class RoomManager : MonoBehaviour, IPacketDataReceiver, IEventListener
{
    public static RoomManager instance;

    private Room room;
    public Room RoomInfo { get { return room; }
        set {
            if(room == null)
            room = value;
        } }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);

        if(GameManager.instance.NetPart == SocketPart.Client)
        PacketManager.instance.AddPacketDataReceiver(this);
        
    }

    private void Start()
    {
        AddListener();
    }

    private void UpdateMap(MapChangeEvent e)
    {
        this.room.Map = (byte)e.MapCode;

        GameManager.instance.ShowMessage("1-" + (this.room.Map + 1) + " 로 맵이 변경되었습니다! (현재 1-1만 구현됬어요.. 그저 관상용.. )", 1.0f, MessageType.Commmon);

        if (GameManager.instance.NetPart == SocketPart.Server)
            PacketManager.instance.PutPacket(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.SYNC_ROOM_MAP, new byte[1] { this.room.Map }));

    }

    public void Receive(Packet packet)
    {
        try
        {
            try
            {
                if (packet.TypeCode == Packet.Type.SYNC_ROOM_MAP)
                {
                    EventManager.CallEvent(new MapChangeEvent(packet.Body[0]));
                }
            }
            catch (System.Exception e)
            {

            }
        }catch(System.Exception e)
        {
            GameManager.instance.ShowMessage(UnityEngine.Random.Range(0, 2) % 2 == 0 ? "패킷이 잘못전달되었습니다.. 개발자 일안하냐ㅏ!!"
                : "패킷이 잘못전달되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);
        }
    }

    public bool CheckResponsible(Packet packet)
    {
        if (packet.TypeCode == Packet.Type.SYNC_ROOM_MAP && GameManager.instance.NetPart == SocketPart.Client) return true;
        else return false;
    }

    public void AddListener()
    {
        EventManager.AddListener<MapChangeEvent>(UpdateMap);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<MapChangeEvent>(UpdateMap);
    }

    private void OnDestroy()
    {
        if (GameManager.instance.NetPart == SocketPart.Client)
            PacketManager.instance.RemovePacketDataReceiver(this);
        RemoveListener();
    }

    private void OnDisable()
    {
        if (GameManager.instance.NetPart == SocketPart.Client)
            PacketManager.instance.RemovePacketDataReceiver(this);
        RemoveListener();
    }
}
