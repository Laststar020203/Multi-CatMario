using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform playerTr;
    public Transform tr;
    public float moving = 5f;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;




    // Use this for initialization
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        tr.position = Vector3.Slerp(playerTr.position, new Vector3(Mathf.Clamp(playerTr.position.x, minX, maxX)
            , Mathf.Clamp(playerTr.position.y, minY, maxY), -10),1);
    }
}
