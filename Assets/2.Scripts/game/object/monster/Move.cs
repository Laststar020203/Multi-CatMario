using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonsterBase
{
    public float MoveSpeed = 0;
    public int way = 2;
    //0 = up, 1 = down, 2 = left, 3 = right
    float xMove, yMove;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Moves();
    }

    void Moves()
    {
      //  Vector3 moveVect3 = Vector3.zero;

        if (way == 0)
        {
            yMove = MoveSpeed * Time.deltaTime;
            // moveVect3 = Vector3.up;
        }
        else if (way == 1)
        {
            yMove = -MoveSpeed * Time.deltaTime;
            // moveVect3 = Vector3.down;
        }
        else if (way == 2)
        {
            xMove = -MoveSpeed * Time.deltaTime;
            //moveVect3 = Vector3.left;
          
        }
        else if (way == 3)
        {
            xMove = MoveSpeed * Time.deltaTime;
              // moveVect3 = Vector3.right;
        }

        //transform.position += moveVect3 * MoveSpeed * Time.deltaTime;
        transform.Translate(new Vector3(xMove, yMove, 0));
    }
}
