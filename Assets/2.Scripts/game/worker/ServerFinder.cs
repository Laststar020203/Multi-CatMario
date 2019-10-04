using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;


public class ServerFinder : MonoBehaviour
{

    public float findDelay = 1f;
    private List<IPAddress> ipList;
    private IPAddress myIp;
    private byte[] byteIpAddress;

    public IPAddress MyIP { get { return myIp; } }

    private void Start()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach(var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                myIp = ip;
        }
        ipList = new List<IPAddress>();
        byteIpAddress = myIp.GetAddressBytes();

        StartCoroutine(FindServer());

    }

    
    private IEnumerator FindServer()
    {
        
        while (true)
        {

            Ping ping;
            for (byte i = 2; i < 255; i++)
            {
                byteIpAddress[3] = i;
                IPAddress newIP = new IPAddress(byteIpAddress);
                

                ping = new Ping(newIP.ToString());

                yield return new WaitForSeconds(0.1f);


                if (ping.time != -1)
                {
                    if (!ipList.Contains(newIP) && !newIP.Equals(myIp))
                    {
               
                        ipList.Add(newIP);
                        EntranceSceneManager.instance.AddRoomBox(newIP.ToString());
                    }
                }
                else
                {
                    if (ipList.Contains(newIP))
                    {
                        ipList.Remove(newIP);
                        EntranceSceneManager.instance.RemoveRoomBox(newIP.ToString());
                    }
                }


             }



        }
        
    } 










}
