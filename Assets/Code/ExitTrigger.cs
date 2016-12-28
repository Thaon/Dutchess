using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NPC")
            Destroy(other.gameObject);
	}
}
