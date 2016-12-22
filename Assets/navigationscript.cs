using UnityEngine;
using System.Collections;

public class navigationscript : MonoBehaviour {

    public GameObject target;


	void Start ()
    {
        GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
	}
	
	void Update () {
	
	}
}
