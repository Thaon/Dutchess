using UnityEngine;
using System.Collections;

/// <summary>
/// Simple Class to handle the team selection.
/// </summary>
/// 

public class SelecetTeam : MonoBehaviour {
    public PunTeams.Team Team;
	public void OnClickBtn () {
        PhotonNetwork.player.SetTeam(Team);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
