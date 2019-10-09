using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingRoomSceneManager : MonoBehaviour, IEventListener, IPacketDataReceiver
{


    public GameObject areYouSure;

    private int[] mapList;
    private int largerIndex;

    public static WaitingRoomSceneManager instance;
    public Dictionary<byte, int> voters;

    public SpriteRenderer[] voteBoxsR;

    private bool areYouSureOpen;

    private void Awake()
    {
        mapList = new int[voteBoxsR.Length];
        largerIndex = 0;
    }

    void Start()
    {

        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;


        GameManager.instance.StartScene(SceneManager.GetActiveScene().buildIndex);


        //Close Ui

        areYouSure.GetComponentInChildren<Button>().onClick.AddListener(LoadSceneManager.instance.PreviousSceneLoad);
        areYouSure.SetActive(false);
        areYouSureOpen = false;



        GameObject player = ClientGameSystem.instance.Player.gameObject;
        User myInfo = GameManager.instance.Me;
        ClientGameSystem.instance.Player.Init(myInfo.ID, myInfo.Name, myInfo.CharacteID);

        if (GameManager.instance.Part == SocketPart.Server)
        {
            voters = new Dictionary<byte, int>();
            foreach (byte id in RoomManager.instance.RoomInfo.Personnel.Keys)
            {
                voters.Add(id, 0);
            }
        }

        //ADD
        AddListener();
        PacketManager.instance.AddPacketDataReceiver(this);

        
    }
    
    private void ChoiceBox(int index)
    {
        for(int i = 0; i < voteBoxsR.Length; i++)
        {
            if (i == index)
                voteBoxsR[i].color = new Color(255, 0, 0);
            else
                voteBoxsR[i].color = new Color(255, 255, 255);
        }


    }

    private void AddVoter(ClientJoinEvent e)
    {
        voters.Add(e.User.ID, 0);
    }

    private void RemoveVoter(ClientQuitEvent e) {

        voters.Remove(e.User.ID);
    }


    public void Vote(byte ID, int mapIndex)
    {

        if (ID == GameManager.instance.Me.ID)
            ChoiceBox(mapIndex);

        if (GameManager.instance.Part != SocketPart.Server) return;

        if (voters[ID] == mapIndex)
        {
            return;
        }

        mapList[voters[ID]]--;
        voters[ID] = mapIndex;
        mapList[mapIndex]++;



        UpdateLarger();
    }


    public void StartButton()
    {
        if (GameManager.instance.Part == SocketPart.Server)
            PacketManager.instance.PutPacket(new Packet(Packet.Target.SERVER, Packet.Target.ALL, Packet.Type.GAMESTART, RoomManager.instance.RoomInfo));

        StartGame();
    }

    private void UpdateLarger()
    {

        int newLargerIndex = 0;
        int max = -1;

        for(int i = 0; i < mapList.Length; i++)
        {
            if (max < mapList[i])
            {
                max = mapList[i];
                newLargerIndex = i;
            }else if(max == mapList[i])
            {
                int r = Random.Range(0, 2);
                newLargerIndex = r % 2 == 0 ? i : newLargerIndex;
            }
        }

        if (newLargerIndex != this.largerIndex)
        {
            //RoomManager.instance.UpdateMap(newLargerIndex);
            EventManager.CallEvent(new MapChangeEvent(newLargerIndex));
            this.largerIndex = newLargerIndex;
        }
    }

    private void Escape(EscapeEvent e)
    {
        if (e.CurrentSceneIndex != SceneManager.GetActiveScene().buildIndex) return;

        if (!areYouSureOpen) areYouSure.SetActive(true);
        else areYouSure.SetActive(false);

        areYouSureOpen = !areYouSureOpen;
    }

    private void StartGame()
    {
        EventManager.CallEvent(new StartGameEvent());
        LoadSceneManager.instance.GameSceneLoad(3);
    }

    private void OnDestroy()
    {
        RemoveListener();
        PacketManager.instance.RemovePacketDataReceiver(this);
    }

    private void OnDisable()
    {
        RemoveListener();
        PacketManager.instance.RemovePacketDataReceiver(this);

    }
    public void AddListener()
    {
        EventManager.AddListener<EscapeEvent>(Escape);
        EventManager.AddListener<ClientJoinEvent>(AddVoter);
        EventManager.AddListener<ClientQuitEvent>(RemoveVoter);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<ClientJoinEvent>(AddVoter);
        EventManager.RemoveListener<EscapeEvent>(Escape);
        EventManager.RemoveListener<ClientQuitEvent>(RemoveVoter);
    }

    public void Receive(Packet packet)
    {
        Room serverRoom = new Room(packet.Body);
        Room myRoom = RoomManager.instance.RoomInfo;

        if (myRoom.Equals(serverRoom))
            StartGame();
        else
            GameManager.instance.NetExit();


    }

    public bool CheckResponsible(Packet packet)
    {
        if (packet.TypeCode == Packet.Type.GAMESTART && GameManager.instance.Part == SocketPart.Client) return true;
        else return false;
    }
}
