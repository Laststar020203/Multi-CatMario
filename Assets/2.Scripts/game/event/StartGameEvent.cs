using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameEvent : Event
{
    public static readonly List<GameEvent<StartGameEvent>> listeners = new List<GameEvent<StartGameEvent>>();


    public StartGameEvent()
    {
 
    }

}
