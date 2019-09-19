using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float MoveSpeed = 0;
    public string Direction = "Right";

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
        Vector3 moveVect3 = Vector3.zero;

        if (Direction.Equals("Right"))
        {
            moveVect3 = Vector3.right;
        }else if (Direction.Equals("Left"))
        {
            moveVect3 = Vector3.left;
        }
        else if (Direction.Equals("Up"))
        {
            moveVect3 = Vector3.up;
        }
        else if (Direction.Equals("Down"))
        {
            moveVect3 = Vector3.down;
        }

        transform.position += moveVect3 * MoveSpeed * Time.deltaTime;

    }
}
