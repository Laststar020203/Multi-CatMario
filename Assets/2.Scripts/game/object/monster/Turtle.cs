using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour
{
    MonsterBase mon;
    Move move;
    MovePlayer player;
    int sp = 1;
    
    public enum State
    {
        NOR,
        SHELL,
        MSHELL
    }

    public State state = State.NOR;

    public Sprite shell;
    SpriteRenderer spriteRender;

    // 1 = 평범한 거북이 상태
    // 2 = 정지된 등껍질 상태
    // 3 = 움직이는 등껍질 상태


    // Start is called before the first frame update
    void Start()
    {
        spriteRender = gameObject.GetComponent<SpriteRenderer>();


        player = GameObject.Find("Player").GetComponent<MovePlayer>();
        move = GameObject.Find("Turtle").GetComponent<Move>();

        mon = GameObject.Find("Turtle").GetComponent<MonsterBase>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PLAYER")
        {

            if (state == State.NOR)
            {
                spriteRender.sprite = shell;
                player.isJumping = true;
                move.moveSpeed = 0;
                // mon.killColi = false;
                state = State.SHELL;
            }
            else if (state == State.SHELL)
            {
                player.isJumping = true;
                move.moveSpeed = 6;
                // mon.killColi = true;
                state = State.MSHELL;
            }
            else
            {
                player.isJumping = true;
                move.moveSpeed = 0;
                state = State.SHELL;
            }

        }
    

        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && state == State.MSHELL)
        {
            Destroy(collision.gameObject);
        }


        if (collision.gameObject.tag == "Wall")
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




}
