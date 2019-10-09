using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadBoard : MonoBehaviour
{
    private Text health;
    private GameObject g;

    void Awake()
    {
        health = GetComponent<Text>();
        g = gameObject;
    }

    private void OnEnable()
    {
        health.text = "x " + ClientGameSystem.instance.PlayerLife;
    }

}
