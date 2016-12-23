using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointOfInterest : MonoBehaviour {

    #region member variables

    public byte m_capacity;

    [SerializeField]
    private List<GameObject> m_users;

    #endregion

    void Start ()
    {
        m_users = new List<GameObject>();
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
}
