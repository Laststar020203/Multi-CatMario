using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerController : MonoBehaviour, IPacketDataReceiver, IEventListener
{

    // Start is called before the first frame update
    private Dictionary<byte, RemotePlayer> remotePlayers;
    public GameObject remotePrefab;
    public static RemotePlayerController instance;
    private LocalPlayer player;

    public Dictionary<byte, RemotePlayer> RemotePlayers { get { return remotePlayers; } }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        if (PacketManager.instance == null)
        {
            GameObject o = GameObject.Find("net");
            switch (GameManager.instance.NetPart)
            {
                case SocketPart.Server:
                    o.GetComponentInChildren<Server>().ServerClose();
                    break;
                case SocketPart.Client:
                    o.GetComponentInChildren<Client>().CloseSocket();
                    break;
                default:
                    break;
            }
        }

        PacketManager.instance.AddPacketDataReceiver(this);

        InitPlayer();
    }

    private void Start()
    {
        player = ClientGameSystem.instance.Player;
        AddListener();

        /*
        if (GameManager.instance.Part == SocketPart.Server)
            StartCoroutine(SendUsPos());
        */
    }

    private void InitPlayer()
    {
        if (remotePlayers != null) return;
        remotePlayers = new Dictionary<byte, RemotePlayer>();
        foreach (User u in RoomManager.instance.RoomInfo.Personnel.Values)
        {
            if (u.ID == GameManager.instance.Me.ID) continue;
            RegisterPlayer(u);
        }
    }

    private void RegisterPlayer(ClientJoinEvent e)
    {

        User user = e.User;
        GameObject newRemotePlayerObj = Instantiate(remotePrefab, Vector2.zero, Quaternion.identity);
        RemotePlayer remotePlayer = newRemotePlayerObj.AddComponent<RemotePlayer>();

        remotePlayer.Init(user.ID, user.Name, user.CharacteID);
        remotePlayers.Add(user.ID, remotePlayer);
    }

    private void RegisterPlayer(User u)
    {
        User user = u;
        GameObject newRemotePlayerObj = Instantiate(remotePrefab, Vector2.zero, Quaternion.identity);
        RemotePlayer remotePlayer = newRemotePlayerObj.AddComponent<RemotePlayer>();
        remotePlayer.Init(user.ID, user.Name, user.CharacteID);
        remotePlayers.Add(user.ID, remotePlayer);
    }

    private void ExitPlayer(ClientQuitEvent e)
    {
        User user = e.User;
        if (!remotePlayers.ContainsKey(user.ID)) return;
        RemotePlayer remotePlayer = remotePlayers[user.ID];
        Destroy(remotePlayer.gameObject);
        remotePlayers.Remove(user.ID);
    }

    public bool CheckResponsible(Packet packet)
    {
        byte type = packet.TypeCode;
        if ((type >= 0x12 && type <= 14) || (GameManager.instance.NetPart == SocketPart.Client && type == 0x15) || (GameManager.instance.NetPart == SocketPart.Server && type == 0x11)) return true;
        else return false;
    }


    private void RemotePlayerSurrend(SurrendEvent e)
    {
        if (remotePlayers.ContainsKey(e.ID))
        {
            remotePlayers[e.ID].Stat = PlayerStat.Spectator;
        }
    }

    private void OffRemotePlayerSurrend(GameEndEvent e)
    {
        foreach (Player p in remotePlayers.Values)
        {
            if (p.Stat == PlayerStat.Spectator)
                p.Stat = PlayerStat.Player;
        }
    }

    public void Receive(Packet packet)
    {
        try
        {
            if (!remotePlayers.ContainsKey(packet.Sender)) return;
            RemotePlayer rp = remotePlayers[packet.Sender];
            switch (packet.TypeCode)
            {
                case Packet.Type.SYNC_PLAYER_POS:
                    rp.SyncPosition((Vector2)new UnityPacketData(packet.Body, packet).OData);
                    break;
                case Packet.Type.SYNC_PLAYER_DEAD:
                    rp.Die();
                    break;
                case Packet.Type.SYNC_PLAYER_RESPAWN:
                    rp.Respawn();
                    break;          
            }
        }catch(System.Exception e)
        {
            Debug.Log(e);
            GameManager.instance.ShowMessage(UnityEngine.Random.Range(0, 2) % 2 == 0 ? "패킷이 잘못전달되었습니다.. 개발자 일안하냐ㅏ!!"
                : "패킷이 잘못전달되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);
        }
    }

    private void OnDisable()
    {
        RemoveListener();
        PacketManager.instance.RemovePacketDataReceiver(this);
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        RemoveListener();
        StopAllCoroutines();
        PacketManager.instance.RemovePacketDataReceiver(this);

    }

    public void AddListener()
    {
        EventManager.AddListener<ClientJoinEvent>(RegisterPlayer);
        EventManager.AddListener<ClientQuitEvent>(ExitPlayer);
        EventManager.AddListener<SurrendEvent>(RemotePlayerSurrend);
        EventManager.AddListener<GameEndEvent>(OffRemotePlayerSurrend);
    }

    public void RemoveListener()
    {

        EventManager.RemoveListener<ClientJoinEvent>(RegisterPlayer);
        EventManager.RemoveListener<ClientQuitEvent>(ExitPlayer);
        EventManager.RemoveListener<SurrendEvent>(RemotePlayerSurrend);
        EventManager.RemoveListener<GameEndEvent>(OffRemotePlayerSurrend);
    }
}
