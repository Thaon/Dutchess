using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Extended class to handle the Team Death Match Game Mode.
/// </summary>
/// 
/// 
public class DeathMatchTeamsExample : GameMode
{
    public float StartTimeToSpawn = 3.0F; // Reference to the start time to spawn the player.
    float CounterTime; // Reference to the counter time.
    Text CounterText; // Reference to the counter text.
    RoundTimer roundTimer; // Reference to the round timer script.
    private Text Message; // Reference to the message text.
    private GameObject Result; // Reference to the Result menu.
    bool isNextRound = false; // It is used to know when we have to start the next round.

    public float NextRoundTime = 10.0f; // Time to start a new round after the current round time is up.
    private GameObject TeamSelection; // Reference to the team selection menu.
    GameManager gManager; // Reference to the game manager script.

    /// <summary> Inits some variables and starts the Init coroutine. </summary>
    public override void Awake()
    {
        base.Awake();
        gManager = GameObject.FindObjectOfType(typeof(GameManager)) as GameManager;
        gManager.isInit = false;
        CounterTime = StartTimeToSpawn;
        m_Camera = Camera.main;

        /// <summary> Finds the Team Selection menu and enables it. </summary>
        TeamSelection = GameObject.Find("Multiplayer").transform.Find("TeamSelection").gameObject;
        TeamSelection.SetActive(true);

        StartCoroutine(Init());
        if (AimGUI)
            AimGUI.SetActive(false);
        roundTimer = GameObject.FindObjectOfType(typeof(RoundTimer)) as RoundTimer;
        GameObject _counter = GameObject.Find("Counter");
        if (_counter)
            CounterText = _counter.GetComponent<Text>();

        Message = GameObject.Find("Multiplayer").transform.Find("Message").GetComponent<Text>();
        Result = GameObject.Find("Multiplayer").transform.Find("Result").gameObject;

        /// <summary> Show the cursor so we can select the team at the start of the round. </summary>
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary> This is called when we start the round. </summary>
    protected override IEnumerator Init()
    {
        /// <summary> Waits until we select the team. </summary>
        PunTeams.Team team = PhotonNetwork.player.GetTeam();
        while (team == PunTeams.Team.none)
        {
            team = PhotonNetwork.player.GetTeam();
            yield return null;
        }

        /// <summary> Hides and locks the cursor. </summary>
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        /// <summary> Countdown to spawn the player. </summary> 
        while (CounterTime > 0)
        {
            CounterTime -= Time.deltaTime;
            if (CounterText && CounterTime >= 1)
            {
                int counter = Mathf.RoundToInt(CounterTime);
                CounterText.text = "Spawning in... " + counter.ToString();
            }
            yield return null;
        }

        /// <summary> Clears the counter text. </summary>
        if (CounterText)
            CounterText.text = "";

        /// <summary> Instantiates the player. </summary>
        if (Player == null)
        {
            Invoke("CreatePlayerObject", 0.0f);
        }

        gManager.isInit = true; //Tells the game manager that the round is started.
        yield return null;
    }

    /// <summary> This function should be called before we leave a match. Detachs the camera from the player and destroys the player gameobject. </summary>
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
        Player.name = "LocalPlayer"; //We set the name to "LocalPlayer" so it's easier to find it.
        if (GameUI.activeSelf)
            Player.SendMessageUpwards("Init");
        if (ExitMenu.activeSelf)
        {
            Player.SendMessageUpwards("Stop");
        }
        /// <summary> Shows the Aim GUI, Health, bullets etc.. </summary>
        if (AimGUI)
            AimGUI.SetActive(true);
        isInit = true;
    }

    /// <summary> This function handles the on death event. Detachs the camera from the player and destroys the player. 
    /// Then we instance the player again. If have the reference to the player that killed us and it is from a diferent team then we add the score to that player. </summary>
    public override void OnDeath(GameObject player, PhotonPlayer other)
    {
        m_Camera.transform.parent = null;
        player.gameObject.SetActive(false);
        PhotonNetwork.Destroy(player.gameObject);
        Invoke("CreatePlayerObject", .1f);
        if (other != null && other.GetTeam() != player.GetComponent<PhotonView>().owner.GetTeam())
        {
            AddPlayerScore(other);
        }
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

    /// <summary> Adds the player score. We just set it to the players custom property "pk". </summary>
    void AddPlayerScore(PhotonPlayer player)
    {
        if (player.customProperties.ContainsKey("pk"))
            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "pk", (int)player.customProperties["pk"] + 1 } });
        else
            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "pk", 1 } });
    }

    /// <summary> Shows the result menu if the time is up and we are not waiting for a new round. Then it calls the coroutine "NextRound" to start a new round. </summary>
    void LateUpdate()
    {
        if (roundTimer.TimesUp && !isNextRound)
        {
            if (Result)
            {
                Result.SetActive(true);
                Scoreboard scoreBoard = GameObject.FindObjectOfType(typeof(Scoreboard)) as Scoreboard;

                if (scoreBoard.BlueScore > scoreBoard.RedScore)
                    Result.transform.Find("ResultText").GetComponent<Text>().text = "The Blue Team won the round.";

                if (scoreBoard.BlueScore < scoreBoard.RedScore)
                    Result.transform.Find("ResultText").GetComponent<Text>().text = "The Red Team won the round.";

                if (scoreBoard.BlueScore == scoreBoard.RedScore)
                    Result.transform.Find("ResultText").GetComponent<Text>().text = "No one won the round.";
            }
            StartCoroutine(NextRound());
            isNextRound = true;
            return;
        }

        /// <summary> Inits the countdown for the round. </summary>
        if (CheckForPlayers() && !roundTimer.isRunning && !isNextRound)
        {
            if (Message)
                Message.text = "";
            if (roundTimer)
            {
                roundTimer.StartRound();
            }
        }

        /// <summary> If there's less than 2 players in the room then we show the message "Waiting for other players.." and we stop the countdown. </summary>
        if (!CheckForPlayers() && !isNextRound && isInit)
        {
            if (Message)
                Message.text = "Waiting for other players..";
            if (roundTimer && roundTimer.isRunning)
            {
                roundTimer.StopRound();
            }
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

    /// <summary> Starts a new round. We show a counter and when the next round time is less than 0 we reload the scene. </summary>
    IEnumerator NextRound()
    {
        while (NextRoundTime > 0)
        {
            NextRoundTime -= Time.deltaTime;
            if (Message && NextRoundTime >= 1)
            {
                int counter = Mathf.RoundToInt(NextRoundTime);
                Message.text = "Next Round in... " + counter.ToString();
            }
            yield return null;
        }
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        yield return null;
    }

}
