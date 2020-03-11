using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//ONly Game SYstem

public class ClientGameSystem : MonoBehaviour, IEventListener, IPacketDataReceiver
{
    public static ClientGameSystem instance;
    private LocalPlayer player;
    public float countDown = 10f;
    private int playerLifeCount;
    public GameObject deadBoard;
    public Animator warnning;
    public GameObject fireBall;
    private CameraController camera;
    private bool troll;
    private Vector2 spawnPoint;
    private List<User> goaler;
    private List<User> surrender;
    private GameObject empCheckPoint;
    private bool surrend;
    private bool noneGoal;
    public AudioSource audioSource;
    

    public bool end = false;
    public LocalPlayer Player { get { return player; } }
    public bool Troll { get { return troll; } set { troll = value; } }
    public Vector2 SpawnPoint { get { return spawnPoint; } }
    public bool Surrenning { get { return surrend; } }
    public List<User> Goaler { get { return goaler; } }

    // Start is called before the first frame update
    private void Awake()
    {
        try
        {
            player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<LocalPlayer>();
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("Not Player!");
        }

        camera = GameObject.FindGameObjectWithTag("GAMECAMERA").GetComponent<CameraController>();
        
    }

    private void OnEnable()
    {

    }

    private void Start()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        deadBoard.SetActive(false);
        PacketManager.instance.AddPacketDataReceiver(this);


        playerLifeCount = 3;

        audioSource.volume = GameManager.instance.Setting.SoundValue;

        AddListener();
    }

    private void PlayerSurrend(SurrendEvent e)
    {
        User surrendUser = RoomManager.instance.RoomInfo.Personnel[e.ID];
        if (e.ID == this.player.ID)
        {
            
            PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_SURREND, new byte[1] { GameManager.instance.Me.ID }));
            GameManager.instance.ShowMessage("당신은 항복하였습니다!", 1.0f, MessageType.Important);

            if (RoomManager.instance.RoomInfo.HeadCount == 1)
            {
                GameOver(this.goaler);
                return;
            }
            GameManager.instance.ShowMessage("당신은 관전자!!", 1.0f, MessageType.Commmon);
            player.Stat = PlayerStat.Spectator;
        }
        else
        {

            GameManager.instance.ShowMessage("sdfsdf 님이 항복하였습니다!", 1.0f, MessageType.Important);
        }

        this.surrender.Add(surrendUser);

        if (GameManager.instance.NetPart == SocketPart.Server)
        {
            if(surrender.Count == (RoomManager.instance.RoomInfo.HeadCount - 1) || RoomManager.instance.RoomInfo.HeadCount == 1)
            {
                foreach(User u in RoomManager.instance.RoomInfo.Personnel.Values) {
                    if (u.ID != e.ID && !goaler.Contains(u))
                    {
                        goaler.Add(u);
                    }
                }

                goaler.Sort(delegate (User one, User two)
                {
                    float oneX = RemotePlayerController.instance.RemotePlayers[one.ID].Pos.x;
                    float twoY = RemotePlayerController.instance.RemotePlayers[two.ID].Pos.y;

                    if (oneX > twoY) return 1;
                    else if (oneX == twoY) return 0;
                    else return -1;

                });

                GameOver(this.goaler);
            }

        }


    }


    private void StartGameSystem(GameStartEvent e)
    {

        if (player.Pos.y > 10) player.Teleport(Vector2.zero);
        GameManager.instance.GameStat = GameStat.Game;
        StartCoroutine(GameReadyRoutin());
        playerLifeCount = 3;
        
        noneGoal = true;

        goaler = new List<User>();
        surrender = new List<User>();
        
        audioSource.Play();

    }

    private void EndGameSystem(GameEndEvent e)
    {
        GameManager.instance.GameStat = GameStat.Wait;
        GameRoomSceneManager.instance.UpdateUI();

        end = true;
        player.Stat = PlayerStat.Player;
        audioSource.Stop();

        spawnPoint = Vector2.zero;
    }



    private void GameOver(List<User> goaler)
    {
        StopAllCoroutines();
        if (GameManager.instance.NetPart == SocketPart.Server)
        {
            byte[] goalerData = new byte[this.goaler.Count];
            for (int i = 0; i < goaler.Count; i++)
            {
                goalerData[i] = goaler[i].ID;
            }



            PacketManager.instance.PutPacket(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.GAMEOVER, goalerData));
        }
        else
        {
          
            this.goaler = goaler;
       

        }

        GameManager.instance.ShowMessage("게임 종료!", 1.0f, MessageType.Commmon);
        //camera.SetClamp(camera.MinX, 10f, camera.MinY, 40f);

        audioSource.Stop();
        LoadSceneManager.instance.WinnerSceneLoad();

        player.Respawn();
        deadBoard.SetActive(false);
        EventManager.CallEvent(new GameEndEvent());
    }


    private void PlayerDIe(PlayerDeathEvent e)
    {
        if (player.Stat == PlayerStat.Spectator)
        {
            return;
        }
        audioSource.Stop();
        --playerLifeCount;
        StartCoroutine(PlayerDeadRoutin());
    }

    private IEnumerator PlayerDeadRoutin()
    {
        deadBoard.SetActive(true);
        deadBoard.GetComponentInChildren<Text>().text = "x " + playerLifeCount;
        yield return new WaitForSeconds(2f);
        deadBoard.SetActive(false);
        player.Respawn();
        if(GameManager.instance.GameStat == GameStat.Game)
        audioSource.Play();
    }

    private IEnumerator GameReadyRoutin()
    {

        GameManager.instance.ShowMessage("이제 곧 게임이 시작됩니다!", 1f, MessageType.Important);
        GameManager.instance.ShowMessage("오른쪽에 출구가 생깁니다 모두들 준비해주세요!!",1f, MessageType.Commmon);
        GameManager.instance.ShowMessage("준비~", 1f, MessageType.Commmon);

        if(RoomManager.instance.RoomInfo.HeadCount == 1)
            GameManager.instance.ShowMessage("근데 혼자서 하면 재미없어요", 0.2f, MessageType.Commmon);

        yield return new WaitForSeconds(10f);

        GameRoomSceneManager.instance.UpdateUI();
        GameManager.instance.ShowMessage("GO!!", 0.7f, MessageType.Commmon);
        camera.SetClamp(camera.MinX, 60f, camera.MinY, camera.MinY);


    }

    private void Warnning(PlayerAttackEvent e)
    {
        if(e.ID != player.ID)
        {
            warnning.SetTrigger("Show");
            GameObject.Instantiate(fireBall, RemotePlayerController.instance.RemotePlayers[e.ID].FirePos.position, e.Direction == Vector2.right ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0));
        }


   
    }
    
    private void Goal(PlayerGoalEvent e)
    {
        
        if(RoomManager.instance.RoomInfo.HeadCount == 1)
        {
            GameOver(this.goaler);
            return;
        }

        if(e.PlayerID == player.ID)
        {
            if (goaler.Count == 0)
            {
                GameManager.instance.ShowMessage("1등!", 1f, MessageType.Important);
            }
            else
            {
                GameManager.instance.ShowMessage("골인!", 1f, MessageType.Important);

            }
            GameManager.instance.ShowMessage("당신은 관전자!!", 1.0f, MessageType.Commmon);


            player.Stat = PlayerStat.Spectator;
            PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.ALL, Packet.Type.SYNC_PLAYER_GOAL, GameManager.instance.Me));

        }
        else
        {
            if(goaler.Count == 0)
            StartCoroutine(CountDownRoutin());
        }

        goaler.Add(RoomManager.instance.RoomInfo.Personnel[e.PlayerID]);   
    }

    private IEnumerator CountDownRoutin()
    {
        GameManager.instance.ShowMessage("5초 안에 골인하세요!", 0.5f, MessageType.Important);
        GameManager.instance.ShowMessage("5", 0.3f, MessageType.Commmon);
        GameManager.instance.ShowMessage("4", 0.3f, MessageType.Commmon);
        GameManager.instance.ShowMessage("3", 0.3f, MessageType.Commmon);
        GameManager.instance.ShowMessage("2", 0.3f, MessageType.Commmon);
        GameManager.instance.ShowMessage("1", 0.3f, MessageType.Commmon);
        GameManager.instance.ShowMessage("초가 안맞는 것은 기분탓입니다 ^^", 0.5f, MessageType.Commmon);
        GameManager.instance.ShowMessage("끝!", 0.5f, MessageType.Commmon);
        yield return new WaitForSeconds(13);

        if (GameManager.instance.NetPart == SocketPart.Server)
            GameOver(this.goaler);
 
    }


    private void SavePlayerCheckPoint(PlayerSetCheckPointEvent e)
    {
        spawnPoint = e.CheckPoint;

        if (empCheckPoint != null)
        {
            Destroy(empCheckPoint);
        }

        empCheckPoint = e.Obj;
    }


    public void AddListener()
    {
        EventManager.AddListener<GameStartEvent>(StartGameSystem);
        EventManager.AddListener<PlayerDeathEvent>(PlayerDIe);
        EventManager.AddListener<PlayerGoalEvent>(Goal);
        EventManager.AddListener<PlayerSetCheckPointEvent>(SavePlayerCheckPoint);
        EventManager.AddListener<PlayerAttackEvent>(Warnning);
        EventManager.AddListener<GameEndEvent>(EndGameSystem);
        EventManager.AddListener<SurrendEvent>(PlayerSurrend);

    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<GameStartEvent>(StartGameSystem);
        EventManager.RemoveListener<PlayerDeathEvent>(PlayerDIe);
        EventManager.RemoveListener<PlayerGoalEvent>(Goal);
        EventManager.RemoveListener<PlayerSetCheckPointEvent>(SavePlayerCheckPoint);
        EventManager.RemoveListener<PlayerAttackEvent>(Warnning);
        EventManager.RemoveListener<GameEndEvent>(EndGameSystem);
        EventManager.RemoveListener<SurrendEvent>(PlayerSurrend);
    }

    public void Receive(Packet packet)
    {
        
            byte id;
            switch (packet.TypeCode)
            {
                case Packet.Type.SYNC_PLAYER_GOAL:
                    EventManager.CallEvent(new PlayerGoalEvent(new User(packet.Body).ID));
                    break;
                case Packet.Type.SYNC_PLAYER_ATTACK:
                    EventManager.CallEvent(new PlayerAttackEvent(packet.Body[0], packet.Body[1]));
                    break;
                case Packet.Type.GAMEOVER:
                    int count = packet.Body.Length;
                    byte[] goalerID = packet.Body;
                    List<User> goaler = new List<User>();
                    for (int i = 0; i < count; i++) { 
                        goaler.Add(RoomManager.instance.RoomInfo.Personnel[goalerID[i]]);
                    }
                    GameOver(goaler);
                    break;
                case Packet.Type.SYNC_PLAYER_DEAD:
                    id = packet.Sender;
                    RemotePlayerController.instance.RemotePlayers[id].Die();
                    break;
                case Packet.Type.SYNC_PLAYER_RESPAWN:
                    id = packet.Sender;
                    if(RemotePlayerController.instance.RemotePlayers[id].IsDying)
                    RemotePlayerController.instance.RemotePlayers[id].Respawn();
                    break;
                case Packet.Type.SYNC_SURREND:
                    id = packet.Body[0];
                EventManager.CallEvent(new SurrendEvent(id));
                    break;
                    
            }
        
      
        
            /*
            GameManager.instance.ShowMessage(UnityEngine.Random.Range(0,2) % 2 == 0 ?  "패킷이 잘못전달되었습니다.. 개발자 일안하냐ㅏ!!"
                : "패킷이 잘못전달되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);
            */    
        
    }

    public bool CheckResponsible(Packet packet)
    {
        byte type = packet.TypeCode;

        if (type == Packet.Type.SYNC_PLAYER_ATTACK || type == Packet.Type.SYNC_PLAYER_DEAD || type == Packet.Type.SYNC_PLAYER_RESPAWN || type == Packet.Type.SYNC_SURREND) return true;

        if(GameManager.instance.NetPart == SocketPart.Server)
        {
            if (type == Packet.Type.SYNC_PLAYER_GOAL) return true;

        }else if(GameManager.instance.NetPart == SocketPart.Client)
        {
            if (type == Packet.Type.SYNC_PLAYER_GOAL || type == Packet.Type.GAMEOVER)
                return true;
        }

        return false;
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
