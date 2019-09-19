using System.Collections.Generic;
using UnityEngine;
public class PlayerDeathEvent : Event
{
    public static readonly List<GameEvent<PlayerDeathEvent>> listeners = new List<GameEvent<PlayerDeathEvent>>();

    private readonly Vector2 _deathPosition;

    public Vector2 DeathPosition { get { return _deathPosition; } }

    public PlayerDeathEvent(Vector2 deathPosition)
    {
        _deathPosition = deathPosition;
    }
}
