using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public abstract class NPCAgent : MonoBehaviour, IGoap {

    #region member variables

    private Inventory m_inventory;
    private NavMeshAgent m_nav;
    private Animator m_anim;

    public Vector3 m_lookAtPosition;
    public int m_maxSatisfaction = 3;
    public AnimationState m_state = AnimationState.idle;

    #endregion

    void Start ()
    {
        m_inventory = new Inventory();
        m_nav = GetComponent<NavMeshAgent>();
        m_anim = GetComponent<Animator>();
	}
	
	void Update ()
    {
        //do animator stuff
        m_anim.SetFloat("speed", m_nav.velocity.magnitude);
        m_anim.SetInteger("state", (int)m_state);

        //increase/decrease values
        if (m_nav.velocity.magnitude < 0.1f && m_lookAtPosition != null)
            transform.LookAt(m_lookAtPosition);
    }

    //to be implemented in derived classes
    public abstract HashSet<KeyValuePair<string, object>> createGoalState();
    public abstract HashSet<KeyValuePair<string, object>> getWorldState();

    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        Debug.Log("<color=red>Plan found</color> Plan Failed! " + GoapAgent.prettyPrint(failedGoal));
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<Action> actions)
    {
        // write to the log the queue of actions found
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }
    public void actionsFinished()
    {
        Debug.Log("<color=blue>Actions queue empty</color>");
    }

    public void planAborted(Action aborter)
    {
        // log the action that made us fail
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(Action nextAction)
    {
        /**
         * Using the Navmesh here, but can be reimplemented using any navigation of choiche
         * just make sure that the system will include a SetDestination, Stop and Resume.
         */

        // move towards the NextAction's target
        Vector3 fullDis = transform.position - nextAction.GetTarget().transform.position;
        fullDis.y = transform.position.y;

        if (fullDis.magnitude < .3f)
        {
            // we are at the target location, we are done
            nextAction.SetInRange();
            m_nav.Stop();
            m_lookAtPosition.y = transform.position.y;
            transform.LookAt(m_lookAtPosition);
            return true;
        }
        else
        {
            m_nav.SetDestination(nextAction.GetTarget().transform.position);
            m_nav.Resume();
            return false;
        }
    }

    public void AddToInventory(string item, byte amount)
    {
        m_inventory.AddItem(item, amount);
    }

    public void RemoveFromInventory(string item, byte amount)
    {
        m_inventory.RemoveItem(item, amount);
    }

    void OnDestroy()
    {
        //see if we need to replenish the npc pool
        ClassicMode mode = FindObjectOfType<ClassicMode>() as ClassicMode;
        mode.ReplenishNPCPool();
    }
}
