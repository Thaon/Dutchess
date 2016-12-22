using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Simple Class to handle the player name. Basically we set a name automatically if we haven't set a custom player name yet. 
/// Stores a custom player name in the players prefabs. Then we'll use it next time instead of creating a random "GUESTxx" one.
/// </summary>
/// 

public class PlayerNameScript : MonoBehaviour {
    public string PlayerNameString; // Reference to the player name string.
    public InputField PlayerName; // Reference to the player name input field (We can use it to set a custom player name).

    /// <summary> Checks if we already have a custom player name. </summary>
    void Start()
    {
        CheckPlayerName();
    }

    /// <summary> Checks if we already have a custom player name. 
    /// If we do have one we just use it. If we don't have one then we create a random one.
    /// </summary>
    void CheckPlayerName()
    {
        string pName = PlayerPrefs.GetString("PlayerName", "");
        PlayerName.text = pName;
        PhotonNetwork.player.name = pName;

        if (string.IsNullOrEmpty(pName))
        {
            PlayerNameString = "Guest" + Environment.TickCount % 99;
            PhotonNetwork.player.name = PlayerNameString;
            PlayerName.text = PlayerNameString;
        }
    }

    /// <summary> This is to set the custom player name and it is called when we enter a name in the player name input field. Then we store it in the playerprefs. </summary>
    public void SetPlayerName()
    {
        if (PlayerName.text == "")
        {
            PlayerPrefs.SetString("PlayerName", "");
            CheckPlayerName();
        }
        else
            PlayerPrefs.SetString("PlayerName", PlayerName.text);
        PhotonNetwork.player.name = PlayerName.text;
    }
}
