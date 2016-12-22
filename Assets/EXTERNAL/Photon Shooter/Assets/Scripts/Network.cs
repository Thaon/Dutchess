using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script handles the Network Menu.
/// </summary>

public class Network : MonoBehaviour {
    public Text Players; // Reference to the Players text. (It shows how many players are online in that region)
    public Text Ping; // Reference to the ping text. (It shows the ping we are getting for that region).
    public Text ServerRegion; // Reference to the Server Region text. (It shows the could region).
    public PhotonInit PhotonInitScript; // Reference to the PhotonInitScript.
    public string[] RegionNames; // Reference to the RegionNames string (It conatins all the regions photon currently support).
    int RegionNumber = 0; // It is used to know the current region selected;
    bool isReady = false; // It is used to show the values when we are connected to the cloud otherwise we show "Connecting..".

    public float RefreshRate = .5f; // Interval of time we want to refresh the list.

    /// <summary> Updates the current region selected. Inits the custom update. </summary>
    void OnEnable()
    {
        InvokeRepeating("CustomUpdate", 0.0f, RefreshRate);
        RegionNumber = (int)PhotonInitScript.CloudRegionCodes;
        ServerRegion.text = RegionNames[(int)(CloudRegionCode)RegionNumber];
        isReady = true; 
    }

    /// <summary> Cancel all invokes. </summary>
    void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary> Updates the current region selected. When we click to the left/right buttons in the region selection menu. We skip the region 4 since it is not used by photon. </summary>
    public void UpdateRegion(string val)
    {
        int currentValue = 0;
        currentValue = (int)PhotonInitScript.CloudRegionCodes;
        if (val == "Left")
        {
            currentValue -= 1;
       
            if (currentValue < 0)
                currentValue = 8;

            if (currentValue == 4)
                currentValue -= 1;

            PhotonInitScript.UpdateRegion((CloudRegionCode)currentValue);
            ServerRegion.text = RegionNames[(int)(CloudRegionCode)currentValue];
        }

        if(val == "Right")
        {
            currentValue += 1;

            if (currentValue > 8)
                currentValue = 0;

            if (currentValue == 4)
                currentValue += 1;

            PhotonInitScript.UpdateRegion((CloudRegionCode)currentValue);
            ServerRegion.text = RegionNames[(int)(CloudRegionCode)currentValue];
        }
        Debug.Log(currentValue);
    }

    /// <summary> When we click on the "Apply" button we have to disconnect and reconnect again to the new cloud region. </summary>
    public void Apply()
    {
        PhotonNetwork.Disconnect();
    }

    /// As son as we are disconnected we try to reconnect again to the photon could. </summary>
    void OnDisconnectedFromPhoton()
    {
        isReady = false;
        PhotonInitScript.Join();
    }

    /// <summary> To know when we are connected to the main lobby. </summary>
    void OnJoinedLobby()
    {
        isReady = true;
    }

    /// <summary> This is to update the number of players and ping texts. </summary>
    void CustomUpdate () {
        if(isReady)
        {
            Players.text = PhotonNetwork.countOfPlayers.ToString();
            Ping.text = PhotonNetwork.GetPing().ToString();
        }

        else
        {
            Players.text = "Connecting..";
            Ping.text = "Connecting..";
        }
    }
}
