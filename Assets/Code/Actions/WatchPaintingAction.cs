using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WatchPaintingAction : Action {

    #region member variables

    private bool m_completed = false;
    private float m_startingTime = 0;
    private GameObject m_lastVisited;

    public float m_timeToComplete;

    #endregion

    WatchPaintingAction()
    {
        AddPrecondition("amused", false);
        AddEffect("amused", true);
        SetCost(1);
        m_lastVisited = null;
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
        // find the nearest machine pile that has spare ores
        GameObject[] paintings = GameObject.FindGameObjectsWithTag("ToSee");
        GameObject nearest = null;
        float max = 0;

        List<GameObject> availablePaintings = new List<GameObject>();

        foreach (GameObject painting in paintings)
        {
            PointOfInterest poi = painting.GetComponent<PointOfInterest>();
            if (!poi.IsFull() && !poi.IsUserInteracting(this.gameObject) && poi.gameObject != m_lastVisited)
            {
                availablePaintings.Add(painting);
            }
        }
        
        //to be used when we have different actions
        //foreach (GameObject painting in availablePaintings)
        //{
        //    PointOfInterest poi = painting.GetComponent<PointOfInterest>();

        //    if (nearest == null)
        //    {
        //        nearest = painting;
        //        max = Vector3.Distance(painting.transform.position, transform.position);
        //    }
        //    else
        //    {
        //        float dist = Vector3.Distance(painting.transform.position, transform.position);
        //        if (dist < max)
        //        {
        //            nearest = painting;
        //            max = dist;
        //        }
        //    }
        //}
        nearest = availablePaintings[Random.Range(0, availablePaintings.Count - 1)];

        if (nearest == null)
            return false;

        //randomise time to complete
        m_timeToComplete += Random.Range(1, 5);

        SetTarget(nearest);
        m_lastVisited = nearest;
        nearest.GetComponent<PointOfInterest>().AddUser(this.gameObject);

        return nearest != null && GetComponent<TouristNPC>().m_satisfaction < GetComponent<TouristNPC>().m_maxSatisfaction;
    }

    public override bool Perform(GameObject agent)
    {
        // GetComponent<Animator>().SetBool("faffing", true);
        if (m_startingTime == 0)
            m_startingTime = Time.time;

        if (Time.time - m_startingTime > m_timeToComplete)
        {
            //GetComponent<Animator>().SetBool("faffing", false);
            m_target.GetComponent<PointOfInterest>().RemoveUser(this.gameObject);
            GetComponent<TouristNPC>().m_satisfaction++;
            m_completed = true;
        }
        return true;
    }
}
