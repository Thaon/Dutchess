using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// This script handles Player List.
/// </summary>
/// 

public class PlayerList : MonoBehaviour {
    public ScrollRect scrollRect; // Reference to the vertical scroll bar.
    public GameObject ScrollBarObj; // Reference to the vertical scroll bar gameobject.
    private List<GameObject> Players; // List of Players

    public Transform Content; // Reference to the scrollbar content gameobject.
    public Color PlayerColor; // It is used to set the color of the local player.

    /// <summary> Init some variables. </summary>
    void Awake()
    {
        if (Players == null)
        {
            Players = new List<GameObject>();
        }
    }

    /// <summary> Show or hide the vertical scrollbar. </summary>
    void ShowScrollBar(bool value)
    {
        scrollRect.enabled = value;
        ScrollBarObj.SetActive(value);
    }

    /// <summary> Fill the list of players as soon as the gameobject is enabled. </summary>
    void OnEnable()
    {
        FillList();
    }

    /// <summary> Fill the list of players. If we find a player we add it to the list. Keep track of the photon player and set the color to the "PlayerColor" if it is the local player. 
    /// Show the vertical scroll bar if there are more than 10 players. </summary>
    public void FillList()
    {
        ClearPlayerList();//Clear the previous list.
        PhotonPlayer[] PlayerList = PhotonNetwork.playerList; //Get the Player list.

        int i = 0;
        foreach (PhotonPlayer player in PlayerList)
        {
            GameObject PlayerObj = (GameObject)Instantiate(Resources.Load("PlayerUI"));

            Players.Add(PlayerObj);
            PlayerObj.transform.SetParent(Content, false);
            PlayerObj.transform.FindChild("Name").GetComponent<Text>().text = player.name;
            Transform KickButton = PlayerObj.transform.FindChild("Kick");
            KickButton.GetComponent<Kick>().Player = player;
            if (PhotonNetwork.player == player)
            {
                PlayerObj.GetComponent<Image>().color = PlayerColor;
                KickButton.gameObject.SetActive(false);
            }
            i += 1;
        }
        if (PlayerList.Length > 10)
            ShowScrollBar(true);
        else
            ShowScrollBar(false);
        CheckMasterServer();
    }

    /// <summary> Update the player list if a player joins the room. </summary>
    void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        FillList();
    }

    /// <summary> Update the player list if a player leaves the room. </summary>
    void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        FillList();
    }

    /// <summary> If we are not the master client then we hide the "Kick" buttons else we enable them. </summary>
    void CheckMasterServer()
    {
        GameObject[] GameobjectsToDisable = GameObject.FindGameObjectsWithTag("DisableIfNotMaster"); 

        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < GameobjectsToDisable.Length; i++)
            {
                GameobjectsToDisable[i].SetActive(true);
            }
        }

        else
        {
            for (int i = 0; i < GameobjectsToDisable.Length; i++)
            {
                GameobjectsToDisable[i].SetActive(false);
            }
        }
    }

    /// <summary> Clears the players list. </summary>
    void ClearPlayerList()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            Destroy(Players[i]);
        }
        Players.Clear();
    }

}
