using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoalEvent : Event
{
    public static readonly List<GameEvent<PlayerGoalEvent>> listeners = new List<GameEvent<PlayerGoalEvent>>();

    private byte id;
    public byte PlayerID { get { return id; } }

    public PlayerGoalEvent(byte id)
    {
        this.id = id;
    }

}
