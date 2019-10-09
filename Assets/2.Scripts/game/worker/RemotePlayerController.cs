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

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        if (PacketManager.instance == null)
        {
            GameObject o = GameObject.Find("net");
            switch (GameManager.instance.Part)
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
        RemotePlayer remotePlayer = newRemotePlayerObj.GetComponent<RemotePlayer>();
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
        if ((type >= 0x12 && type <= 14) || (GameManager.instance.Part == SocketPart.Client && type == 0x15) || (GameManager.instance.Part == SocketPart.Server && type == 0x11)) return true;
        else return false;
    }

    private Dictionary<byte, Vector2> getAllPos()
    {
        Dictionary<byte, Vector2> d = new Dictionary<byte, Vector2>();
        foreach(byte key in remotePlayers.Keys)
        {
            d.Add(key, remotePlayers[key].Pos);
        }
        d.Add(GameManager.instance.Me.ID, player.Pos);

        return d;
    }

    private void Update()
    {
        if (GameManager.instance.Part == SocketPart.Server)
        {
            PacketManager.instance.PutPacket(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.SYNC_PLAYER_POS_TOCLIENT, new UnityPacketData(getAllPos())));

        }


    }

    public void Receive(Packet packet)
    {
        if (!remotePlayers.ContainsKey(packet.Sender)) return;
        switch (packet.TypeCode)
        {
            case Packet.Type.SYNC_PLAYER_POS_TOSERVER:
                RemotePlayer rp = remotePlayers[packet.Sender];
                rp.SyncPosition((Vector2)new UnityPacketData(packet.Body, packet).OData);
                break;
            case Packet.Type.SYNC_PLAYER_DEAD:

                break;
            case Packet.Type.SYNC_PLAYER_RESPAWN:

                break;
            case Packet.Type.SYNC_PLAYER_POS_TOCLIENT:
                Dictionary<byte, Vector2> d = (Dictionary<byte,Vector2>) new UnityPacketData(packet.Body, packet).OData;
                foreach(byte b in d.Keys)
                {
                    if (b == GameManager.instance.Me.ID) continue;
                    Debug.Log(b + "pos : " + d[b]);
                    remotePlayers[b].SyncPosition(d[b]);

                }
                break;
        }
    }

    private void OnDisable()
    {
        RemoveListener();
        PacketManager.instance.RemovePacketDataReceiver(this);

    }

    private void OnDestroy()
    {
        RemoveListener();
        PacketManager.instance.RemovePacketDataReceiver(this);

    }

    public void AddListener()
    {
        EventManager.AddListener<ClientJoinEvent>(RegisterPlayer);
        EventManager.AddListener<ClientQuitEvent>(ExitPlayer);
    }

    public void RemoveListener()
    {

        EventManager.RemoveListener<ClientJoinEvent>(RegisterPlayer);
        EventManager.RemoveListener<ClientQuitEvent>(ExitPlayer);
    }
}
