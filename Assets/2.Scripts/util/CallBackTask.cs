using System.Collections;
using UnityEngine;

public delegate void Action();

public class CallBackTask : MonoBehaviour
{

    private float time;

    private Action r;
    private Action c;

    public CallBackTask(float time, Action r, Action c)
    {
        this.time = time;
        this.r = r;
        this.c = c;
    }

    public void Run()
    {
        StartCoroutine(Wait());
    }

    private  IEnumerator Wait()
    {
        float nextTime = Time.time + time;
        while(Time.time <= nextTime)
        {
            if(r != null)
            r();
            yield return null;
        }
        if(c != null)
        c();
    }

    public void Stop()
    {
        StopCoroutine(Wait());
    }

}
