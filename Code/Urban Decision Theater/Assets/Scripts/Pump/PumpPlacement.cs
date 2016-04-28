using UnityEngine;
using System.Collections;
using System.IO;

public class PumpPlacement : MonoBehaviour {

	public float scrollSensitivity;
	private PlaceblePump placeblePump;
	private Transform currentPump;
	private bool hasPlaced;

	public LayerMask PumpsMask;
	private Camera camera;

	private PlaceblePump placeablePumpOld;

	//fileI'O
	public GameTime gm;
	StreamWriter fileWriter = null;
	void Start()
	{
		camera = GetComponent<Camera> ();
		string fileName = Application.persistentDataPath + "/local_decisions.txt";
		fileWriter = File.CreateText(fileName);
		gm = GameObject.Find ("HUDGameTime").GetComponent <GameTime> ();
	}
		
	// Update is called once per frame
	void Update () {
		//set the building to mouse position
		Vector3 m = Input.mousePosition;
		m = new Vector3(m.x,m.y,transform.position.y);
		Vector3 p = camera.ScreenToWorldPoint(m);

		if (currentPump != null && !hasPlaced) {

			currentPump.position = p;

			if (Input.GetMouseButtonDown(0)) {
				if (IsLegalPosition()) {
					hasPlaced = true;	
				}
			}
		}
		else {
			if (Input.GetMouseButtonDown(0)) {
				RaycastHit hit = new RaycastHit();
				Ray ray = new Ray(p, Vector3.down);
				if (Physics.Raycast(ray, out hit,Mathf.Infinity,PumpsMask)) {
					if (placeablePumpOld != null) {
						placeablePumpOld.SetSelected(false);
					}
					hit.collider.gameObject.GetComponent<PlaceblePump>().SetSelected(true);
					placeablePumpOld = hit.collider.gameObject.GetComponent<PlaceblePump>();
				}
				else {
					if (placeablePumpOld != null) {
						placeablePumpOld.SetSelected(false);
					}
				}
			}
	
	}
	}

	void OnApplicationQuit()
	{
		// when you are done writing
		fileWriter.Close();
	}

	bool IsLegalPosition()
		{
		if (placeblePump.colliders.Count > 0) {
			return false;
		}
		return true;
	}

	public void SetItem(GameObject p) {
		hasPlaced = false;
		currentPump = ((GameObject)Instantiate(p)).transform;
		placeblePump = currentPump.GetComponent<PlaceblePump>();
		write_to_file (p);
	}

	public void write_to_file(GameObject p)
	{
		string line = string.Format("{0}:{1} {2} {3},{4},{5}",gm.GetHour(), gm.GetMin(), p.tag.ToString(), p.transform.position.x.ToString(), p.transform.position.y.ToString(), p.transform.position.z.ToString());	
		fileWriter.WriteLine(line);	
	}

}
