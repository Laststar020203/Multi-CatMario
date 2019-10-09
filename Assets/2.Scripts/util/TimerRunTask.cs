using System.Collections;
using UnityEngine;
using System.Threading;
using System;

//ㅇ
public enum TaskType { Coroutine , Thread}

public class TimerRunTask : MonoBehaviour
{ 
    private float time;
    private Action r;
    private Action c;

    private Thread thread;
    private Thread modifier;

    private TaskType type;

    public TimerRunTask(float time, Action r, Action c, TaskType type)
    {
        this.time = time;
        this.r = r;
        this.c = c;
        this.type = type;
    }

    public void Run()
    {
        switch (type)
        {
            case TaskType.Coroutine:
                StartCoroutine(Coroutine_Run());
                break;
            case TaskType.Thread:

                thread = new Thread(() => Thread_Run());

                modifier = new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep((int)time * 1000);
                        thread.Abort();
                        if(c != null)
                        c();

                    }catch(ThreadInterruptedException e)
                    {
                        thread.Abort();
                    }

                });

                thread.Start();
                modifier.Start();

                break;
        }
    }

    private IEnumerator Coroutine_Run()
    {
        float nextTime = Time.time + time;
        while (Time.time <= nextTime)
        {
            if (r != null)
                r();
            yield return null;
        }
        if (c != null)
            c();
    }

    private void Thread_Run()
    {
        while (true)
        {
            if(r != null)
            r();
        }
    }

    public void Stop()
    {
        switch (type)
        {
            case TaskType.Coroutine:
                StopCoroutine(Coroutine_Run());
                break;
            case TaskType.Thread:
                modifier.Interrupt();
                break;
        }
    }

}
