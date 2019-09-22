using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exam : MonoBehaviour
{

    public float speed = 1f;
    Transform tr;
    Vector2 direction = Vector2.right;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (tr.position.x < -7 || tr.position.x > 7)
            direction = -direction;
        tr.Translate(direction * speed * Time.deltaTime);
    }
}
