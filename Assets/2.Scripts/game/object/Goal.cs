using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("PLAYER"))
        {
            EventManager.CallEvent(new PlayerGoalEvent(ClientGameSystem.instance.Player.ID));
        }
    }
}
