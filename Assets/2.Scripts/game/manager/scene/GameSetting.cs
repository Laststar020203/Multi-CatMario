using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting
{
    private string myName;
    private byte characterCode;
    private float soundValue = 1f;

    public float SoundValue { get { return soundValue; } set { soundValue = value; } }
    public string Name { get { return myName; } set { myName = value; } }

    public byte CharacterCode { get { return characterCode; } set { characterCode = value; } }

   
}
