using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawnEvent : Event
{
    public static readonly List<GameEvent<PlayerRespawnEvent>> listeners = new List<GameEvent<PlayerRespawnEvent>>();
    
}

public class PlayerAttackEvent : Event
{
    public static readonly List<GameEvent<PlayerAttackEvent>> listeners = new List<GameEvent<PlayerAttackEvent>>();

    private byte id;
    private Vector2 direction;
    public byte ID { get { return id; } }
    public Vector2 Direction { get { return direction; } }

    public PlayerAttackEvent(byte id)
    {
        this.id = id;
    }
    public PlayerAttackEvent(byte id, byte direction)
    {
        this.id = id;
        switch (direction)
        {
            case 1:
                this.direction = Vector2.right;
                break;
            case 0:
                this.direction = -Vector2.right;
                break;
        }
    }


}

