using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    private Transform tr;
    public float removeTime;



    private void Start()
    {
        tr = GetComponent<Transform>();
        Destroy(this.gameObject, removeTime);
    }

    public void Update()
    {
        tr.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PLAYER"))
        {
            ClientGameSystem.instance.Player.Die();
        }
    }


}
