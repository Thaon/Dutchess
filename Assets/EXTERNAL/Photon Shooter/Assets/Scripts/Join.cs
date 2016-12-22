using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script updates the list of servers, the region info and players online.
/// </summary>

public class Join : MonoBehaviour
{
    public ServersList ServerList; // Reference to the ServerList script.
    public Text Region; // Reference to the region text.
    public Text PlayersOnline; // Reference to the player online text.
    public PhotonInit PhotonInitScript; // Referemce to the PhotonInitScript script.
    public float RefreshTime = 2.0f; // Sets the referesh time to update the list.

    public string[] RegionNames; // Strings containing all the regions photon is currently supporting.
    public Text SystemMessage; // Reference to the system message text.

    /// <summary> Starts the function CustomUpdate as soon as the gameobject is enabled </summary>
    public void OnEnable()
    {
        SystemMessage.text = "";
        ServerList.FillList();
        InvokeRepeating("CustomUpdate", 0.0f, RefreshTime);
    }

    /// <summary> Cancel all invokes. </summary>
    void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary> Updates the region info and players online </summary>
    void UpdateInfo()
    {
        Region.text = RegionNames[(int)PhotonInitScript.CloudRegionCodes];
        PlayersOnline.text = PhotonNetwork.countOfPlayers.ToString();
    }

    /// <summary> CustomUpdate repeating time set in RefreshTime </summary>
    void CustomUpdate()
    {
        UpdateInfo();
    }

}
