using UnityEngine;
using System.Collections;
using Photon;

public enum AnimationState { idle, amused }

[RequireComponent(typeof(PhotonView))]
public class Player : PunBehaviour {

    public AnimationState m_animState = AnimationState.idle;

    #region member variables

    private PhotonView m_pview;
    private Material m_mat;
    private NavMeshAgent m_nav;
    private Animator m_anim;

    public bool m_isSpy;

    #endregion

    void Start ()
    {
        m_pview = GetComponent<PhotonView>();
        m_nav = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();

        if (PhotonNetwork.isMasterClient)
            m_isSpy = true;
        else
            m_isSpy = false;

    }
	
	void Update ()
    {
        m_anim.SetFloat("speed", m_nav.velocity.magnitude);

	    if (!m_pview.isMine)
        {
            //set material to standard
            gameObject.layer = 0;
            return;
        }

        if (m_nav.destination != null)
        {
            if (Vector3.Distance(transform.position, m_nav.destination) < 1)
            {
                m_nav.Stop();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000))
            {
                if (hit.collider.tag == "ToSee")
                {
                    m_animState = AnimationState.amused;
                    m_nav.destination = hit.point;
                    m_nav.Resume();
                }
            }
        }
	}
}
