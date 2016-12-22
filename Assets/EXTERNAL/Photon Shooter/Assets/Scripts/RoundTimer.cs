using UnityEngine;
using System.Collections;
using UnityEngine.UI;
/// <summary>
/// This script handles the Round Timer.
/// </summary>

public class RoundTimer : MonoBehaviour {
    public int TimePerRound = 5; // Reference to the Time per round;                
    private double StartTime; // It is used to set the start time 
    private const string StartTimeKey = "st"; // Key we use to set and get the start time custom property.

    public Text m_Timer; // Reference to the timer text.
    public bool isRunning = false; // It is used to know if the timer is started.
    public bool TimesUp = false; // It is used to know if the time is up.

    /// <summary> Gets the room Match Time custom property and updates the Time Per Round (minutes) then multiplies it by 60 sec </summary>
    void Awake () {
        if (PhotonNetwork.room.customProperties.ContainsKey("mt"))
            TimePerRound = (int)PhotonNetwork.room.customProperties["mt"];
        TimePerRound *= 60;
    }

    public void StartRound()
    {
        StartCoroutine(StartRoundCoroutine());
    }

    /// <summary> Start the round Coroutine if we are the master client or else we wait to get the Start time value from the room custom property "st" then we update the StartTime and set isRunning to true </summary>
    public IEnumerator StartRoundCoroutine()
    {
        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(StartRoundNow());
            isRunning = true;
        }

        else
        {
            while (!PhotonNetwork.room.customProperties.ContainsKey(StartTimeKey))
            {
                yield return null;
            }
            StartTime = (double)PhotonNetwork.room.customProperties[StartTimeKey];
            isRunning = true;
        }

        yield return null;
    }

    /// <summary> Sets isRunning to false so we can know the timer is not running </summary>
    public void StopRound()
    {
        isRunning = false;
    }

    /// <summary> Waits until the server time is available then sets the Room custom property "st" with the start time </summary>
    IEnumerator StartRoundNow()
    {
        // In some cases, when you enter a room, the server time is not available immediately.
        // time should be 0.0f but to make sure we detect it correctly, check for a very low value.
        while (PhotonNetwork.time < 0.0001f)
        {
            yield return null;
        }

        /// <summary> Sets the Room custom property "st" with the start time </summary>
        ExitGames.Client.Photon.Hashtable startTimeProp = new ExitGames.Client.Photon.Hashtable();  
        startTimeProp[StartTimeKey] = PhotonNetwork.time;
        PhotonNetwork.room.SetCustomProperties(startTimeProp);
        yield return null;
    }

    /// <summary> Check if the custom property "st" changed and if so we update the StartTime value </summary>
    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(StartTimeKey))
        {
            StartTime = (double)propertiesThatChanged[StartTimeKey];
        }
    }

    /// <summary> If the Master client leaves the room then we start the timer again. </summary>
    public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (!PhotonNetwork.room.customProperties.ContainsKey(StartTimeKey))
        {
            StartCoroutine(StartRoundNow());
        }
    }

    /// <summary> If we are the master client and the timer is not started yet we set the timer text to an empty string "" then if the time is not up we update the timer and the timer text. </summary>
    void LateUpdate () {
        if (PhotonNetwork.isMasterClient && !isRunning)
        {
            m_Timer.text = "";
            return;
        }

        if (m_Timer && !TimesUp) // Update the time only if TimesUp is equal to false (if the remaining time is less or equal to .1f the the time is up)
        {
            double elapsedTime = (PhotonNetwork.time - StartTime);
            double remainingTime = TimePerRound - (elapsedTime % TimePerRound);
            if (remainingTime <= 0.1f)
                TimesUp = true;
            int minutes = Mathf.FloorToInt((float)remainingTime / 60F);
            int seconds = Mathf.FloorToInt((float)remainingTime - minutes * 60);
            string Time = string.Format("{0:00}:{1:00}", minutes, seconds);
            m_Timer.text = Time;
        }
    }
}
