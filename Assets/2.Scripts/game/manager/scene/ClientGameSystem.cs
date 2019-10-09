using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;


public class ClientGameSystem : MonoBehaviour, IEventListener, IPacketDataReceiver
{
    public static ClientGameSystem instance;
    private LocalPlayer player;
    public float countDown = 10f;
    private int playerLifeCount;
    private bool isGameStart;

    public GameObject deadBoard;

    private CameraController camera;
    private bool troll;
    private Vector2 spawnPoint;
    private List<User> goaler;

    public LocalPlayer Player { get { return player; } }
    public bool GameStart { get { return isGameStart; } }
    public bool Troll { get { return troll; } set { troll = value; } }
    public int PlayerLife { get { return playerLifeCount; } }
    
    public Vector2 SpawnPoint { get { return spawnPoint; } }

    

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
    private void Start()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        deadBoard.SetActive(false);

        AddListener();
    }

    private void StartGameSystem(StartGameEvent e)
    {
        isGameStart = true;
        StartCoroutine(GameReadyRoutin());
        playerLifeCount = 3;


        if (GameManager.instance.Part == SocketPart.Server)
            goaler = new List<User>();
    }

    private void PlayerDIe(PlayerDeathEvent e)
    {
        --playerLifeCount;
        PlayerDeadRoutin();
    }

    private IEnumerator PlayerDeadRoutin()
    {
        deadBoard.SetActive(true);
        yield return new WaitForSeconds(2f);
        player.Respawn();
    }

    private IEnumerator GameReadyRoutin()
    {
        GameManager.instance.ShowMessage("이제 곧 게임이 시작됩니다!", 1f);
        GameManager.instance.ShowMessage("오른쪽에 출구가 생길겁니다 모두들 준비해주세요!!",1f);

        yield return new WaitForSeconds(8f);

        if (troll)
        {
            GameManager.instance.ShowMessage("몸통박치기 해도 벽은 부셔지지 않습니다 --", 1f);
            troll = false;
        }
        yield return new WaitForSeconds(3f);

        GameManager.instance.ShowMessage("GO!!", 0.7f);

    }

    private void Goal(PlayerGoalEvent e)
    {
        if (goaler.Count == 0)
        {
            if (e.PlayerID == player.ID)
            {
                GameManager.instance.ShowMessage("축하", 1f);
            }
            else
            {
                StartCoroutine(CountDownRoutin());
            }
        }

        if (GameManager.instance.Part == SocketPart.Client && e.PlayerID == player.ID)
            PacketManager.instance.PutPacket(new Packet(GameManager.instance.Me.ID, Packet.Target.SERVER, Packet.Type.SYNC_PLAYER_GOAL));

        if(GameManager.instance.Part == SocketPart.Server)
        goaler.Add(RoomManager.instance.RoomInfo.Personnel[e.PlayerID]);
       
    }

    private IEnumerator CountDownRoutin()
    {
        GameManager.instance.ShowMessage("5초 안에 골인하세요!", 0.5f);
        GameManager.instance.ShowMessage("5", 0.3f);
        GameManager.instance.ShowMessage("4", 0.3f);
        GameManager.instance.ShowMessage("3", 0.3f);
        GameManager.instance.ShowMessage("2", 0.3f);
        GameManager.instance.ShowMessage("1", 0.3f);
        GameManager.instance.ShowMessage("초가 안맞는 것은 기분탓입니다 ^^", 0.5f);
        GameManager.instance.ShowMessage("끝!", 0.5f);
        yield return new WaitForSeconds(13);

        if (GameManager.instance.Part == SocketPart.Server)
            GameOver();
 
    }

    private void GameOver()
    {

        if (GameManager.instance.Part == SocketPart.Server)
            PacketManager.instance.PutPacket(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.GAMEOVER));

    }


    private void SavePlayerCheckPoint(PlayerSetCheckPointEvent e)
    {
        spawnPoint = e.CheckPoint;
    }


    public void AddListener()
    {
        EventManager.AddListener<StartGameEvent>(StartGameSystem);
        EventManager.AddListener<PlayerSetCheckPointEvent>(SavePlayerCheckPoint);

    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<StartGameEvent>(StartGameSystem);
        EventManager.RemoveListener<PlayerSetCheckPointEvent>(SavePlayerCheckPoint);

    }

    public void Receive(Packet packet)
    {
        if(GameManager.instance.Part == SocketPart.Server)
        {
            //Goal Event 발생
        }
        else
        {

        }
    }

    public bool CheckResponsible(Packet packet)
    {
        byte type = packet.TypeCode;

        if(GameManager.instance.Part == SocketPart.Server)
        {
            if (type == Packet.Type.SYNC_PLAYER_GOAL) return true;

        }else if(GameManager.instance.Part == SocketPart.Client)
        {
            if (type == Packet.Type.SYNC_PLAYER_GOAL || type == Packet.Type.GAMEOVER)
                return true;
        }

        return false;
    }
}
