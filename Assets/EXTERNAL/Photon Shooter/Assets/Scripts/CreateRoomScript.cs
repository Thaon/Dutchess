using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This script handles Room creation.
/// </summary>

public class CreateRoomScript : MonoBehaviour {

    public InputField RoomName; // Reference to the input field for the room name.
    public ServersList ServerList; // Reference to the Server list.

    public Text SystemMessage; // Reference to the system message text.
    public GameObject CreateRoomMenu; // Reference to the Create Room Menu.
    public GameObject RoomMenu; // Reference to the Room Menu.
    public GameObject Password; // Reference to the Password GameObject (If we check the private toggle then it is enabled)

    private bool hasPassword = false; // It is used to check if the room is open or private.
    public int maxPlayers = 2; // Set the max players for the room.
    public InputField InputFieldPassword; // Reference to the password input field.
    public Text PlayersNumber; // Reference to the players number.

    /// <summary> Creates the room. Sets the custom properties for the room: Map, Game mode, password, game state, Max players </summary>
    public void CreateRoom () {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.CustomRoomPropertiesForLobby = new string[4] {"mp", "gm", "pw", "gs" };
		roomOptions.MaxPlayers =(byte)maxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();

        roomOptions.CustomRoomProperties.Add("mp", 0);
        roomOptions.CustomRoomProperties.Add("gm", 0);

        /// <summary> If the room has a password then we set it in the room custom properties else we just set it to an empty string</summary>
        if (hasPassword)
        {
            if (InputFieldPassword.text == "")
                InputFieldPassword.text = "PASSWORD";
            roomOptions.CustomRoomProperties.Add("pw", InputFieldPassword.text);
        }
        else
            roomOptions.CustomRoomProperties.Add("pw", "");

        ClearMessages(); //Clear the system message.

        /// <summary> If we didn't put a name for the room in the the room text we just set it to "NAME" </summary>
        if (RoomName.text == "")
            RoomName.text = "NAME";

        /// <summary> We check if the name was taken. If it wasn't taken then we create the room else we show the message "Name Taken" </summary>
        if (!ServerList.CheckRoomName(RoomName.text))
        {
            PhotonNetwork.CreateRoom(RoomName.text, roomOptions, null);
        }
        else
            SystemMessage.text = "Name Taken";
    }

    /// <summary> Clear the system messages </summary>
    void ClearMessages()
    {
        SystemMessage.text = "";
    }

    /// <summary> This activate/deactivate the password input filed if we check the "Private" toggle </summary>
    public void TogglePrivateRoom()
    {
        hasPassword = !hasPassword;
        Password.SetActive(hasPassword);
    }

    /// <summary> Updates the players number text and values </summary>
    public void UpdatePlayers(string val)
    {
        if (val == "Left")
        {
            maxPlayers -= 1;
            if (maxPlayers < 1)
                maxPlayers = 12;
            UpdateNumberOfPlayers();
        }

        if (val == "Right")
        {
            maxPlayers += 1;
            if (maxPlayers > 12)
                maxPlayers = 1;
            UpdateNumberOfPlayers();
        }
    }

    /// <summary> Updates the players number text </summary>
    void UpdateNumberOfPlayers()
    {
        PlayersNumber.text = maxPlayers.ToString();
    }

    /// <summary> If the room is successfully created then we hide the Create room menu and activate the room menu </summary>
    void OnCreatedRoom()
    {
        RoomMenu.SetActive(true);
        CreateRoomMenu.SetActive(false);
        Debug.Log("OnCreatedRoom() : You Have Created a Room : " + PhotonNetwork.room.name);
    }
}
