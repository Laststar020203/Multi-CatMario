using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagButtonImageChanger : MonoBehaviour
{
    Image[] images;
    [SerializeField]
    private float offAlpha = 70;

    void Awake()
    {
        images = new Image[2];
        images[0] = GetComponent<Image>();
        images[1] = transform.GetChild(0).GetComponent<Image>();

    }

    public void On()
    {
        foreach(Image i in images)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 255);
        }
    }

    public void Off()
    {
        foreach (Image i in images)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 70);
        }
    }
}
