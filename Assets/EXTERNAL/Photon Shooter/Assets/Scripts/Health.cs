using UnityEngine;
using System.Collections;
/// <summary>
/// Base class to handle the health.
/// </summary>
/// 

public class Health : MonoBehaviour {
    protected PhotonView m_PhotonView; // Reference to the photonview.
    public float MaxHealth = 100.0f; // Maximum health.
    public float m_Health = 0.0f; // Current Health.

    /// <summary> Init some variables. (Set the current health to max health) and get the photonview. </summary>
    public virtual void Awake()
    {
        m_Health = MaxHealth;
        m_PhotonView = transform.root.GetComponent<PhotonView>();
    }

    /// <summary> Handles the damage. Subtracts the current health and clamps the values to the min and max values. </summary>
    public virtual void Damage(float Val, PhotonPlayer other = null)
    {
        m_Health -= Val;
        m_Health = Mathf.Clamp(m_Health, 0, MaxHealth);
    }

    /// <summary> Remote procedure call to handle the damage in the other clients. </summary>
    [PunRPC]
    public void DamageRPC(float val, PhotonPlayer other = null)
    {
        Damage(val, other);
    }

    /// <summary> Function to handle the "OnDeath" event. </summary>
    protected virtual void OnDeath(PhotonPlayer other = null)
    {
   
    }

    /// <summary> In case we need to reset the health. </summary>
    public virtual void Reset()
    {
        m_Health = MaxHealth;
    }

}
