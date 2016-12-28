using UnityEngine;
using System.Collections;

public class ClassicMode : GameMode
{

    #region member variables

    public int m_minNPC = 5;

    #endregion

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
        if (PhotonNetwork.isMasterClient)
            StartCoroutine(SpawnNPC(Random.Range(10, 20)));
    }

    public void ReplenishNPCPool()
    {
        int npcs = GameObject.FindGameObjectsWithTag("NPC").Length;
        int ran = Random.Range(1, 3);

        if (PhotonNetwork.isMasterClient)
        {
            if (npcs < m_minNPC)
            {
                for (int i = 0; i < ran; i++)
                {
                    GameObject npc = PhotonNetwork.Instantiate("NPC", GetRandomSpawnPoint(), Quaternion.identity, 0);
                    npc.GetComponent<TouristNPC>().m_maxSatisfaction += Mathf.RoundToInt(Random.Range(0, 3));
                }
            }
        }
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

    public IEnumerator SpawnNPC(float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        GameObject npc = PhotonNetwork.Instantiate("NPC", GetRandomSpawnPoint(), Quaternion.identity, 0);
        npc.GetComponent<TouristNPC>().m_maxSatisfaction += Mathf.RoundToInt(Random.Range(0, 3));
        StartCoroutine(SpawnNPC(Random.Range(10, 20)));
    }
}
