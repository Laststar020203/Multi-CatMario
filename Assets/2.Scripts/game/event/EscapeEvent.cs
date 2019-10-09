using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeEvent : Event
{
    public static readonly List<GameEvent<EscapeEvent>> listeners = new List<GameEvent<EscapeEvent>>();

    private int currentSceneIndex;
    public int CurrentSceneIndex { get { return currentSceneIndex; } }

    public EscapeEvent(int currentSceneIndex)
    {
        this.currentSceneIndex = currentSceneIndex;
    }


}
