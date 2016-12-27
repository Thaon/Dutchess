using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointOfInterest : MonoBehaviour {

    #region member variables

    public GameObject m_lookAtPosition;

    [SerializeField]
    private List<GameObject> m_users;
    [SerializeField]
    private List<GameObject> m_blockedPositions;
    [SerializeField]
    private List<GameObject> m_availablePositions;
    private byte m_capacity;

    #endregion

    void Start ()
    {
        m_users = new List<GameObject>();
        m_availablePositions = new List<GameObject>();
        m_blockedPositions = m_availablePositions;

        foreach (Transform tr in GetComponentsInChildren<Transform>())
        {
            if (tr != transform)
                m_availablePositions.Add(tr.gameObject);
        }
        m_capacity = (byte)(m_availablePositions.Count - 1);
    }

    public bool AddUser(GameObject user)
    {
        if (m_users.Count < m_capacity)
        {
            m_users.Add(user);
            return true;
        }
        return false;
    }

    public void RemoveUser(GameObject user)
    {
        m_users.Remove(user);
    }

    public bool IsFull()
    {
        return m_users.Count == m_capacity;
    }

    //void OnTriggerExit(Collider other)
    //{
    //    print("exiting!");
    //    if (m_users.Contains(other.gameObject))
    //        m_users.Remove(other.gameObject);
    //}

    public bool IsUserInteracting(GameObject user)
    {
        return m_users.Contains(user);
    }

    public GameObject GetAvailablePosition()
    {
        if (m_availablePositions.Count > 0)
        {
            GameObject go = m_availablePositions[0];
            m_blockedPositions.Add(m_availablePositions[0]);
            m_availablePositions.RemoveAt(0);
            return go;
        }
        else
            return null;
    }

    public void RestorePosition(GameObject position)
    {
        if (m_blockedPositions.Contains(position))
        {
            m_blockedPositions.Remove(position);
            m_availablePositions.Add(position);
        }
        else
            print("the position was not in the blocked positions list");
    }
}
