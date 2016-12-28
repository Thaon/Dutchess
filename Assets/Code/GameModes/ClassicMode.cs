using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClassicMode : GameMode
{

    #region member variables

    public int m_minNPC = 5;

    private RoundTimer roundTimer; // Reference to the round timer script.
    private Text Message;
    private GameObject Result; // Reference to the Result menu.
    private bool m_isOver = false;

    #endregion

    /// <summary> Inits some variables. </summary>
    public override void Awake()
    {
        base.Awake();
        roundTimer = GameObject.FindObjectOfType(typeof(RoundTimer)) as RoundTimer;
        Message = GameObject.Find("Multiplayer").transform.Find("Message").GetComponent<Text>();
        Result = GameObject.Find("Multiplayer").transform.Find("Result").gameObject;
        m_Camera = Camera.main;
    }

    void Start()
    {
        Debug.Log("Initialising mode");
        StartCoroutine(Init());
        if (PhotonNetwork.isMasterClient)
            StartCoroutine(SpawnNPC(Random.Range(10, 20)));
    }

    void LateUpdate()
    {
        if (roundTimer.TimesUp)
        {
            if (Result)
            {
                Result.SetActive(true);
                Result.transform.Find("ResultText").GetComponent<Text>().text = "The Spy won!";
                if (!m_isOver)
                    StartCoroutine(EndRound());
            }
            return;
        }

        /// <summary> Inits the countdown for the round. </summary>
        if (CheckForPlayers() && !roundTimer.isRunning)
        {
            if (Message)
                Message.text = "";
            if (roundTimer)
            {
                roundTimer.StartRound();
            }
        }
    }

    public void SpyWins()
    {
        Result.SetActive(true);
        if (Result)
        {
            Result.SetActive(true);
            Result.transform.Find("ResultText").GetComponent<Text>().text = "The Spy won!";
            if (!m_isOver)
                StartCoroutine(EndRound());
        }
    }

    public void PoliceWins()
    {
        roundTimer.StopRound();
        if (Result)
        {
            Result.SetActive(true);
            Result.transform.Find("ResultText").GetComponent<Text>().text = "The Police won!";
            if (!m_isOver)
                StartCoroutine(EndRound());
        }
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

    public IEnumerator EndRound()
    {
        yield return new WaitForSeconds(3);
        Exit();
    }

    /// <summary> Called when we left the practice mode. Detachs the camera from the player. Destroys the player. </summary>
    protected override void Exit()
    {
        if (m_Camera)
            m_Camera.transform.parent = null;
        if (Player != null)
            PhotonNetwork.Destroy(Player);
        isInit = false;
        Application.LoadLevel(0);
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

    /// <summary> Shows a message and clears the text after 3 seconds. </summary>
    public override void ShowMessage(string message)
    {
        Debug.Log(message);
        base.ShowMessage(message);
        if (Message)
        {
            Message.text = message;
            Invoke("ClearMessage", 3.0f);
        }
    }

    /// <summary> Clears the message. </summary>
    void ClearMessage()
    {
        if (Message)
        {
            Message.text = "";
        }
    }

    /// <summary> Only start the round if there are at least more than 1 player. </summary>
    bool CheckForPlayers()
    {
        int numOfPlayers = PhotonNetwork.playerList.Length;
        if (numOfPlayers > 1)
        {
            return true;
        }
        return false;
    }
}
