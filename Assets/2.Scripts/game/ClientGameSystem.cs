using System.Collections;
using UnityEngine;
using System;

public class ClientGameSystem : MonoBehaviour
{
    public static ClientGameSystem instance;
    public Player player;

    public float countDown = 10f;

    private bool isGameOver;
    private bool isGoal;

    private int playerLifeCount;

    public Vector2 spawnPoint;

    // Start is called before the first frame update
    private void Start()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
        DontDestroyOnLoad(instance);

        Setting();

        EventManager.AddListener<PlayerSetCheckPointEvent>(SavePlayerCheckPoint);
    }
    private void Update()
    {
        
    }

    private void Setting()
    {
        try
        {
            player = GameObject.Find("LOCALPLAYER").GetComponent<Player>();
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("Not Player!");
        }

        isGoal = false;
        isGameOver = false;
    }

    public void CountDown()
    {
        StartCoroutine(CountDownControl());
    }

    private IEnumerator CountDownControl()
    {
        yield return new WaitForSeconds(countDown);

        if (!isGoal)
        GameOver();
    }

    private void Goal()
    {
        isGoal = true;
    }

    private void GameOver()
    {
        isGameOver = true;
    }

    public void PlayerDIe(PlayerDeathEvent e)
    {
        --playerLifeCount;
        //... UI 처리

        PlayerRespown();
    }

    private void SavePlayerCheckPoint(PlayerSetCheckPointEvent e)
    {
        spawnPoint = e.CheckPoint;
    }

    private void PlayerRespown()
    {
        EventManager.CallEvent(new PlayerRespawnEvent());
        //.. UI 처리

        player.transform.position = spawnPoint;
    }





}
