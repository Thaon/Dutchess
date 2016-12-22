using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script handles flag.
/// </summary>
/// 

public class Flag : MonoBehaviour {
    protected PhotonView m_PhotonView; // Reference to the flag photonview.
    Transform Target; // Reference to the target (If a player enters in the flag collider we set the target to that player).
    public int Points = 1; // The points you get after a period of time if we got the flag.
    public float PointTimeRate = 10.0f; // Rate time we get the points.
    bool isAddingScore = false; // It is used to know if we have to add points to the player.
    Vector3 FlagPos; // It is used to synchronize the position of the flag in all the clients.
    Quaternion FlagRot; // It is used to synchronize the rotation of the flag in all the clients.
    public GameObject MessagePref; // Message we used to show what player has the flag.
    private GameObject Message; //Reference to the message gameobject.

    private GameObject MultiplayerMenu; // Reference to the Multiplayer Menu.

    /// <summary> Init some variables. </summary>
    public void Start()
    {
        MultiplayerMenu = GameObject.Find("Multiplayer");
        m_PhotonView = GetComponent<PhotonView>();
        Message = Instantiate(MessagePref);
        Message.transform.SetParent(MultiplayerMenu.transform, false);
        Message.SetActive(false);
    }

    /// <summary> If a rigidbody enters inside the flag collider then we check if it is a player if so we set the target to that player and send a RPC to all the clients with the target. </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FlagTrigger(other.transform);
            m_PhotonView.RPC("FlagTriggerRPC", PhotonTargets.Others, other.GetComponent<PhotonView>().viewID);
        }
     }

    /// <summary> Sets the target. </summary>
    void FlagTrigger(Transform player)
    {
        Target = player;
        if(Message)
        {
            Message.SetActive(true);
            Message.GetComponent<Text>().text = "The player: " + player.GetComponent<PhotonView>().owner.name + " has the flag.";
        }
    }

    /// <summary> Detachs the Flag by setting the target to null. </summary>
    public void DetachFlag()
    {
        Target = null;
        if (Message)
        {
            Message.GetComponent<Text>().text = "";
            Message.SetActive(false);
        }
    }

    /// <summary> RPC to detach the flag in all the clients. </summary>
    [PunRPC]
    void DetachFlagRPC()
    {
        DetachFlag();
    }

    /// <summary> RPC to attach the flag in all the clients to a target. </summary>
    [PunRPC]
    void FlagTriggerRPC(int PlayerID)
    {
        PhotonView player = PhotonView.Find(PlayerID);
        if(player)
        FlagTrigger(player.transform);
    }

    /// <summary> If we have a target we just set the position and rotation of the flag to the target position and rotation and if we are the owners of the photonview we add the score. 
    /// If we don't have a target and we are not the owners of the photonview we just synchronize the flag position and rotation then stop all coroutines to stop adding the score. </summary>
    void LateUpdate()
    {
        if (Target != null)
        {
            this.transform.position = Target.position;
            this.transform.rotation = Quaternion.LookRotation(Target.forward);
            if(m_PhotonView.isMine)
            {
                PhotonView player = Target.GetComponent<PhotonView>();
                if(player && !isAddingScore)
                {
                    StartCoroutine(AddScore(player));
                }
            }
        }

        else
        {
            if (!m_PhotonView.isMine)
            {

                transform.position = FlagPos;
                transform.rotation = FlagRot;
            }
            if(isAddingScore)
            {
                StopAllCoroutines();
                isAddingScore = false;
            }
        }
    }

    /// <summary> Adds the score to the player that has the flag and sets it in the players custom porperty. We use the PointTimeRate to just add points every x period of time. </summary>
    IEnumerator AddScore(PhotonView _player)
    {
        isAddingScore = true;
        yield return new WaitForSeconds(PointTimeRate);
        if (_player.owner.customProperties.ContainsKey("pk"))
            _player.owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "pk", (int)_player.owner.customProperties["pk"] + 1 } });
        else
            _player.owner.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "pk", 1 } });
        isAddingScore = false;
    }

    /// <summary> This is used to synchronize the flag position and rotation, we receive and set the info in the local variables. </summary>
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            FlagPos = (Vector3)stream.ReceiveNext();
            FlagRot = (Quaternion)stream.ReceiveNext();
        }
    }

}
