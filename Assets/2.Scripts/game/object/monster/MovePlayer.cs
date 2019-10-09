using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float movePower = 55f;
    public float jumpPower = 55f;
    public bool breakE = false;
    Rigidbody2D rigid;


    Vector3 movement;

   // public bool dead;
    public bool isJumping = false;
    public bool inGround;

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

      
    }

    private void FixedUpdate()
    {

        Move();
        jump();
    }

    public void Die()
    {
        Debug.Log("Dead");
      //  dead = false;
    }

    void Move()
    {
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal")<0)
        {
            moveVelocity = Vector3.left;
        }else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            moveVelocity = Vector3.right;
        }

        transform.position += moveVelocity * movePower * Time.deltaTime;

    }

    public void jump()
    {
        
        if (!isJumping)
            return;

        rigid.velocity = Vector2.zero;

        Vector2 JumpVelocity = new Vector2(0, jumpPower);
        rigid.AddForce(JumpVelocity, ForceMode2D.Impulse);
        inGround = false;

        isJumping = false;
      

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
       
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "GROUND")
        {
            inGround = true;
        }
        if (breakE)
        {
            if (collision.gameObject)
            {
                //.tag == "Wall" || collision.gameObject.tag == "GROUND" 
                Destroy(collision.gameObject);
            }
        }
    }


}
