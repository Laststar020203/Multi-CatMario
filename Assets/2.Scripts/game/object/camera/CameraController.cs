using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform playerTr;
    public Transform tr;
    public float moving = 5f;

    [SerializeField]
    private float minX;
    [SerializeField]
    private float maxX;
    [SerializeField]
    private float minY;
    [SerializeField]
    private float maxY;

    public float MinX { get => minX; }
    public float MaxX { get => maxX;  }
    public float MinY { get => minY; }
    public float MaxY { get => maxY; }

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    
    public void SetClamp(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxX = maxX;
    }

    public void SetDefaultWaitRoomClamp() {
        
        this.minX = -10.45f;
        this.maxX = 1.88f;
        this.minY = -2.68f;
        this.maxY = 40f;
        
    }
    void Update()
    {
        tr.position = Vector3.Slerp(playerTr.position, new Vector3(Mathf.Clamp(playerTr.position.x, MinX, MaxX)
            , Mathf.Clamp(playerTr.position.y, MinY, MaxY), -10),1);
    }
}
