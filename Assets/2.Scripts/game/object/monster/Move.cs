using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 0;
    public int hor= 1;
    public int vir= 0;
    //0 = up, 1 = down, 2 = left, 3 = right
    float xMove, yMove;
    public bool itTurns = false;
    Rigidbody2D rigid2D;

    public bool itBeam;
    public float beamSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        //rigid2D = GameObject.Find("Turtle").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Moves();
        
    }

    void Moves()
    {
      //  Vector3 moveVect3 = Vector3.zero;

        if (vir == 1)
        {
            yMove = moveSpeed * Time.deltaTime;
            // moveVect3 = Vector3.up;
        }
        else if (vir == 2)
        {
            yMove = -moveSpeed * Time.deltaTime;
            // moveVect3 = Vector3.down;
        }
        else if (hor == 1)
        {
            xMove = -moveSpeed * Time.deltaTime;
            //moveVect3 = Vector3.left;
          
        }
        else if (hor == 2)
        {
            xMove = moveSpeed * Time.deltaTime;
              // moveVect3 = Vector3.right;
        }

        //transform.position += moveVect3 * MoveSpeed * Time.deltaTime;
        transform.Translate(new Vector3(xMove, yMove, 0));


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" && itTurns)
        {

            if (hor == 1)
            {
                hor = 2;
            }
            else if (hor == 2)
            {
                hor = 1;
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PLAYER" && itBeam)
        {
            moveSpeed = beamSpeed;
        }
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Change")
        {
            if (vir == 1)
            {
                vir = 0;
                hor = 1;
            }
        }
    }

    */

}
