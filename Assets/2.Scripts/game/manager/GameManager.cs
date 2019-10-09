using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SocketPart { Server, Client};

public class GameManager : MonoBehaviour, IEventListener
{

    public static GameManager instance;
    private GameSetting setting;

    //게임룸 씬에서 생성이 될것이다.
    private User me;
    private SocketPart part;
    private SceneMaker[] sceneMakers;
    private Queue<GameMessage> messageQueue;

    public GameObject messageUi;

    public User Me { get { return me; } }
    public GameSetting Setting { get { return setting; } }
    public SocketPart Part { get { return part; } set { part = value; } }


    private void Awake()
    {

        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
            instance = this;
        

        Part = SocketPart.Client;

        sceneMakers = new SceneMaker[5];
        sceneMakers[0] = new StartSceneMaker();
        sceneMakers[1] = new EntranceSceneMaker();
        sceneMakers[2] = new WaitingRoomSceneMaker();
        sceneMakers[3] = new GameRoomSceneMaker();

        me = new User("sdfsdf", 3);
        messageQueue = new Queue<GameMessage>();

       

    }

    private void Start()
    {
        AddListener();
           DontDestroyOnLoad(instance.gameObject);
        //DontDestroyOnLoad(informUI);

    }

    

    public void LoadMyInfo()
    {
        string name = PlayerPrefs.GetString("Name");
        byte characterCode = (byte) PlayerPrefs.GetInt("CharacterCode");

        if (name != null)
            me = new User(name, characterCode);
        else
            me = new User("Believe", 0);
        
    }

    public void QuitScene(int index)
    {
        sceneMakers[index].Quit();
    }

    public void StartScene(int index)
    {
        sceneMakers[index].Start();
    }

    public void ShowMessage(string msg, float time)
    {
        GameObject o = GameObject.Instantiate(messageUi, Vector2.zero, Quaternion.identity);
        GameMessage message = o.GetComponent<GameMessage>();
        message.Init(msg, time);
         
        if(messageQueue.Count != 0)
        {
            messageQueue.Enqueue(message);
        }
        else
        {
            messageQueue.Enqueue(message);
            message.Show();
        }
    }


    private void QuitGameMessage(GameMessageQuitEvent e)
    {
        if (messageQueue.Peek() == e.GameMessage)
        {
            messageQueue.Dequeue();
        }

        if(messageQueue.Count != 0)
        {
            messageQueue.Peek().Show();
        }

    }

    public void NetExit()
    {
        LoadSceneManager.instance.SceneLoad(1);
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                EventManager.CallEvent(new EscapeEvent(SceneManager.GetActiveScene().buildIndex));
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                EventManager.CallEvent(new EscapeEvent(SceneManager.GetActiveScene().buildIndex));
            }
        }
    }

    public void AddListener()
    {
        EventManager.AddListener<GameMessageQuitEvent>(QuitGameMessage);
    }

    public void RemoveListener()
    {
        EventManager.RemoveListener<GameMessageQuitEvent>(QuitGameMessage);
    }

    private abstract class SceneMaker
    {
        public abstract int SceneIndex { get; }
        public abstract void Start();
        public abstract void Quit();
    }

    private class StartSceneMaker : SceneMaker
    {
        public override int SceneIndex => 0;

        public override void Quit()
        {

        }

        public override void Start()
        {
            GameObject.Find("StartGameButton").GetComponent<Button>().onClick.AddListener(() => { LoadSceneManager.instance.NextSceneLoad(); });
            GameObject.Find("OptionButton").GetComponent<Button>().onClick.AddListener(() => { LoadSceneManager.instance.SceneLoad(5); });



            Destroy(GameObject.Find("net"));
        }
    }
    
    private class EntranceSceneMaker : SceneMaker
    {

        private GameObject net;

        public override int SceneIndex => 1;

        public override void Quit()
        {
            
        }

        public override void Start()
        {

            GameManager.instance.Part = SocketPart.Client;
            //Net
            //GameObject net = GameObject.Find("net");

            GameObject gameCamera = GameObject.FindGameObjectWithTag("GAMECAMERA");
            if (gameCamera != null)
                Destroy(gameCamera);



            if (net == null)
            {
                net = new GameObject("net");
                DontDestroyOnLoad(net);
                Transform netTr = net.transform;

                CreateClient(netTr);

                //Create PacketManager
                GameObject packetManager = new GameObject("packetManager");
                packetManager.AddComponent<PacketManager>();
                packetManager.transform.SetParent(netTr);

            }
            else
            {

                GameObject netChild = net.transform.GetChild(1).gameObject;
                Transform netTr = net.transform;

                if(netChild.name == "server")
                {
                    Destroy(netChild);
                    CreateClient(netTr);
                }else
                {

                    Client client = netChild.GetComponent<Client>();
                    if(client.IsConnected)
                    client.CloseSocket();
                }
                

            }

            //Destroy
            GameObject[] gm = GameObject.FindGameObjectsWithTag("PLAYMANAGER");
            foreach(GameObject g in gm)
            {
                Destroy(g);
            }
            GameObject[] rplayers = GameObject.FindGameObjectsWithTag("REMOTEPLAYER");
            foreach (GameObject g in rplayers)
            {
                Destroy(g);
            }
            GameObject player = GameObject.FindGameObjectWithTag("PLAYER");
            if (player != null)
                Destroy(player);

        }

        private void CreateClient(Transform net)
        {
            GameObject client = new GameObject("client");
            client.AddComponent<Client>();

            client.transform.SetParent(net.transform);
        }
    }

    private class WaitingRoomSceneMaker : SceneMaker
    {
        GameObject clientReceiver;

        public override int SceneIndex => 2;
       
        public override void Quit()
        {
            if (clientReceiver != null)
            {
                Destroy(clientReceiver);
                clientReceiver = null;
            }

         
            WaitingRoomGameObjectsSetActive(false);
        }

        public override void Start()
        {
            if(GameManager.instance.part == SocketPart.Server && clientReceiver == null)
            {
                clientReceiver = new GameObject("clientReceiver");
                clientReceiver.AddComponent<NewClientReceiver>();
            }

            WaitingRoomGameObjectsSetActive(true);
        }

        private void WaitingRoomGameObjectsSetActive(bool b)
        {
            GameObject startButton = GameObject.Find("StartButton");
            if (GameManager.instance.Part == SocketPart.Server)
            {
                 startButton.SetActive(b);
            }
            else
            {
                startButton.SetActive(false);
            }

            Transform controllerTr = GameObject.FindGameObjectWithTag("CONTROLLER").GetComponent<Transform>();
            controllerTr.GetChild(3).gameObject.SetActive(!b);
            controllerTr.GetChild(4).gameObject.SetActive(!b);


            GameObject.Find("WaitingRoomSceneManager").SetActive(b);
        }
    }

    private class GameRoomSceneMaker : SceneMaker
    {
        public override int SceneIndex => 3;

        public override void Quit()
        {
            throw new System.NotImplementedException();
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }
    }




}
