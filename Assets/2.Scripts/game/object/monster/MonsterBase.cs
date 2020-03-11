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
    SpriteRenderer spriteRender;

    int sp = 1;
    public bool split = false;

    private void Start()
    {
        spriteRender = gameObject.GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<LocalPlayer>();
        Mplayer = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<MovePlayer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("PLAYER") && killColi)
        {
            player.Die();
        }
        if (collision.gameObject.tag == "Wall" && split)
        {
            if (sp == 1)
            {
                spriteRender.flipX = true;
                sp = 0;
            }
            else
            {
                spriteRender.flipX = false;
                sp = 1;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("PLAYER") && killTrig)
        {
            player.Die();
            Destroy(gameObject);
        }

        if (collision.CompareTag("PLAYER") && jumper)
        {
            Mplayer.isJumping = true;
            Destroy(gameObject);
        }
        
    }


}
