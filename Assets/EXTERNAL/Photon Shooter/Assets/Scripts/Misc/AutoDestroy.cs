using UnityEngine;
using System.Collections;

/// <summary>
/// Simple Class to destroy a gameobject after a period of time.
/// </summary>

public class AutoDestroy : MonoBehaviour {
    public float TimeOut = 1.0f;
	// Use this for initialization
	void Start () {
        Invoke("DeleteMe", TimeOut);
	}
	
    void DeleteMe()
    {
        Destroy(this.gameObject);
    }
}
