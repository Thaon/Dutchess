using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script handles Room.
/// </summary>

public class RoomScript : MonoBehaviour {

    public Text RoomName; // Reference to the Room Name text.
    public Text PlayersNumber; // Reference to the Room Players Number text.
    public float RefreshTime = 2.0f; // Refresh time for the custom update.
    public int CurrentMap = 0; // Reference to the current map number.
    public int CurrentMode = 0; // Reference to the current game mode number.

    public string[] Maps; // Reference to the map "Names" (We can define the map names here).
    public string[] Modes; // Reference to the Game modes "Names" (We can define he game modes here).

    public Text MapText; // Reference to the Map text.
    public Text ModeText; // Reference to the Game Mode text.

    public Text MatchDurationText; // Reference to the match durationt text.

    public GameObject[] GameobjectsToDisable; // List of gameobjects to disable if we are not the master client.
    public GameObject Room; // Reference to the Room Menu.
    public GameObject Main; // Reference to the Main Menu.
    public GameObject PracticeObj; // Reference to the the Practice Gameobject.

    int MatchDurationTime = 5; // Match duration time.

    public int MaxMatchDurationTime = 12; // Referece to the max time for the round.
    Transform LoadingLevelPanel;

    /// <summary> Inits the custom update and sets the room custom property "mt" to the "match duration" we selected in the menu. </summary>
    void Awake () {
        InvokeRepeating("UpdateInfo", 0.0f, RefreshTime); //Init the CustomUpdate function.
        PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "mt", MatchDurationTime } }); //Set the Round duration time.
        GameObject Temp = GameObject.Find("Multiplayer");
        LoadingLevelPanel = Temp.transform.Find("LoadingLevelPanel");
    }

    /// <summary> Inits some variables. Checks if we are the master cient and enable/disable the menu depending on it. Sends "Init" message to the practice gameobject. </summary>
    void OnEnable()
    {
        CurrentMap = 0;
        CurrentMode = 0;
        CheckMasterServer(); // Checks if we are the master client.
        InvokeRepeating("UpdateInfo", 0.0f, RefreshTime); // Updates all the info in the room Map, Game mode, Match time.

        /// <summary> Sends "Init" message to the practice gameobject which will create the player for the practice mode. </summary>
        if (PracticeObj)
            PracticeObj.SendMessage("Init");
    }

    /// <summary> Cancel all invokes. </summary>
    void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary> Checks if we are the master client and if so we set the custom property "gs" to Lobby.
    /// If we are not the master client we just disable all the Gameobjects in the GameobjectsToDisable array.
    /// </summary>
    void CheckMasterServer()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "gs", "Lobby" } });
        }
        else
        {
            EnableMasterServerMenu(false);
        }
    }

    /// <summary> Enable/Disable the the Gameobjects in the GameobjectsToDisable array. </summary>
    void EnableMasterServerMenu(bool val)
    {
        for (int i = 0; i < GameobjectsToDisable.Length; i++)
        {
            GameobjectsToDisable[i].SetActive(val);
        }
    }

    /// <summary> Updates the Current Map, text and custom properties for the room. </summary>
    public void MapLeft()
    {
        CurrentMap -= 1;
        if (CurrentMap < 0)
            CurrentMap = Maps.Length-1;
        MapText.text = Maps[CurrentMap];
        SetCustomProperties();
    }

    /// <summary> Updates the Current Map, text and custom properties for the room. </summary>
    public void MapRight()
    {
        CurrentMap += 1;
        if (CurrentMap >= Maps.Length)
            CurrentMap = 0;
        MapText.text = Maps[CurrentMap];
        SetCustomProperties();
    }

    /// <summary> Updates the Current Game Mode, text and custom properties for the room. </summary>
    public void ModeLeft()
    {
        CurrentMode -= 1;
        if (CurrentMode < 0)
            CurrentMode = Modes.Length-1;
        CurrentMode = Mathf.Clamp(CurrentMode, 0, Modes.Length - 1);
        ModeText.text = Modes[CurrentMode];
        SetCustomProperties();
    }

    /// <summary> Updates the Current Game Mode, text and custom properties for the room. </summary>
    public void ModeRight()
    {
        CurrentMode += 1;
        if (CurrentMode >= Modes.Length)
            CurrentMode = 0;
        ModeText.text = Modes[CurrentMode];
        SetCustomProperties();
    }

    /// <summary> Updates the Current Match Duration time, text and custom properties for the room. </summary>
    public void MatchDurationLeft()
    {
        MatchDurationTime -= 1;
        if (MatchDurationTime <= 0)
            MatchDurationTime = MaxMatchDurationTime;
        PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "mt", MatchDurationTime } });
        MatchDurationText.text = MatchDurationTime.ToString();
    }

    /// <summary> Updates the Current Match Duration time, text and custom properties for the room. </summary>
    public void MatchDurationRight()
    {
        MatchDurationTime += 1;
        if (MatchDurationTime > MaxMatchDurationTime)
            MatchDurationTime = 1;
        PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "mt", MatchDurationTime } });
        MatchDurationText.text = MatchDurationTime.ToString();
    }

    /// <summary> Updates the custom properties for the room. </summary>
    void SetCustomProperties()
    {
        if(PhotonNetwork.isMasterClient)
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "gm", CurrentMode }, { "mp", CurrentMap } });
    }

    /// <summary> If the master client left the room check if we are the master client and if we are then enable the menu. </summary>
    void OnMasterClientSwitched()
    {
        if(PhotonNetwork.isMasterClient)
        {
            EnableMasterServerMenu(true);
        }
    }

    /// <summary> Send "Exit" to the practice Gameobject which will destoy the player and then leaves the room, hides the Room menu and shows the Main Menu. </summary>
    public void OnLeftRoom()
    {
        if (PracticeObj)
            PracticeObj.SendMessage("Exit");
        PhotonNetwork.LeaveRoom();
        Main.SetActive(true);
        Room.SetActive(false);
    }

    /// <summary> Updates the Room info, Room name, Current Players, Max Players, Map, Game mode and Match time. </summary>
    void UpdateInfo()
    {
        Room CurrentRoom = PhotonNetwork.room;
        RoomName.text = CurrentRoom.name;
        PlayersNumber.text = PhotonNetwork.room.playerCount + "/" + PhotonNetwork.room.maxPlayers;
        MapText.text = Maps[(int)CurrentRoom.customProperties["mp"]].ToString();
        ModeText.text = Modes[(int)CurrentRoom.customProperties["gm"]].ToString();
        MatchDurationText.text = CurrentRoom.customProperties["mt"].ToString();
    }

    /// <summary> If we are the master client we set the custom property "gs" to "InGame" and load the map selected. </summary>
    public void StartMap()
    {
        if (PhotonNetwork.isMasterClient)
            PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "gs", "InGame" } });
        if (LoadingLevelPanel)
            LoadingLevelPanel.gameObject.SetActive(true);
            PhotonNetwork.LoadLevel(MapText.text);
    }
}
