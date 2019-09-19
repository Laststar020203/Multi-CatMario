using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    Player player;

    // Start is called before the first frame update
    void Start()
    {

         player = GameObject.Find("Player").GetComponent<Player>();

    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Monster")
        {
           player.Die();
        }


    }

}
