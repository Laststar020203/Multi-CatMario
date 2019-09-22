using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : Move
{

    Move move;

    // Start is called before the first frame update
    void Start()
    {
        move = GameObject.Find("Block").GetComponent<Move>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Walls")
        {
            
        }


    }

}
