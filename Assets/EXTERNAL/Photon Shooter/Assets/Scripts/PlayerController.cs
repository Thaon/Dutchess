using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the Player controller.
/// </summary>
/// 
public class PlayerController : MonoBehaviour {
    private CharacterController Controller; // Reference to the player controller.
    protected Animator animator; // Reference to the player animator.
    protected PhotonView m_PhotonView; //Reference to the player photon view.

    Transform tCamera; // Reference to the third person camera transform.
    public float Speed = 10.0f; // The player speed.
    private Vector3 MoveDirection = Vector3.zero; // The move direction (It is used to move the player). 
    public float Gravity = 9.8f; // Reference to the gravity.
    public float MinMovementVal = 0.2f; // Min movement value (This is used to handle the movement sensitivity)

    public int CurrentAnimState = 0; // Reference to the current animation state.
    Transform myTransform; // Reference to the player transform.

    public float RotSpeed = .5f; // Reference to the player rotation speed.
    public bool isStarted = false; // This is used to know if we are ready. 
    public float jumpSpeed = 8.0F; // Reference to the jump speed.

    // Use this for initialization
    void Start () {
        Controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        m_PhotonView = GetComponent<PhotonView>();
        tCamera = Camera.main.transform;
        myTransform = transform;
        if (!m_PhotonView.isMine)
            this.enabled = false;
    }

    /// <summary> We are ready to move. This use called from other scripts like the game manager to allow the player movements after we exit the In-Game menu. </summary>
    public void Init()
    {
        isStarted = true;
    }

    /// <summary> We are not ready to move. This use called from other scripts like the game manager to prevent the player movements when we enter the In-Game menu. </summary>
    public void Stop()
    {
        isStarted = false;
    }


    /// <summary> Only moves the player if we are the owners of the photonview.  </summary>
    void Update () {
   
        if (m_PhotonView.isMine == false) //If we are not the owners of the photonview we just exit the function.
            return;

        /// <summary> Gets the directions we need to calculate the player movements then we apply the Horizontal and Vertical input to determine the movement direction. 
        /// Also if we pressed the jump button we add the jumpspeed to it. </summary>
        Vector3 forward = myTransform.forward;
        forward.y = 0;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        float h = 0;
        float v = 0;

        if(isStarted)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
        }

        if (Controller.isGrounded)
        {
            MoveDirection = (h * right + v * forward);
            MoveDirection *= Speed;
              
            
            if (Input.GetButton("Jump") && isStarted)
            {
                MoveDirection.y = jumpSpeed;
            }
        }

        MoveDirection.y -= Gravity * Time.deltaTime; //Adds some gravity.
        Controller.Move(MoveDirection * Time.deltaTime); //Moves the player.

        /// <summary> If the player is moving we set the animation to "1" which should be the "Walk" animation. </summary>
        float vel = new Vector3(Controller.velocity.x, 0.0f, Controller.velocity.z).magnitude;
            if (vel > MinMovementVal)
            {
                CurrentAnimState = 1;
            }
            else
                CurrentAnimState = 0;

        /// <summary> Sets the player rotation to the camera forward direction and removes the y axis from that direction (we only want to rotate the payer in the x and z axis). </summary>
        Vector3 LookVector = tCamera.transform.forward;
        LookVector.y = 0.0f;
        if (LookVector != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookVector), RotSpeed);

        animator.SetInteger("AnimState", CurrentAnimState); //Sets the current animation state.
    }
}
