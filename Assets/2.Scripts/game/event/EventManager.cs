using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;

public delegate void GameEvent<T>(T t) where T : Event;

public class EventManager : MonoBehaviour
{
    public static Queue<Action> eventListener;

    private void OnEnable()
    {
        eventListener = new Queue<Action>();
    }

    public static void AddListener<T>(GameEvent<T> dele)
          where T : Event
    {
        FieldInfo field = typeof(T).GetField("listeners");

        try
        {
            List<GameEvent<T>> list = (List<GameEvent<T>>)field.GetValue(null);
            list.Add(dele);
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning("이벤트 클래스에 핸들러가 들어가지 않았습니다.");
        }
    }

    public static void RemoveListener<T>(GameEvent<T> dele)
        where T : Event
    {
        FieldInfo field = typeof(T).GetField("listeners");

        try
        {
            List<GameEvent<T>> list = (List<GameEvent<T>>)field.GetValue(null);
            list.Remove(dele);
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning("이벤트 클래스에 핸들러가 들어가지 않았습니다.");
        }
    }


    public static void CallEvent<T>(T e)
        where T : Event
    {
        FieldInfo field = typeof(T).GetField("listeners");


        
        List<GameEvent<T>> list = (List<GameEvent<T>>)field.GetValue(null);

        foreach (GameEvent<T> listeners in list)
        {
            eventListener.Enqueue( () => listeners(e));
        }
    }

    private void Update()
    {
        if(eventListener.Count != 0)
        {
            eventListener.Dequeue()();
        }
    }
}
