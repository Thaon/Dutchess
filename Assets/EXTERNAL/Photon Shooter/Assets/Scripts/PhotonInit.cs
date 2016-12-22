using UnityEngine;
using System;
using UnityEngine.UI;

/// <summary>
/// This script handles the connection to the photon cloud. This is the first thing we do when we run the game.
/// </summary>

public class PhotonInit : MonoBehaviour {

    public string Version = "v1.0"; // Version of the game (This is useful to control the versions of the game so you can only connect to a game that is running the same client version).
    public Text LogText; // Text that shows the current connection state.

    public bool UseRegions; // Set in inspector (if true we use the values we stored in the playerprefs to connect to a specific region or a default region if no values are found)
    public CloudRegionCode CloudRegionCodes; // Region Codes

    void Start()
    {
        PhotonNetwork.automaticallySyncScene = true; // To synchronize the loaded level in all clients
    }

    /// <summary>Try to connect to the cloud. </summary>
    void Connect()
    {
        /// <summary> Return if we are already connected </summary>
        if (PhotonNetwork.connectionState != ConnectionState.Disconnected)
        {
            return;
        }

        try
        {
            if (UseRegions)
            {
                PhotonNetwork.ConnectToRegion(CloudRegionCodes, Version);
                Debug.Log("Connecting to: " + CloudRegionCodes.ToString());
            }
            else
                PhotonNetwork.ConnectUsingSettings(Version);
            PhotonNetwork.autoJoinLobby = true;
        }
        catch
        {
            Debug.LogWarning("Couldn't connect to server");
        }
    }

    /// <summary> Simple function to show the connection status </summary>
    void Log(string _text) {
        LogText.text = _text;
    }

    /// <summary> Try to join as soon as the gameobject is enabled </summary>
    void OnEnable()
    {
        Join();
    }


    public void Join()
    {
        /// <summary> Check if we are not already connected </summary>
        if (!PhotonNetwork.connected)
        {
            /// <summary> Check if we already have a stored value for the region (if we do we'll use it when we try to connect to the cloud) </summary>
            int regionNumber = PlayerPrefs.GetInt("CloudRegion", 100);
            if (regionNumber != 100)
            {
                CloudRegionCodes = (CloudRegionCode)regionNumber;
            }
            Connect(); //Try to connect.
        }

    }

    /// <summary> Updates the region and saves the value in the playerprefs </summary>
    public void UpdateRegion(CloudRegionCode value)
    {
        CloudRegionCodes = value;
        PlayerPrefs.SetInt("CloudRegion", (int)CloudRegionCodes);
    }

    void LateUpdate()
    {
        Log(PhotonNetwork.connectionStateDetailed.ToString()); //Updates the current connection status.
    }


    void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected..");
    }

    public void QuitGame()
    {
        Application.Quit(); //Quits the Game.
    }
}
