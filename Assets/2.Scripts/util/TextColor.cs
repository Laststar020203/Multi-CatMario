using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColor
{
    

    /*
    public static string Wear(byte characterCode, string msg)
    {
        string color = null;
        switch (characterCode)
        {
            case 0:
                color = "#ffffff";
                break;
            case 1:
                color = "#ff0000";
                break;
            case 2:
                color = "#50bcdf";
                break;
            case 3:
                color = "#BFBF00";
                break;
        }
        setColorText(color, ref msg);

        return msg;
    }
    */

    private static void setColorText(string colorCode, ref string text)
    {
        text = "<color=" + colorCode + ">" + text + "</color>";
    }

}
