using UnityEngine;
using System.Collections;

public class ClassicMode : GameMode
{
    /// <summary> Inits some variables. </summary>
    public override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;
    }

    void Start()
    {
        Debug.Log("Initialising mode");
        StartCoroutine(Init());
    }

    /// <summary> Creates the player. Called from the RoomScript script. </summary>
    protected override IEnumerator Init()
    {
        if (Player == null)
        {
            Debug.Log("creating player");
            Invoke("CreatePlayerObject", .1f);
        }
        
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
        print("Spawning player:" + PlayerPrefabName);
        Player = PhotonNetwork.Instantiate(PlayerPrefabName, GetRandomSpawnPoint(), Quaternion.identity, 0);
        Player.name = "LocalPlayer";
        isInit = true;
    }

    /// <summary> Handles the Ondeath event. </summary>
    public override void OnDeath(GameObject player, PhotonPlayer other)
    {
    }
}
