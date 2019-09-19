using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalPlayer : Player
{
    public void LeftButtonDown(ButtonEvent eventData)
    {
        LeftMove();
    }

    public void RightButtonDown(ButtonEvent eventData)
    {
        RightMove();
    }

    public void JumpButtonDown(ButtonEvent eventData)
    {
        Jump();
    }
}
