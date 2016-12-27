using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// This is the game manager base class. 
/// </summary>
 
public class GameManager : MonoBehaviour
{
    public GameObject Menu; // Reference to the In-Game menu. 
    protected GameObject PlayerObj; // Reference to the local player.
    public GameObject[] GameModes; // Reference to the game modes gameobjects. (We will instantiate the one we selected in the room menu).

    int GameModeSelected = 0; // Reference to the current game mode selected.
    public bool isInit = true; // This is used know if the game is started so we can only activate the In-Game menu when we are ready.

    /// <summary> Gets the custom property "gm" which contains the info about what game mode we have to instantiate. If we have the info we just instantiate the game mode selected. </summary>
    public virtual void Awake()
    {
        if (PhotonNetwork.room.customProperties.ContainsKey("gm"))
            GameModeSelected = (int)PhotonNetwork.room.customProperties["gm"];
        Instantiate(GameModes[GameModeSelected]);
    }

    /// <summary> Clears the "Players Kills". </summary>
    public virtual void Start()
    {
        if (PhotonNetwork.player.customProperties.ContainsKey("pk"))
            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "pk", 0 } });
    }

    public virtual void OnEnable()
    {

    }

    public virtual void OnDisable()
    {

    }

    /// <summary> If we press the escape key we activate the In-Game menu. Show and unlock the cursor and send a stop message to the payer. 
    /// If we press the key again and the menu is active we just do the opposite. </summary>
    public virtual void Update()
    {
        if (!isInit)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!Menu.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Menu.SetActive(true);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Menu.SetActive(false);
            }
        }
    }

    /// <summary> This is used to disable the player, camera movements and other inputs in it. </summary>
    public virtual void Stop()
    {
        Menu.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary> This is used to enable the player, camera movements and other inputs in them when we deactivate the In-Game menu. We also lock the cursor and hide it. </summary>
    public virtual void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary> If we want to exit the game we leave the room and we get back to the main menu. </summary>
    public virtual void OnLeaveRoom()
    {
        StartCoroutine(OnLeaveRoomCoroutine());
    }

    /// <summary> Leave room coroutine. </summary>
    IEnumerator OnLeaveRoomCoroutine()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.room != null)
            yield return 0;
        SceneManager.LoadScene(0);
    }

    void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
}
