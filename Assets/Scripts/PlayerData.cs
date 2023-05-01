using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{    
    public string PlayerId;
    
    public Vector3 PlayerPosition;

    public int CurrentHp;

    public int Experience;

    public PlayerData()
    {
        PlayerId = string.Empty;
        PlayerPosition = Vector3.zero;
        CurrentHp = 100;
        Experience = 0;
    }
}
