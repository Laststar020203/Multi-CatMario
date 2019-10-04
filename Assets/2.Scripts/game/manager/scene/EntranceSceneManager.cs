using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using UnityEngine.SceneManagement;


public class EntranceSceneManager : MonoBehaviour
{
    public static EntranceSceneManager instance;

    private ServerFinder finder;

    public GameObject roomList;
    public Text myIpInfo;

    public GameObject roomPrefab;

    public GameObject createRoomPanel;

    private List<GameObject> listBoxes;
    private Client client;

    private bool connectTrying;


    public Client Client { get { return client; }}


    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;

        //manager라는 오브젝트에 EntranceSceneManager와 ServerFinder가 붙어있을것이다.
        finder = GameObject.Find("ServerFinder").GetComponent<ServerFinder>();
        listBoxes = new List<GameObject>();

        GameManager.instance.StartScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        createRoomPanel.SetActive(false);
        myIpInfo.text = "My IP : " + finder.MyIP.ToString();


        client = GameObject.Find("net").GetComponentInChildren<Client>();
    }
    bool creatingServer = false;

    public void CreateRoom()
    {

        if (creatingServer) return;
        creatingServer = true;

        Transform t = createRoomPanel.transform;
        string roomTitle = t.GetChild(0).GetComponent<InputField>().text;

        GameObject serverObj = new GameObject("server");
        serverObj.transform.SetParent(GameObject.Find("net").transform);

        Server server = serverObj.AddComponent<Server>();

        if (server.ServerStart(5252))
        {
            Destroy(GameObject.Find("client"));
            GameManager.instance.Part = SocketPart.Server;

            CreateRoomManager(new Room(roomTitle, 1, 3));
        }
        else
        {
            Destroy(serverObj);
        }
        creatingServer = false;

    }
    
    public void AddRoomBox(string ip)
    {
        GameObject listBox = Instantiate(roomPrefab, Vector2.zero, Quaternion.identity);
        listBox.name = ip;
        listBox.transform.SetParent(roomList.transform);
        listBoxes.Add(listBox);

        Button button = listBox.GetComponent<Button>();
        button.onClick.AddListener(() => ClickRoomBox(listBox.name));
    }

    public void RemoveRoomBox(string ip) {

        GameObject removeListBox = listBoxes.Find(x => x.name.Equals(ip));
        if(removeListBox != null)
        {
            listBoxes.Remove(removeListBox);
            Destroy(removeListBox);
        }

    }

    public void ClickRoomBox(string ip)
    {
        if (connectTrying) return;
        connectTrying = true;

        GameObject emp_serverJoiner = new GameObject("emp_serverJoiner");

        ServerJoiner serverJoiner = emp_serverJoiner.AddComponent<ServerJoiner>();
        emp_serverJoiner.transform.SetParent(GameObject.Find("net").transform);

        

        serverJoiner.Join(ip, 3.0f, () => {

            Debug.Log("Server connect Fali");
            connectTrying = false;
        });
    }

    private void CreateRoomManager(Room room)
    {
        GameObject roomManagerObj = new GameObject("roomManager");
        RoomManager roomManager = roomManagerObj.AddComponent<RoomManager>();
        roomManager.RoomInfo = room;
        DontDestroyOnLoad(roomManagerObj);

    }

}
