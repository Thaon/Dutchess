using UnityEngine;
using System.Collections;
using Photon;

public class UnitNetworkSync : PunBehaviour {

    #region member variables

    private Player m_player;
    private NavMeshAgent m_nav;

    #endregion

    void Start ()
    {
        PhotonNetwork.sendRate = 10;
        PhotonNetwork.sendRateOnSerialize = 10;

        m_player = GetComponent<Player>();
        m_nav = GetComponent<NavMeshAgent>();
    }

    void Update ()
    {
	
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(m_nav.destination);
            stream.SendNext((int)m_player.m_animState);
        }
        else //TODO: Refactor this to include an authoritative server!!!
        {
            m_nav.SetDestination((Vector3)stream.ReceiveNext());
            m_nav.Resume();
            m_player.m_animState = (AnimationState)stream.ReceiveNext();
        }
    }
}
