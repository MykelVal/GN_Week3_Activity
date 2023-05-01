using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    private const string SAVE_KEY = "PlayerData";

    public PlayerData PlayerData;

    [ContextMenu("Save")]
    public void Save()
    {
        var stringValue = JsonUtility.ToJson(PlayerData);
        Debug.Log(stringValue);
        PlayerPrefs.SetString(SAVE_KEY, stringValue);
        PlayerPrefs.Save();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            var convert = PlayerPrefs.GetString(SAVE_KEY);
            try
            {
                PlayerData = JsonUtility.FromJson<PlayerData>(convert);
            } 
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
            }
        }
        else
        {
            PlayerData = new PlayerData();
        }
    }
}
