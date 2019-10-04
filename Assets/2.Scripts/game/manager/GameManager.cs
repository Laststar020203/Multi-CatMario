using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SocketPart { Server, Client};

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private GameSetting setting;

    //게임룸 씬에서 생성이 될것이다.
    private User me;
    private SocketPart part;
    private SceneMaker[] sceneMakers;

    public User Me { get { return me; } }
    public GameSetting Setting { get { return setting; } }
    public SocketPart Part { get { return part; } set { part = value; } }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
        DontDestroyOnLoad(instance);

        Part = SocketPart.Client;

        sceneMakers = new SceneMaker[3];
        sceneMakers[0] = new EntranceSceneMaker();
        sceneMakers[1] = new WaitingRoomSceneMaker();
        sceneMakers[2] = new GameRoomSceneMaker();


    }

    private void Start()
    {
      
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

    public void StartScene(int index)
    {
        sceneMakers[index - 1].Start();
    }


    private void Update()
    {
        
    }

    private abstract class SceneMaker
    {
        public abstract int SceneIndex { get; }
        public abstract void Start();
        public abstract void Quit();
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
                GameObject netChild = net.transform.GetChild(0).gameObject;
                Transform netTr = net.transform;

                if(netChild.name == "server")
                {
                    Destroy(netChild);
                    CreateClient(netTr);
                }

            }
        }

        private void CreateClient(Transform net)
        {
            GameObject client = new GameObject("client");
            client.AddComponent<Client>();

            client.transform.SetParent(net.transform);
        }

        /*
        private void CreatePacketManager(Transform net)
        {
            GameObject packetManager = Instantiate(new GameObject("packetManager"), Vector2.zero, Quaternion.identity);
            packetManager.AddComponent<PacketManager>();
            packetManager.transform.SetParent(net);
        }
        */
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
        }

        public override void Start()
        {
            if(GameManager.instance.part == SocketPart.Server && clientReceiver == null)
            {
                clientReceiver = new GameObject("clientReceiver");
                clientReceiver.AddComponent<ClientReceiver>();
            }
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
