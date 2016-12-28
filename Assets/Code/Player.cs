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
    private GameObject m_target;

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
        m_anim.SetInteger("state", (int)m_animState);
        if (m_target != null && m_nav.velocity.magnitude < 0.1f)
            transform.LookAt(m_target.transform.position);

        if (!m_pview.isMine)
        {
            //set material to standard
            gameObject.layer = 0;
            return;
        }

        //WE ARE THE SPY, LET'S DO SPY SHIT
        if (m_isSpy)
        {
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
                        m_target = hit.collider.gameObject;
                        m_nav.destination = hit.collider.GetComponent<PointOfInterest>().GetAvailablePosition().transform.position;
                        m_nav.Resume();
                    }
                }
            }
        }

        //WE ARE NOT THE SPY, LET'S GET THE FUCKER
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
            gameObject.layer = 0;

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 1000))
                {
                    if (hit.collider.tag == "Player")
                    {
                        //we have the spy, let's end the round
                        ClassicMode mode = FindObjectOfType<ClassicMode>() as ClassicMode;
                        mode.PoliceWins();
                    }
                }
            }
        }
	}
}
