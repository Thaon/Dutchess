using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// This script handles Scoreboard.
/// </summary>

public class Scoreboard : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the vertical scroll.
    public GameObject ScrollBarObj; // Reference to the vertical scroll gameobject.
    public List<GameObject> Players; // Reference to the players list.

    public Transform Content; // Reference to the vertical scroll content.
    public Color PlayerColor; // Reference to the player color. (We use this for the local player).
    bool UseTeams = false; // This is to know if the game mode is team based.
    public GameObject BlueTeamObj; // This is to separate the teams "Blue/Red".
    public GameObject RedTeamobj; // This is to separate the teams "Blue/Red".
    public float RefreshTime = .5f; // Refresh time for the custom update function.

    TeamScoreboard TeamsScore; // Reference to the TeamScore class (Simple Class to update the Scoreboard in a team match).
    public int BlueScore = 0; // Reference to the Blue team score.
    public int RedScore = 0; // Reference to the Red team score.

    /// <summary> Inits some variables. Starts the custom update funtion. Checks if the game is team based and if it is we enable the team menu. </summary>
    void Start()
    {
        if (Players == null)
        {
            Players = new List<GameObject>();
        }
        InvokeRepeating("CustomUpdate", 0.0f, RefreshTime);
        GameObject temp = GameObject.Find("Multiplayer").transform.Find("TeamsScore").gameObject;
        if (temp)
        {
            GameMode gm = GameObject.FindObjectOfType(typeof(GameMode)) as GameMode;
            if (gm.isTeamBased)
            {
                UseTeams = true;
                temp.SetActive(true);
            }
            TeamsScore = temp.GetComponent<TeamScoreboard>();
        }
    }

    /// <summary> Shows the vertical scroll bar. </summary>
    void ShowScrollBar(bool value)
    {
        scrollRect.enabled = value;
        ScrollBarObj.SetActive(value);
    }

    /// <summary> Fills the players list as soon as the gameobject is enabled. </summary>
    void OnEnable()
    {
        FillList();
    }

    /// <summary> Cancel all invokes. </summary>
    void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary> Fills the list of players. Updates the players kills. </summary>
    public void FillList()
    {
        ClearPlayerList();
        PhotonPlayer[] PlayerList = PhotonNetwork.playerList;

        List<GameObject> TeamBluePlayers = new List<GameObject>();
        List<GameObject> TeamRedPlayers = new List<GameObject>();

        int i = 0;
        int BlueTeam = 0;
        int RedTeam = 0;

        BlueScore = 0;
        RedScore = 0;

        foreach (PhotonPlayer player in PlayerList)
        {
            if(UseTeams)
            {
                if (player.GetTeam() == PunTeams.Team.none)
                    continue;
            }

            GameObject PlayerObj = (GameObject)Instantiate(Resources.Load("ScoreBoardUI"));

            Players.Add(PlayerObj);
            PlayerObj.transform.SetParent(Content, false);

            PlayerObj.transform.FindChild("Name").GetComponent<Text>().text = player.name;
            PlayerObj.GetComponent<Kick>().Player = player;
            if (player.customProperties.ContainsKey("pk"))
            {
                Transform killsTransform = PlayerObj.transform.FindChild("Kills");
                if (killsTransform != null)
                    killsTransform.GetComponent<Text>().text = player.customProperties["pk"].ToString();

                if(player.GetTeam() == PunTeams.Team.blue)
                {
                    BlueScore += (int)player.customProperties["pk"];
                }

                if (player.GetTeam() == PunTeams.Team.red)
                {
                    RedScore += (int)player.customProperties["pk"];
                }
            }

            if (PhotonNetwork.player == player)
            {
                PlayerObj.GetComponent<Image>().color = PlayerColor;
            }

            if (UseTeams)
            {
                if (player.GetTeam() == PunTeams.Team.blue)
                {
                    BlueTeam += 1;
                    TeamBluePlayers.Add(PlayerObj);
                }

                if (player.GetTeam() == PunTeams.Team.red)
                {
                    RedTeam += 1;
                    TeamRedPlayers.Add(PlayerObj);
                }
            }

            i += 1;
        }

        if (UseTeams)
        {
            if (BlueTeam > 0)
            {
                BlueTeamObj.SetActive(true);
                BlueTeamObj.transform.SetSiblingIndex(0);

                for (int a = 0; a < TeamBluePlayers.Count; a++)
                {
                    TeamBluePlayers[a].transform.SetSiblingIndex(a + 1);
                }
            }
            else
                BlueTeamObj.SetActive(false);

            if (RedTeam > 0)
            {
                RedTeamobj.SetActive(true);
                for (int a = 0; a < TeamRedPlayers.Count; a++)
                {
                    TeamRedPlayers[a].transform.SetSiblingIndex(RedTeamobj.transform.GetSiblingIndex() + 1);
                }
            }
            else
                RedTeamobj.SetActive(false);

            if (TeamsScore)
                TeamsScore.SetScore(BlueScore, RedScore);
        }

    }

    /// <summary> Updates the players kills count. </summary>
    void LateUpdate()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].GetComponent<Kick>().Player.customProperties.ContainsKey("pk"))
            {
                Transform killsTransform = Players[i].transform.FindChild("Kills");
                if (killsTransform != null)
                {
                    killsTransform.GetComponent<Text>().text = Players[i].GetComponent<Kick>().Player.customProperties["pk"].ToString();
                }
            }

        }

    }

    /// <summary> Custom Update to fill the list of players every x period of time. </summary>
    void CustomUpdate()
    {
        FillList();
    }

    /// <summary> Update the list of players when a player joins the room. </summary>
    void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        FillList();
    }

    /// <summary> Update the list of players when a player leaves the room. </summary>
    void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        FillList();
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
