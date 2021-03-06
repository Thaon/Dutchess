﻿using UnityEngine;
using System.Collections;

public class FaffAboutAction : Action {

    #region member variables

    private bool m_completed = false;
    private float m_startingTime = 0;

    public float m_timeToComplete;

    #endregion

    FaffAboutAction()
    {
        AddPrecondition("hungry", false);
        SetCost(5);
    }

    public override void Reset()
    {
        m_completed = false;
        m_startingTime = 0;
    }

    public override bool IsDone()
    {
        return m_completed;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        // we can always faff about
        return true;
    }

    public override bool Perform(GameObject agent)
    {
       // GetComponent<Animator>().SetBool("faffing", true);
        if (m_startingTime == 0)
            m_startingTime = Time.time;

        if (Time.time - m_startingTime > m_timeToComplete)
        {
            //GetComponent<Animator>().SetBool("faffing", false);
            m_completed = true;
        }
        return true;
    }
}
