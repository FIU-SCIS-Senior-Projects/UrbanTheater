using UnityEngine;
using UnityEngine.UI;

public class DynamicIntegrationTest : MonoBehaviour
{
	public GameObject sl;

	public void Start()
	{
		sl = GameObject.Find ("Budget Slider");
		//IntegrationTest.Pass(sl);

	}
	public void Update()
	{
		sl.GetComponent <Slider> ().value +=20;
	}
}
