using UnityEngine;
using System.Collections;

/// <summary>
/// Base class to handle the game modes.
/// </summary>

public class GameMode : MonoBehaviour
{
    public string PlayerPrefabName = "Player"; // Player prefab name we want to instantiate (This prefab should be in the Resource folder).
    protected bool isInit = false; // It is used to know if the game mode is started.
    protected GameObject Player; // Reference to the local player.
    public Vector3 SpawnOffset = Vector3.zero; // Offset to have more control over the spawn player position.
    protected Camera m_Camera; // Reference to the Main Camera.
    protected GameObject GameUI; // Reference to the In-Game menu.
    protected GameObject ExitMenu; //Reference to the Exit menu.
    protected GameObject AimGUI; // Reference to the Aim GUI (Health, bullets, etc..)
    public bool isTeamBased = false;
    Transform[] SpawnPoints; // It is used to know if the game mode is team based.
    public int SpawnPointsNum = 12; // Reference to the spawn ponts number. This is to know how many spawn points there are in the scene.

    /// <summary> Init some variables. Get the spawn points etc.. </summary>
    public virtual void Awake()
    {
        if (AimGUI == null)
            AimGUI = GameObject.Find("Multiplayer").transform.Find("GameUI/AimGUI").gameObject;
        if(GameUI == null)
        GameUI = GameObject.Find("Multiplayer").transform.Find("GameUI").gameObject;
        if (ExitMenu == null)
            ExitMenu = GameObject.Find("Multiplayer").transform.Find("GameUI/ExitMenu").gameObject;

        Transform SpawnPointsObj = GameObject.Find("SpawnPoints").transform;
        SpawnPoints = new Transform[SpawnPointsNum];
        int i = 0;
        foreach (Transform child in SpawnPointsObj)
        {
            SpawnPoints[i] = child;
            i++;
        }
    }

    /// <summary> Resets the players team. </summary>
    protected virtual IEnumerator Init()
    {
        PhotonNetwork.player.SetTeam(PunTeams.Team.none);
        yield return null;
    }

    protected virtual void Exit()
    {

    }

    /// <summary> Spawns the player </summary>
    protected virtual void CreatePlayerObject()
    {
  
    }

    /// <summary> Simple funtion to show a message </summary>
    public virtual void ShowMessage(string message)
    {
    }

    /// <summary> Simple function to get a random spawn point. </summary>
    protected Vector3 GetRandomSpawnPoint()
    {
        int rnd = Random.Range(0, SpawnPointsNum);
        Vector3 pos = SpawnPoints[rnd].position;
        Vector3 position = pos + SpawnOffset;
        position.x += Random.Range(-3f, 3f);
        position.z += Random.Range(-4f, 4f);
        return position;
    }

    /// <summary> Handles the "On Death" event. </summary>
    public virtual void OnDeath(GameObject player, PhotonPlayer other)
    {
 
    }

}
