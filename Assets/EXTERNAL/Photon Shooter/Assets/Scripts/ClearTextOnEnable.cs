using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary> 
/// Simple class to clear texts when the gameobject is enabled
/// </summary>
/// 
public class ClearTextOnEnable : MonoBehaviour {
    public Text[] Texts;

	void OnEnable () {
        foreach (Text t in Texts)
        {
            t.text = "";
        }
	}
	
}
