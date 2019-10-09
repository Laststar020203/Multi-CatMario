using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    public Vector2 direction;

    private Transform tr;

    private void Start()
    {
        tr = GetComponent<Transform>();
    }
    private void Update()
    {
        tr.Translate(direction * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("LOCALPLAYER"))
        {
            ClientGameSystem.instance.Player.Die();
        }
    }


}
