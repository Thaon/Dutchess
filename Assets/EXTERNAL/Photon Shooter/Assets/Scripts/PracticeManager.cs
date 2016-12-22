using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the practice manager. Similar to the game manager but we need to do things a bit differently.
/// </summary>

public class PracticeManager : GameManager {
    public GameObject Room; // Reference to the room menu.
    public GameObject ConnectionStatus; // Reference to the connection status game object.

    public override void Awake()
    {
  
    }

    /// <summary> Locks and hide the cursor and we send "Init" to the player to enable the movement and input in it. </summary>
    public override void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerObj = GameObject.Find("LocalPlayer");
        if (PlayerObj != null)
            PlayerObj.SendMessageUpwards("Init");
    }

    /// <summary> If we are getting back to the room menu we disable the player movements and input. </summary>
    public override void OnDisable()
    {
        base.OnDisable();
        if (PlayerObj != null)
            PlayerObj.SendMessageUpwards("Stop");
    }

    /// <summary> Gets back to the room menu and shows the connection status again. </summary>
    public override void Stop()
    {
        base.Stop();
        Room.SetActive(true);
        ConnectionStatus.SetActive(true);
    }

}
