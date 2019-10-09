using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChangeEvent : Event
{
    public static readonly List<GameEvent<MapChangeEvent>> listeners = new List<GameEvent<MapChangeEvent>>();

    private int mapCode;
    public int MapCode { get { return mapCode; } }

    public MapChangeEvent(int mapCode)
    {
        this.mapCode = mapCode;
    }


}
