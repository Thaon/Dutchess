using UnityEngine;
using System.Collections;

/// <summary>
/// Simple class to handle the kick player button pressed.
/// </summary>
/// 

public class Kick : MonoBehaviour {
    public PhotonPlayer Player;

    public void OnKickPlayer()
    {
        PhotonNetwork.CloseConnection(Player);
    }
}
