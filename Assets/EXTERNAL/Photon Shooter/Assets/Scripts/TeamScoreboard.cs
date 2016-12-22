using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Simple Class to update the Scoreboard in a team match.
/// </summary>
/// 

public class TeamScoreboard : MonoBehaviour {
    public Text TeamBlue;
    public Text TeamRed;
	public void SetScore (int blue, int red) {
        if (TeamBlue)
            TeamBlue.text = blue.ToString();
        if (TeamRed)
            TeamRed.text = red.ToString();

    }
	

}
