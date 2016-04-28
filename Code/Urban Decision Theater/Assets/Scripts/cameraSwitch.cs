using UnityEngine;
using System.Collections;

public class cameraSwitch : MonoBehaviour {
	public Camera firstPersonCamera;
	public Camera overheadCamera;
	public bool flag;

	public void ShowOverheadView() {
		firstPersonCamera.enabled = false;
		overheadCamera.enabled = true;
		flag = false;
	}

	public void ShowFirstPersonView() {
		firstPersonCamera.enabled = true;
		overheadCamera.enabled = false;
		flag = true;
	}
	// Use this for initialization
	void Start () {
		ShowFirstPersonView ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.B)) {
			if (flag)
				ShowOverheadView ();
			else
				ShowFirstPersonView ();
		}
	
	}
}
