using UnityEngine;
using System.Collections;

public class RotateRandom : MonoBehaviour {
    public Vector3 Angle;

	// Use this for initialization
	void Start () {
        float rnd = Random.Range(0, 360);
        transform.Rotate(new Vector3(Angle.x * rnd, Angle.y * rnd, Angle.z * rnd));
	}
}
