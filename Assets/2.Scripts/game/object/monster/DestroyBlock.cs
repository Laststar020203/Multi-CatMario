using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlock : MonoBehaviour
{
    public bool maker = false;
    public bool destroy = true;
    public GameObject Mushroom = null;
    bool making = true;

    public Sprite active;
    SpriteRenderer spriteRender;
    protected Animator animator;
    
    //public Object BreakingB;
    bool YesITis = true;

    // Start is called before the first frame update
    void Start()
    {
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER") && destroy)
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("PLAYER") && maker && making)
        {

            Instantiate(Mushroom, transform.position + Vector3.up * 0.3f, transform.rotation);
            spriteRender.sprite = active;
            making = false;

        }
    }
    

}
