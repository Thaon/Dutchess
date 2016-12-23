using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(Animator))]
public abstract class NPCAgent : MonoBehaviour, IGoap {

    #region member variables

    private Inventory m_inventory;
    private NavMeshAgent m_nav;
    private Animator m_anim;

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

        //increase/decrease values
	}

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldState = new HashSet<KeyValuePair<string, object>>();

        /**
         * add actions like this:
         * 
         *   if (GetComponent<EatAction>() != null)
         *      {
         *          worldState.Add(new KeyValuePair<string, object>("hungry", GetComponent<EatAction>().IsHungry()));
         *      }
         */
        /**
         * check inventory if needed like this:
         * 
         * worldState.Add(new KeyValuePair<string, object>("hasIngredients", m_inventory.Contains("ingredients")));
         */

        return worldState;
    }

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
        fullDis.y = 0;
        if (fullDis.magnitude <= 1)
        {
            // we are at the target location, we are done
            nextAction.SetInRange();
            m_nav.Stop();
            return true;
        }
        else
        {
            m_nav.SetDestination(nextAction.GetTarget().transform.position);
            m_nav.Resume();
            return false;
        }
    }

    //to be implemented in derived classes
    public abstract HashSet<KeyValuePair<string, object>> createGoalState();

    public void AddToInventory(string item, byte amount)
    {
        m_inventory.AddItem(item, amount);
    }

    public void RemoveFromInventory(string item, byte amount)
    {
        m_inventory.RemoveItem(item, amount);
    }
}
