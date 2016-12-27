using UnityEngine;
using System.Collections;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class Player : PunBehaviour {

    #region member variables

    private PhotonView m_pview;
    private Material m_mat;
    private NavMeshAgent m_agent;

    public bool m_isSpy;
    public Material m_standard;

    #endregion

    void Start ()
    {
        m_pview = GetComponent<PhotonView>();
        m_mat = GetComponent<Material>();
        m_agent = GetComponent<NavMeshAgent>();
        if (PhotonNetwork.isMasterClient)
            m_isSpy = true;
        else
            m_isSpy = false;

    }
	
	void Update ()
    {
	    if (!m_pview.isMine)
        {
            //set material to standard
            m_mat = m_standard;
            return;
        }

        if (m_agent.destination != null)
        {
            if (Vector3.Distance(transform.position, m_agent.destination) < 1)
            {
                m_agent.Stop();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000))
            {
                if (hit.collider.tag == "ToSee")
                {
                    m_agent.destination = hit.point;
                    m_agent.Resume();
                }
            }
        }
	}
}
