using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnEvent : Event
{
    public static readonly List<GameEvent<PlayerRespawnEvent>> listeners = new List<GameEvent<PlayerRespawnEvent>>();
    
}
