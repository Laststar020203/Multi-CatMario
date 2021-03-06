﻿using System.Collections;
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
        firePos = modelTr.GetChild(0);

    }

    protected override void gv_UpdateStat(PlayerStat stat)
    {
        switch (stat)
        {
            case PlayerStat.Player:
                sr.enabled = true;
                sr.color = myColor;
                break;
            case PlayerStat.Spectator:
                sr.enabled = false;
                break;
        }
    }
    public float av_speed = 2.0f;
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
        animator.SetBool(IsDead, false);
        modelTr.localPosition = Vector2.zero;
        isDying = false;
    }

   
}
