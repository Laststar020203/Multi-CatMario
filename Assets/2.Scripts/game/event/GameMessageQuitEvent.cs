using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMessageQuitEvent : Event
{
    public static readonly List<GameEvent<GameMessageQuitEvent>> listeners = new List<GameEvent<GameMessageQuitEvent>>();

    private GameMessage gameMessage;
    public GameMessage GameMessage { get { return gameMessage; } }

    public GameMessageQuitEvent(GameMessage message)
    {
        this.gameMessage = message;
    }

}
