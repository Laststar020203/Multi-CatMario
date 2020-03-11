using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SocketPart { Server, Client};
public enum MessageType { Commmon, Important};

public enum GameStat { Find ,Wait, Game }
public class GameManager : MonoBehaviour, IEventListener
{

    public static GameManager instance;
    private GameSetting setting;

    //게임룸 씬에서 생성이 될것이다.
    private User me;
    private SocketPart part;
    private GameStat stat;
    private SceneMaker[] sceneMakers;
    private Queue<GameMessage> messageQueue;

    public GameObject messageUi;
    

    public User Me { get { return me; } }
    public GameSetting Setting { get { return setting; } }
    public SocketPart NetPart { get { return part; } set { part = value; } }
    public GameStat GameStat { get { return stat; } set { stat = value; } }



    private void Awake()
    {

        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
            instance = this;
        

        NetPart = SocketPart.Client;

        sceneMakers = new SceneMaker[6];
        sceneMakers[0] = new StartSceneMaker();
        sceneMakers[1] = new EntranceSceneMaker();
        sceneMakers[2] = new WaitingRoomSceneMaker();
        sceneMakers[3] = new GameRoomSceneMaker();
        sceneMakers[5] = new GameSettingSceneMaker();
        messageQueue = new Queue<GameMessage>();

       

    }

    private void Start()
    {
        AddListener();
           DontDestroyOnLoad(instance.gameObject);
        //DontDestroyOnLoad(informUI);
        LoadSetting();

    }



    public void LoadSetting()
    {
        string name = PlayerPrefs.GetString("Name");
        byte characterCode = (byte) PlayerPrefs.GetInt("CharacterCode");
        float sound = PlayerPrefs.GetFloat("Sound");

        setting = new GameSetting();


        if (name != null)
        {
            setting.Name = name;
            setting.CharacterCode = characterCode;
            setting.SoundValue = sound;
            me = new User(name, characterCode);
        }
        else
        {
            setting.Name = "Player";
            setting.CharacterCode = 0;
            setting.SoundValue = 1;
            me = new User("Player", 0);

        }

    }

    public void QuitScene(int index)
    {
        sceneMakers[index].Quit();
    }

    public void StartScene(int index)
    {
        sceneMakers[index].Start();
    }

    public void ShowMessage(string msg, float time, MessageType t)
    {

        GameObject o = GameObject.Instantiate(messageUi, Vector2.zero, Quaternion.identity);
        GameMessage message = o.GetComponent<GameMessage>();
        message.Init(msg, time);
         
        if(messageQueue.Count != 0)
        {
            //messageQueue.Enqueue(message);
            switch (t)
            {
                case MessageType.Commmon:
                    messageQueue.Enqueue(message);
                    break;
                case MessageType.Important:
                    while(messageQueue.Count != 0)
                    {
                        GameMessage gm = messageQueue.Dequeue();
                        if(gm != null)
                        Destroy(gm.gameObject);
                    }

                    message.Show();
                    messageQueue.Enqueue(message);
                    break;
            }
        }
        else
        {
            message.Show();
            messageQueue.Enqueue(message);
        }
    }


    private void QuitGameMessage(GameMessageQuitEvent e)
    {
        

        if (messageQueue.Count != 0 && messageQueue.Peek() == e.GameMessage)
        {
            messageQueue.Dequeue();
        }

        if(messageQueue.Count != 0)
        {
            messageQueue.Peek().Show();
        }

    }

    public void MessageClear()
    {
        if (messageQueue == null) return;
        while (messageQueue.Count != 0)
        {
            GameMessage gm = messageQueue.Dequeue();
            if (gm != null)
                Destroy(gm.gameObject);
        }
    }

    public void NetEscape()
    {
        GameManager.instance.ShowMessage(UnityEngine.Random.Range(0, 2) % 2 == 0 ? "심각한 오류로 네트워크가 종료되었습니다. 개발자 일안하냐ㅏ!!"
                : "심각한 오류로 네트워크가 종료되었습니다. 버그 제보는 저희에게 큰 힘이 됩니다 카톡 (010-4187-7834) ", 1.0f, MessageType.Important);
        LoadSceneManager.instance.SceneLoad(1);
        PacketManager.instance.Clear();
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

    private void OnDestroy()
    {
        MessageClear();

    }

    private void OnDisable()
    {
        MessageClear();
    }

    private void OnApplicationQuit()
    {
        while (messageQueue.Count != 0)
        {
            GameMessage gm = messageQueue.Dequeue();
            if (gm != null)
                Destroy(gm.gameObject);
        }
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
            Destroy(GameObject.Find("Bgm"));

            if (PacketManager.instance != null)
            PacketManager.instance.Clear();

            GameManager.instance.NetPart = SocketPart.Client;
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
                    if(client != null && client.IsConnected)
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

        public override int SceneIndex => 2;
       
        public override void Quit()
        {

         
        }

        public override void Start()
        {


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


    private class GameSettingSceneMaker : SceneMaker
    {
        public override int SceneIndex => throw new System.NotImplementedException();

        private GameObject optionButton;
        private GameObject startButton;


        public override void Quit()
        {
            startButton.SetActive(true);
            optionButton.SetActive(true);
        }

        public override void Start()
        {
            optionButton = GameObject.Find("OptionButton");
            startButton = GameObject.Find("StartGameButton");

            startButton.SetActive(false);
            optionButton.SetActive(false);
        }
    }



}
