using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("PLAYER"))
        {
            EventManager.CallEvent(new PlayerGoalEvent(ClientGameSystem.instance.Player.ID));
        }
    }
}
