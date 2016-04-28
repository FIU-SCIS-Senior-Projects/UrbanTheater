using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceblePump : MonoBehaviour {

	[HideInInspector]
	public List<Collider> colliders = new List<Collider>();
	private bool isSelected;
	public string pName;

	void OnGUI() {
		if (isSelected) {
			GUI.Button(new Rect(Screen.width /2, Screen.height / 20, 100, 30), pName);
		}

	}

	void OnTriggerEnter(Collider c) {
		if (c.tag == "pump") {
			colliders.Add(c);	
		}
	}

	void OnTriggerExit(Collider c) {
		if (c.tag == "pump") {
			colliders.Remove(c);	
		}
	}

	public void SetSelected(bool s) {
		isSelected = s;	
	}


}