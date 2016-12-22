using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// This script fills the server list in the join menu.
/// </summary>
/// 

public class ServersList : MonoBehaviour
{
    private List<GameObject> Servers; // List of current Servers.
    public Color SelectedColor; // Color of the selected Server.
    public Color NormalColor;

    public ScrollRect scrollRect; // Reference to the Vertical ScrollBar
    public GameObject ScrollBarObj; // Reference to the Vertical ScrollBar Gameobject (So we can enable or disable it depending on the quantity of the servers number).
    public Transform Content;

    public RoomScript RoomScript; // Reference to the RoomScript to get the Maps and Modes of the Servers. 

    public Toggle[] Filters; // Filters for the list "In Lobby", "Open" etc..
    public GameObject JoinButton; // Referencve to the Join Button (If we find Servers available we enable it and select the first server in the list). 

    public GameObject EnterPassword; // Reference to the PassWord Menu (If we try to join a server with a password we show the password menu).
    public GameObject Join; // Reference to the Join Menu (Here we can see the list of servers and join a game).
    public InputField Password; // Reference to the password input field.
    public GameObject Room; // Reference to the Room Menu (We show the Room Menu if we are successful connecting to a game)

    public Text[] SystemMessage; // Reference to the system message (We use it to show messages i.e: "Incorrect password" or "Room is full"
    private int ServerSelectedNumber = 0; // Current Server selected
    private GameObject ServerSelected; // Reference to the current Server Gameobject.

    /// <summary> Init some variables </summary>
    void Start()
    {
        if (Servers == null)
        {
            Servers = new List<GameObject>();
        }
    }

    /// <summary> Fill the list </summary>
    public void FillList() {
        RoomInfo[] RoomList = PhotonNetwork.GetRoomList(); // Get the list of rooms.
        ClearServerList(); // Clear the previous list.  

        ShowScrollBar(false); // Hide the vertical scrll bar.
        JoinButton.SetActive(false); // Hide the Join button.

        if (RoomList != null) // If we got any room.
        {
            int nServers = 0; // Reset the number of Servers
            for(int i = 0; i < RoomList.Length; i++)  // Loop through the list of rooms.
            {
                /// <summary> Checks filters </summary>
                if (Filters[0].isOn)
                {
                    string pw = RoomList[i].customProperties["pw"].ToString();
                    if (pw.Length > 0)
                        continue;
                }

                if (Filters[1].isOn)
                {
                    if (RoomList[i].playerCount >= RoomList[i].maxPlayers)
                        continue;
                }

                if (Filters[2].isOn)
                {
                    if ((string)RoomList[i].customProperties["gs"] != "Lobby")
                        continue;
                }
                
                GameObject ServerObj = (GameObject)Instantiate(Resources.Load("Server")); // Instantiate the Server GameObject.
                if (i == 0)
                    SetServerSelected(ServerObj, 0); // Select the first Server in the list.

                Servers.Add(ServerObj); //Add it to the list
                ServerObj.transform.SetParent(Content, false); // Add it to the sroll content

                /// <summary> Get the info from the Server and update the Server Gameobject with the new info </summary>
                ServerObj.transform.FindChild("ServerName").GetComponent<Text>().text = RoomList[i].name;
                ServerObj.transform.FindChild("Map").GetComponent<Text>().text = RoomScript.Maps[(int)RoomList[i].customProperties["mp"]].ToString();
                ServerObj.transform.FindChild("Mode").GetComponent<Text>().text = RoomScript.Modes[(int)RoomList[i].customProperties["gm"]].ToString();
                ServerObj.transform.FindChild("Status").GetComponent<Text>().text = (string)RoomList[i].customProperties["gs"];

                /// <summary> Checks if the room has password </summary>
                string pass = RoomList[i].customProperties["pw"].ToString();
                if(pass.Length<1)
                    ServerObj.transform.FindChild("Pass").GetComponent<Text>().text = "No";
                else
                    ServerObj.transform.FindChild("Pass").GetComponent<Text>().text = "Yes";
                
                ServerObj.transform.FindChild("Players").GetComponent<Text>().text = RoomList[i].playerCount + "/" + RoomList[i].maxPlayers; // Update the players currently in the room and Max players for that room.
                nServers += 1; // Update the number of Servers
            }

            /// <summary> Enable the vertical scroll bar if there's more than 6 Servers </summary>
            if (nServers > 6)
            {
                ShowScrollBar(true);
            }
        }

    }
    /// <summary> Checks the password, if it is correct then we try to join the room. If it is not correct then we show the "Incorrect password" message  </summary>
    public void CheckPassword()
    {
        RoomInfo[] RoomList = PhotonNetwork.GetRoomList();
        string RoomName = ServerSelected.transform.FindChild("ServerName").GetComponent<Text>().text;
        if (RoomList != null)
        {
            for (int i = 0; i < RoomList.Length; i++)
            {
                if(RoomName == RoomList[i].name)
                {
                    if(Password.text == RoomList[i].customProperties["pw"].ToString())
                    {
                        JoinRoom();
                    }
                    else
                    {
                        SystemMessage[1].text = "Incorrect Password";
                    }
                }
            }
        }
    }

    /// <summary> Checks the Max players in the selected room. If the current players in the room is equal to the Max player in the room it will return true if it's not then false.
    /// It is useful to check if we can join a room or not.
    /// </summary>
    public bool CheckMaxPlayers()
    {
        RoomInfo[] RoomList = PhotonNetwork.GetRoomList();
        string RoomName = ServerSelected.transform.FindChild("ServerName").GetComponent<Text>().text;
        if (RoomList != null)
        {
            for (int i = 0; i < RoomList.Length; i++)
            {
                if (RoomName == RoomList[i].name)
                {
                    if (RoomList[i].playerCount == RoomList[i].maxPlayers)
                        return true;
                }
            }
        }
        return false;
    }

    /// <summary> Trigger Event when we press the "Join" button. If the room has a password then we show the password menu else we just try to join the Server </summary>
    public void OnJoinButtonClick()
    {
       string pass = Servers[ServerSelectedNumber].transform.FindChild("Pass").GetComponent<Text>().text;
        if (CheckRoomInLobby())
        {
            if (pass == "Yes")
            {
                EnterPassword.SetActive(true);
                Join.SetActive(false);
            }
            else
            {
                JoinRoom();

            }
        }
        else
        {
            JoinRoom();
        }
        }

    /// <summary> Checks if the selected Room is in the "Lobby" </summary>
    bool CheckRoomInLobby()
    {
        if (Servers[ServerSelectedNumber].transform.FindChild("Status").GetComponent<Text>().text == "Lobby")
            return true;

        return false;
    }

    /// <summary> Function to Join the Server. If the server is not full we just try to join it. If the Server is full we show the message "The room is full" </summary>
    void JoinRoom()
    {
        if(!CheckMaxPlayers())
        PhotonNetwork.JoinRoom(Servers[ServerSelectedNumber].transform.FindChild("ServerName").GetComponent<Text>().text);
        else
            SystemMessage[0].text = "The room is full.";
    }

    /// <summary> Loop through the list of Servers to set the selected Server. Activate the Join Button and set the color of the selected server to the selected color </summary>
    void SetServerSelected(GameObject obj, int number)
    {
        for (int i = 0; i < Servers.Count; i++)
        {
            Servers[i].GetComponent<Image>().color = NormalColor;
        } 

        ServerSelectedNumber = number;
        ServerSelected = obj;
        ServerSelected.GetComponent<Image>().color = SelectedColor;
        JoinButton.SetActive(true);
    }

    /// <summary> Checks if the rooms name  </summary>
    public bool CheckRoomName(string _RoomName)
    {
        RoomInfo[] RoomList = PhotonNetwork.GetRoomList();
        if (RoomList != null)
        {
            for (int i = 0; i < RoomList.Length; i++)
            {
                if (RoomList[i].name == _RoomName)
                {
                    return true;
                }
            }

            }
            return false;
    }

    /// <summary> Shows or hides the vertical scroll bar </summary>
    void ShowScrollBar(bool value)
    {
        scrollRect.enabled = value;
        ScrollBarObj.SetActive(value);
    }

    /// <summary> Clear the list of Servers. </summary>
    void ClearServerList()
    {
        for(int i = 0; i < Servers.Count; i++)
        {
            Destroy(Servers[i]);
        }
        Servers.Clear();
    }

    /// <summary> Calls the CheckClickServer function every frame </summary>
    void Update()
    {
        CheckClickServer();
    }

    /// <summary> Cheks if we clicked on a Server. If we did then we select it </summary>
    void CheckClickServer()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameObject server = EventSystem.current.currentSelectedGameObject;
            if (server != null && server.name == "Server(Clone)")
            {
                for (int i = 0; i < Servers.Count; i++)
                {
                    if(server == Servers[i])
                    {
                        SetServerSelected(server, i);
                    }
                }
                 
            }
        }
    }

    /// <summary> Calls the FillList after we join the main lobby </summary>
    void OnJoinedLobby()
    {
        Invoke("FillList", .5f);
    }

    /// <summary> This is called after we join a Server. Hides the Join menu and password menu and activate the Room Menu </summary>
    void OnJoinedRoom()
    {
        Join.SetActive(false);
        EnterPassword.SetActive(false);
        Room.SetActive(true);
        Debug.Log("OnJoinedRoom() : You Have Joined the Room : " + PhotonNetwork.room.name);
    }
}
