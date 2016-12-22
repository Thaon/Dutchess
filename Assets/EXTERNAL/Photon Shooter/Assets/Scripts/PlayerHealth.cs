using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Extended class to handle the player health.
/// </summary>
/// 

public class PlayerHealth : Health {
    private GameObject AimGUI; // Reference to the AimGUI GameObect it contains (Aim, health, bullets etc..) UI.
    public GameObject ParticleEffect; // Particle effect for when we get hit.
    protected Image HealthBar; // Reference to the health bar image.
    private GameMode gMode; // Reference to the current Game Mode.

    /// <summary> Init the variables only if we are the owners. </summary>
    public override void Awake()
    {
        base.Awake();
        if (!m_PhotonView.isMine)
            return;
        if (AimGUI == null)
            AimGUI = GameObject.Find("Multiplayer").transform.Find("GameUI/AimGUI").gameObject;
        HealthBar = AimGUI.transform.FindChild("Health").GetComponent<Image>();
        UpdateHealth();
    }

    /// <summary> Update the health as soon as the gameobject is enabled. </summary>
    public virtual void OnEnable()
    {
        UpdateHealth();
    }

    /// <summary> Handles the damage. Subtracts the current health and clamps the values to the min and max values (in the base class).
    /// If the health is less or equal to 0 then we call the "OnDeath" function and pass the player who killed us. Show the particle effect if we have one.
    /// </summary>
    public override void Damage(float Val, PhotonPlayer other = null)
    {
        base.Damage(Val);
        if (m_Health <= 0)
            OnDeath(other);
        if (m_PhotonView.isMine)
            UpdateHealth();

        if (ParticleEffect)
            Instantiate(ParticleEffect, transform.position, Quaternion.identity);

        if(m_Health > 0)
            this.gameObject.SendMessage("ShowPlayerName");
    }

    /// <summary> Handles the on death event. Only execute the code if we have a reference to the player who killed us.
    /// The on death event is handled in the game mode class and we call the function only if we are the owners of the photonview.
    /// Then we show a message ie: "Player1 killed player2" in all clients</summary>
    protected override void OnDeath(PhotonPlayer other = null)
    {
        base.OnDeath();
        if (other != null)
        {
            transform.gameObject.SetActive(false);
            gMode = GameObject.FindObjectOfType(typeof(GameMode)) as GameMode;
            if (gMode && m_PhotonView.isMine)
                gMode.OnDeath(this.gameObject, other);
            Debug.Log(gMode);
            if (gMode)
                gMode.ShowMessage(other.name + " killed: " + m_PhotonView.owner.name + "\n");
            Debug.Log(other.name + " killed: " + m_PhotonView.owner.name + "\n");
        }

    }

    /// <summary> Update the health UI </summary>
    void UpdateHealth()
    {
        if (HealthBar)
        {
            HealthBar.fillAmount = m_Health / MaxHealth;
        }
    }
}
