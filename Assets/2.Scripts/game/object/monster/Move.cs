using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonsterBase
{
    public float MoveSpeed = 0;
    public int hor= 1;
    public int vir= 0;
    //0 = up, 1 = down, 2 = left, 3 = right
    float xMove, yMove;
    public bool itTurns;

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

        if (vir == 1)
        {
            yMove = MoveSpeed * Time.deltaTime;
            // moveVect3 = Vector3.up;
        }
        else if (vir == 2)
        {
            yMove = -MoveSpeed * Time.deltaTime;
            // moveVect3 = Vector3.down;
        }
        else if (hor == 1)
        {
            xMove = -MoveSpeed * Time.deltaTime;
            //moveVect3 = Vector3.left;
          
        }
        else if (hor == 2)
        {
            xMove = MoveSpeed * Time.deltaTime;
              // moveVect3 = Vector3.right;
        }

        //transform.position += moveVect3 * MoveSpeed * Time.deltaTime;
        transform.Translate(new Vector3(xMove, yMove, 0));


    }

   private void OnTriggerEnter(Collider other)
   {    
    if(other.gameObject.tag == "Wall" && itTurns){
       
                if(hor == 1){
                    hor = 2;
                }else if(hor == 2){
                    hor = 1;
                }
           

        }
    }

}
