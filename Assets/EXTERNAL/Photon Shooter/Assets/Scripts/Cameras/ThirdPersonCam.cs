using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the Third Person Camera
/// </summary>

public class ThirdPersonCam : MonoBehaviour {
    public float SmoothTime = 0.8f; // It is used to smooth the camera movements.
    Transform targetTransform; // Store the player transform.
    Transform CamTransform; // Store the camera transform.
    public float Distance = 10.0f; // It is used to set the distance from the camera to the player.
    private Vector2 inputVal = Vector2.zero; // Used to store the input values.

    public float MaxYAngle = 60.0f; // Limit the vertical rotation.
    public float MinYAngle = 0.0f; // Limit the vertical rotation.
    public Vector3 Offset; // Offset for the camera position.
    public Vector3 LookAtOffset; // Offset for the Player position.
    protected PhotonView m_PhotonView; // Used to store the player photonview.
    Camera m_Camera; // It is used to store the camera.
    bool isStarted = false; // It is used to check if the camera is started.

    /// <summary> Store the Transform. Disable the script if we are not the owners. </summary>
    void Awake()
    {
        targetTransform = this.transform;
        m_PhotonView = gameObject.GetComponent<PhotonView>();
        if (!m_PhotonView.isMine)
            this.enabled = false;
        m_Camera = Camera.main;
    }

    /// <summary> Fuction to get the camera if we need it </summary>
    public Transform GetCamera()
    {
        return CamTransform;
    }

    /// <summary> Function to init the camera. If there is a main camera and we are the owners the we store the camera transform and attach the camera to the player. </summary>
    public void Init()
    {
        if(m_Camera && m_PhotonView.isMine)
        {
            CamTransform = m_Camera.transform;
            CamTransform.parent = this.transform;
        }
        isStarted = true;
    }

    /// <summary> Function to stop the camera. We detach it from the player. </summary>
    public void Stop()
    {
        if (m_Camera && m_PhotonView.isMine)
        {
            CamTransform.parent = null;
        }
        isStarted = false;
    }

    /// <summary> Get the input to rotate the camera and clamp the values to the min and max for the y axis. </summary>
    void Update()
    {
        if (!isStarted)
            return;
        inputVal.x += Input.GetAxis("Mouse X");
        inputVal.y += -Input.GetAxis("Mouse Y");

        inputVal.y = Mathf.Clamp(inputVal.y, MinYAngle, MaxYAngle);
    }

    /// <summary> Sets the position and rotation. We also check with a ray cast to adjust the camera position </summary>
	void LateUpdate () {
        if (!isStarted)
            return;
        Vector3 Dir = new Vector3(0.0f, 0.0f, -(Distance));
        Quaternion rotation = Quaternion.Euler(inputVal.y, inputVal.x, 0);
        CamTransform.position = Vector3.Slerp(CamTransform.position, (targetTransform.position + Offset) + rotation * Dir, SmoothTime);
        CamTransform.LookAt(targetTransform.position + LookAtOffset);
        RayCastCamera();
    }

    /// <summary> check with a ray cast to adjust the camera position </summary>
    void RayCastCamera()
    {
        RaycastHit hit;
        Vector3 dir = CamTransform.forward;
        Vector3 StartPoint = CamTransform.position + (dir * Distance);
        if (Physics.Raycast(StartPoint, -dir, out hit, Distance * 1.2f))
        {
            Debug.DrawLine(StartPoint, hit.point, Color.red);
            CamTransform.position = hit.point + new Vector3(0.0f, 0.0f, 0.0f) + (CamTransform.forward * .5f);
        }
    }
}
