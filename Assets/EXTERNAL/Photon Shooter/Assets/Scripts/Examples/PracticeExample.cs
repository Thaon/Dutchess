using UnityEngine;
using System.Collections;

/// <summary>
/// Extended class to handle the Practice Game Mode.
/// </summary>
/// 
public class PracticeExample : GameMode
{
    /// <summary> Inits some variables. </summary>
    public override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;
        if (AimGUI)
            AimGUI.SetActive(false);
    }

    /// <summary> Creates the player. Called from the RoomScript script. </summary>
    protected override IEnumerator Init()
    {
        if (Player == null)
        {
            Invoke("CreatePlayerObject", .1f);
        }
        Debug.Log("PracticeExample_OnJoinedRoom");
        yield return null;
    }

    /// <summary> Called when we left the practice mode. Detachs the camera from the player. Destroys the player. </summary>
    protected override void Exit()
    {
        if (m_Camera)
            m_Camera.transform.parent = null;
        if (Player != null)
            PhotonNetwork.Destroy(Player);
        isInit = false;
    }

    /// <summary> Instantiates the player. </summary>
    protected override void CreatePlayerObject()
    {
        Player = PhotonNetwork.Instantiate(PlayerPrefabName, GetRandomSpawnPoint(), Quaternion.identity, 0);
        Player.name = "LocalPlayer";
        if(GameUI.activeSelf)
        Player.SendMessageUpwards("Init");
        if (ExitMenu.activeSelf)
        {
            Player.SendMessageUpwards("Stop");
        }
        if (AimGUI)
            AimGUI.SetActive(true);
        isInit = true;
    }

    /// <summary> Handles the Ondeath event. </summary>
    public override void OnDeath(GameObject player, PhotonPlayer other)
    {
        m_Camera.transform.parent = null;
        player.gameObject.SetActive(false);
        PhotonNetwork.Destroy(player.gameObject);
        Invoke("CreatePlayerObject", .1f);
        if (other != null)
            Debug.Log(other.name + " Killed: " + player.GetComponent<PhotonView>().owner.name);
    }

}
