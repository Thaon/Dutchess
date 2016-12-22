using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple class to activate the menu In-Game so you can leave a match or see the player list.
/// </summary>
/// 


public class InGameMenu : MonoBehaviour {
    public GameObject Menu;

    /// <summary> If we press escape we activate the menu. </summary>
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.GetActive());
        }
    }

    /// <summary> Function to leave the match. We just leave the room and get back to the main menu. </summary>
    public void QuitGame()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
