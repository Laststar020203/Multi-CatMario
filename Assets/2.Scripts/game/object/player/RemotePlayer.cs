using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayer : Player
{

    private bool isJump;
    private bool isDying;

    public override bool IsDying => isDying;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        modelTr = tr.GetChild(0).GetComponent<Transform>();
    }

 

    public void SyncPosition(Vector2 pos)
    {


        if(tr.position.x < pos.x)
        {
            RightMove();
        }
        else if(tr.position.x > pos.x)
        {
            LeftMove();
        }
        else
        {
            animator.SetBool(IsWalking, false);
        }

        tr.position = pos;
    }

    protected override void LeftMove()
    {
        if (isJump || isDying) return;
        base.LeftMove();
    }

    protected override void RightMove()
    {
        if (isJump || isDying) return;
        base.RightMove();
    }

    public override void Die()
    {
        isDying = true;
        animator.SetBool(IsDead, true);
    }

    public override void Respawn()
    {
        isDying = false;
        animator.SetBool(IsDead, false);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("GROUND"))
        {
            isJump = false;
            animator.SetBool(IsJumping, false);
        }
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("GROUND"))
        {
            isJump = true;
            animator.SetBool(IsJumping, true);
        }
    }
}
