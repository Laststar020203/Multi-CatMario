using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomViewer : MonoBehaviour
{

    private string myip;
    private Ping ping;

    private Text textIp;
    private Text textPingValue;

    private void Awake()
    {
        Transform t = GetComponent<Transform>();
        textIp = t.GetChild(0).GetComponent<Text>();
        textPingValue = t.GetChild(1).GetComponent<Text>();
    }
    public void startView(string ip)
    {
        this.myip = ip;
        this.textIp.text = ip;
        ping = new Ping(this.myip);
        
        StartCoroutine(View());
    }

    IEnumerator View()
    {
        while (true)
        {
            if (ping.isDone)
            {
                textPingValue.text = ping.time.ToString();
            }
            else
            {
                Destroy(this);
            }
        }
    }

    private void OnDisable()
    {
        StopCoroutine(View());
    }


}
