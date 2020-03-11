using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using UnityEngine.SceneManagement;
using System;

public class EntranceSceneManager : MonoBehaviour, IEventListener
{
    public static EntranceSceneManager instance;

    private ServerFinder finder;

    public Text myIpInfo;

    public GameObject roomListPanel;
    public GameObject roomPrefab;

    public GameObject createRoomPanel;
    public GameObject registerServerPanel;

    private Dictionary<string, ServerViewer> roomList;

    private Client client;

    private bool connectTrying;

    public Client Client { get { return client; }}
    public Dictionary<string, ServerViewer> ServerList { get { return roomList; } }
    public bool Joining { get { return connectTrying; } }



    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        
       finder = GameObject.Find("ServerFinder").GetComponent<ServerFinder>();
        roomList = new Dictionary<string, ServerViewer>();

        
    }

    private void Start()
    {
        GameManager.instance.StartScene(SceneManager.GetActiveScene().buildIndex);


        createRoomPanel.SetActive(false);
        registerServerPanel.SetActive(false);

        myIpInfo.text = "My IP : " + finder.MyIP.ToString();


        client = GameObject.Find("net").GetComponentInChildren<Client>();

        AddListener();
    }
    bool creatingServer = false;

    public void CreateRoom()
    {

        if (creatingServer) return;
        creatingServer = true;

        Transform t = createRoomPanel.transform;
        string roomTitle = t.GetChild(0).GetComponent<InputField>().text;
        int maxUser = Convert.ToInt32(t.GetChild(1).GetComponent<InputField>().text);

        GameObject serverObj = new GameObject("server");
        serverObj.transform.SetParent(GameObject.Find("net").transform);

        Server server = serverObj.AddComponent<Server>();

        if (server.ServerStart(5252))
        {
            Destroy(GameObject.Find("client"));
            GameManager.instance.NetPart = SocketPart.Server;

            GameManager.instance.Me.ID = Packet.Target.SERVER;

            CreateRoomManager(new Room(roomTitle, 0, (byte)maxUser));
        }
        else
        {
            Destroy(serverObj);
            creatingServer = false;
            return;
        }

        LoadSceneManager.instance.NextSceneLoad();

        creatingServer = false;

    }
    

    public void AddRoomBox(string ip, Room room)
    {
        string title = room.Title;
        int headCount = room.HeadCount;
        int maxUser = room.MaxUser;

        GameObject listBox = Instantiate(roomPrefab, Vector2.zero, Quaternion.identity);
        listBox.name = ip;
        listBox.transform.SetParent(roomListPanel.transform);
        listBox.transform.localScale = new Vector3(1, 1, 1);
        ServerViewer roomViewer = listBox.GetComponent<ServerViewer>();
        roomViewer.Keep(ip, room, 15f);

        roomList.Add(ip, roomViewer);

        /* RoomViewer에서 처리
         * 
        Transform listBoxTr = listBox.transform;

        Text titleText = listBoxTr.GetChild(0).GetComponent<Text>();
        Text UserStatText = listBoxTr.GetChild(1).GetComponent<Text>();

        titleText.text = title;
        UserStatText.text = headCount.ToString() + "/" + maxUser.ToString();
        */

        Button button = listBox.GetComponent<Button>();
        button.onClick.AddListener(() => ClickRoomBox(listBox.name));
       
    }

    public void AddServer(Text text)
    {
        if (Joining) return;

        finder.Peek(text.text, AddRoomBox, null);
        registerServerPanel.SetActive(false);
    }

    public void RemoveRoomBox(string ip) {

        if (roomList.ContainsKey(ip))
        {
            ServerViewer viewer = roomList[ip];
            Destroy(viewer.gameObject);
            roomList.Remove(ip);
        }

        /*
        GameObject removeListBox = listBoxes.Find(x => x.name.Equals(ip));
        if(removeListBox != null)
        {
            listBoxes.Remove(removeListBox);
            Destroy(removeListBox);
        }
        */

    }

    public void ClickRoomBox(string ip)
    {
        if (connectTrying) return;
        connectTrying = true;

        GameObject emp_serverJoiner = new GameObject("emp_serverJoiner");

        ServerJoiner serverJoiner = emp_serverJoiner.AddComponent<ServerJoiner>();
        emp_serverJoiner.transform.SetParent(GameObject.Find("net").transform);

        GameManager.instance.ShowMessage(ip + " 접속 시도!", 1.0f, MessageType.Commmon);
        
        serverJoiner.Join(ip, 5f, () => {

            GameManager.instance.ShowMessage(ip + " 로 시도했으나 실패했습니다", 1.0f, MessageType.Important);
            connectTrying = false;
        });
    }

    public void CreateRoomManager(Room room)
    {
        GameObject roomManagerObj = new GameObject("roomManager");
        roomManagerObj.tag = "PLAYMANAGER";
        RoomManager roomManager = roomManagerObj.AddComponent<RoomManager>();
        roomManager.RoomInfo = room;
        DontDestroyOnLoad(roomManagerObj);

    }



    private void Escape(EscapeEvent e)
    {
        if (e.CurrentSceneIndex != SceneManager.GetActiveScene().buildIndex) return;

        LoadSceneManager.instance.PreviousSceneLoad();
       
    }

    private void OnDisable()
    {
        RemoveListener();
    }

    private void OnDestroy()
    {
        RemoveListener();
    }

    public void AddListener()
    {
        EventManager.AddListener<EscapeEvent>(Escape);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<EscapeEvent>(Escape);

    }
}
