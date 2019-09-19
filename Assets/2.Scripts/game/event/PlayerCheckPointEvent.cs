using System.Collections.Generic;
using UnityEngine;

public class PlayerSetCheckPointEvent : Event
{

    public static readonly List<GameEvent<PlayerSetCheckPointEvent>> listeners = new List<GameEvent<PlayerSetCheckPointEvent>>();

    private readonly Vector2 _checkPoint;
    
    public Vector2 CheckPoint { get { return _checkPoint; } }

    public PlayerSetCheckPointEvent(Vector2 checkPoint)
    {
        _checkPoint = checkPoint;
    }
}
