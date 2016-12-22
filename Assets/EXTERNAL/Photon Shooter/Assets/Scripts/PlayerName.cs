using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Simple class to show the players name on top of it. Also checks the players team and sets the color according to it.
/// </summary>

public class PlayerName : MonoBehaviour
{
    protected PhotonView m_PhotonView; // Reference to the player photonview.
    private GameObject MultiplayerMenu; // Reference to the Multiplayer Menu.
    Transform Player; //Reference to the player
    public GameObject TextPref; // Message we used to show what player has the flag.
    private GameObject PlayeNameObj; //Reference to the message gameobject.
    public bool isStarted = false; // This is used to know if we are ready. 
    Transform myTransform; //Reference to the text transform.
    public float TimeToShowName = 3.0f;

    void Start()
    {
        Player = this.transform;
        MultiplayerMenu = GameObject.Find("Canvas");
        m_PhotonView = GetComponent<PhotonView>();
        PlayeNameObj = Instantiate(TextPref);
        PlayeNameObj.transform.SetParent(MultiplayerMenu.transform, false);

        myTransform = PlayeNameObj.transform;

        if (PlayeNameObj)
        {
            PlayeNameObj.GetComponentInChildren<Text>().text = m_PhotonView.owner.name;

            if(m_PhotonView.owner.GetTeam() == PunTeams.Team.blue)
            {
                PlayeNameObj.GetComponentInChildren<Text>().color = Color.blue;
            }

            if (m_PhotonView.owner.GetTeam() == PunTeams.Team.red)
            {
                PlayeNameObj.GetComponentInChildren<Text>().color = Color.red;
            }

        }
    }

    void ShowPlayerName()
    {
        if (isStarted)
            return;
        isStarted = true;
        CancelInvoke("HidePlayerName");
        Invoke("HidePlayerName", TimeToShowName);
        PlayeNameObj.GetComponentInChildren<Text>().text = m_PhotonView.owner.name;
    }

    void HidePlayerName()
    {
        PlayeNameObj.GetComponentInChildren<Text>().text = "";
        isStarted = false;
    }

    void OnDisable()
    {
        Destroy(PlayeNameObj);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PhotonView.isMine)
            return;

        if (Player != null && isStarted)
        {
            Vector3 targetPos = Player.position + new Vector3 (0.0f, 2.0f, 0.0f);
            if (!float.IsInfinity(targetPos.x) && !float.IsInfinity(targetPos.y) && !float.IsInfinity(targetPos.z))
                myTransform.position = targetPos;
            if(Camera.main)
            myTransform.LookAt(Camera.main.transform);

        }
    }
}