using UnityEngine;
using System.Collections;

public class LeaveAreaAction : Action {

    #region member variables

    private bool m_completed = false;
    private float m_startingTime = 0;

    public float m_timeToComplete;

    #endregion

    LeaveAreaAction()
    {
        AddEffect("satisfied", true);
        AddPrecondition("satisfied", false);
        SetCost(5);
    }

    public override void Reset()
    {
        SetTarget(null);
        m_completed = false;
        m_startingTime = 0;
    }

    public override bool IsDone()
    {
        return m_completed;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        if (GetComponent<TouristNPC>().m_satisfaction >= GetComponent<TouristNPC>().m_maxSatisfaction)
        {
            SetTarget(GameObject.FindWithTag("Exit"));
            return true;
        }
        return false;
    }

    public override bool Perform(GameObject agent)
    {
        return true;
    }
}
