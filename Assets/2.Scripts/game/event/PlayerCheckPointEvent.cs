using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetCheckPointEvent : Event
{

    public static readonly List<GameEvent<PlayerSetCheckPointEvent>> listeners = new List<GameEvent<PlayerSetCheckPointEvent>>();

    private readonly Vector2 _checkPoint;
    private GameObject checkPointObj;

   

    public Vector2 CheckPoint { get { return _checkPoint; } }
    public GameObject Obj { get { return checkPointObj; } }


    public PlayerSetCheckPointEvent(Vector2 checkPoint, GameObject checkPointObj)
    {
        this._checkPoint = checkPoint;
        this.checkPointObj = checkPointObj;
    }
}
