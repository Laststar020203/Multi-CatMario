using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ServerViewer : MonoBehaviour
{
 
    private string myip;
    private Ping ping;
    private Text titleText;
    private Text userStatText;
    private Room room;
    private ServerFinder finder;
    //private Action<string> remove;

    private WaitForSeconds ws;
    private Image myImage;

    public Sprite[] statSprite;
    //0 online 1 full 2 exit
    private bool canConnect;

    public bool CanConnect { get { return canConnect; } }


    private void Awake()
    {
        Transform t = GetComponent<Transform>();
        titleText = t.GetChild(0).GetComponent<Text>();
        userStatText = t.GetChild(1).GetComponent<Text>();

        finder = GameObject.Find("ServerFinder").GetComponent<ServerFinder>();
        myImage = GetComponent<Image>();
        
    }
    public void Keep(string ip, Room room, float updateDelay)
    {
        
        this.myip = ip;
        this.room = room;

        this.ws = new WaitForSeconds(updateDelay);

        ping = new Ping(this.myip);

        UpdateRoom(ip, room);

        //this.remove = remove;


        StartCoroutine(View());
    }



    private void UpdateRoom(string ip, Room room)
    {

        titleText.text = room.Title;
        userStatText.text = room.HeadCount.ToString() + "/" + room.MaxUser.ToString();

        if(room.HeadCount == room.MaxUser)
        {
            this.canConnect = false;
            myImage.sprite = statSprite[1];
        }
        else
        {
            this.canConnect = true;
            myImage.sprite = statSprite[0];
        }
    }

    IEnumerator View()
    {
        while (true)
        {
            yield return ws;
            if (EntranceSceneManager.instance.Joining) continue;

            finder.Peek(this.myip, UpdateRoom, () =>
            {
                canConnect = false;
                myImage.sprite = statSprite[2];

            });
        }
    }


    private void OnDisable()
    {
        StopCoroutine(View());
    }

    private void OnDestroy()
    {
        StopCoroutine(View());
    }


}
