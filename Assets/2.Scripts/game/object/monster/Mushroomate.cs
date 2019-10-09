using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroomate : MonoBehaviour
{
    MovePlayer p;

    // Start is called before the first frame update
    void Start()
    {

       p = GameObject.Find("Player").GetComponent<MovePlayer>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "PLAYER")
        {
            collision.transform.localScale = new Vector2(2, 2);
            p.breakE = true;
           // p.isJumping = true;
            Destroy(gameObject);
            
        }
        

        if (collision.gameObject.tag == "Monster")
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }

    }


}
