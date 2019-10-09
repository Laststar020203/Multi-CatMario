using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.StartScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
