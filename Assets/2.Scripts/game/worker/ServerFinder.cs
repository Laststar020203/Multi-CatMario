using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;


public class ServerFinder : MonoBehaviour
{
    //망할 유니티는 왜 스레드 지원이 안됩니까
    public struct SuccessDG
    {
        Action<string, Room> action;
        string ip;
        Room room;

        public SuccessDG(Action<string, Room> action, string ip, Room room)
        {
            this.action = action;
            this.ip = ip;
            this.room = room;
        }

        public void Invoke()
        {
            if(action != null)
            action(ip, room);
        }
    }

    private IPAddress myIp;
    private byte[] byteIpAddress;

    private List<IPAddress> emp_List;
    private Dictionary<string, ServerViewer> serverList;

    public float waitTime = 0.2f;

    public IPAddress MyIP { get { return myIp; } }
    private List<Thread> threads;

    Queue<Action> failDgQueue;
    Queue<SuccessDG> succesDgQueue;

    private void Start()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach(var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                myIp = ip;
        }
        emp_List = new List<IPAddress>();
        byteIpAddress = myIp.GetAddressBytes();


        serverList = EntranceSceneManager.instance.ServerList;
        threads = new List<Thread>();

        failDgQueue = new Queue<Action>();
        succesDgQueue = new Queue<SuccessDG>();

        StartCoroutine(Find());

    }

    private void Update()
    {

        if (EntranceSceneManager.instance.Joining && threads.Count != 0)
            AllThreadStop();

        if (succesDgQueue.Count != 0)
        {
           succesDgQueue.Dequeue().Invoke();
        }
        
        if(failDgQueue.Count != 0)
        {
            Action f = failDgQueue.Dequeue();
            if (f != null)
                f();
        }


    }

    public void Peek(string ip, Action<string, Room> success, Action fail)
    {
        Thread thread = null;
        thread = new Thread(() =>
        {
            TcpClient server = null;
            try
            {
                server = new TcpClient(ip, 5252);
                server.ReceiveTimeout = 1000;
                NetworkStream ns = server.GetStream();
                Packet packet;
                PacketParser.Pasing(ns, out packet);
                Room r =  new Room(packet.Body);

                /*
                if(success != null)
                success(ip, r);
                */

                if (success != null)
                {
                    SuccessDG successDG = new SuccessDG(success, ip, r);
                    succesDgQueue.Enqueue(successDG);
                }

                server.Close();
                return;

            }
            catch (Exception e)
            { 
                if(server != null)
                 server.Close();

                /*
                if (fail != null)
                fail();
                */

                failDgQueue.Enqueue(fail);

                Debug.Log("It's not Server... : " + ip + " Exception : " + e);
                if(thread != null)
                threads.Remove(thread);
                return;
            }
        });
        threads.Add(thread);
        thread.Start();
    }

    private IEnumerator Find()
    {
        
        while (true)
        {
            
            Ping ping;
            for (byte i = 2; i < 255; i++)
            {
                byteIpAddress[3] = i;
                IPAddress newIP = new IPAddress(byteIpAddress);

                if (serverList.ContainsKey(newIP.ToString())) continue;

                ping = new Ping(newIP.ToString());
                yield return new WaitForSeconds(waitTime);


                if (ping.time != -1)
                {
                    if (!serverList.ContainsKey(newIP.ToString()) && !emp_List.Contains(newIP) &&  !newIP.Equals(myIp))
                    {
                        Debug.Log(newIP + " server Peek Start....");
                        emp_List.Add(newIP);
                        Peek(newIP.ToString(), (ip, r) =>
                        {
                            EntranceSceneManager.instance.AddRoomBox(ip, r);
                        }, 
                        () =>
                        {
                            if (newIP != null && emp_List.Contains(newIP))
                                emp_List.Remove(newIP);
               
                        });
                    }
                }
 
             }



        }
        
    }

   private void AllThreadStop()
    {
        foreach (Thread thread in threads)
        {
            thread.Abort();
        }
    }

    private void OnDisable()
    {
        StopCoroutine(Find());
        AllThreadStop();
    }

    private void OnDestroy()
    {
        StopCoroutine(Find());
        AllThreadStop();
    }







}
