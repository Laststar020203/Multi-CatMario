using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{
    LocalPlayer player;
    MovePlayer Mplayer;

    public bool killTrig = false;
    public bool killColi = true;
    public bool jumper = false;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<LocalPlayer>();
        Mplayer = GameObject.Find("Player").GetComponent<MovePlayer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "PLAYER" && killColi)
        {
            player.Die();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player" && killTrig)
        {
            player.Die();
        }

        if (collision.gameObject.tag == "Player" && jumper)
        {
            Mplayer.isJumping = true ;
        }
        
    }


}
