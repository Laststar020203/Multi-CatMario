﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameMessage : MonoBehaviour
{
    private Animator animator;
    private Text text;
    private Transform tr;
    private float time;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<Text>();
    }
     
    public void Init(string text, float time)
    {
        this.text.text = text;
        this.time = time;
        this.text.enabled = false;
        tr.parent = GameObject.FindGameObjectWithTag("CANVAS").transform;
        tr.localPosition = Vector3.zero;

    }

    public void Show()
    {
        this.text.enabled = true;
        animator.SetBool("Start", true);
        StartCoroutine(Wait(time));

    }
    
    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("Close", true);
    }

    public void Close()
    {
        EventManager.CallEvent(new GameMessageQuitEvent(this));
        Destroy(this.gameObject);
    }


}
