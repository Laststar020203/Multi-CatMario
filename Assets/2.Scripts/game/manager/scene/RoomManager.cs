using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 게임 맵 설정이라든지 이런것들을 담당
 * */
public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    private Room room;
    public Room RoomInfo { get { return room; }
        set {
            if(room == null)
            room = value;
        } }

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
        DontDestroyOnLoad(instance);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
