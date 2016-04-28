using UnityEngine;
using System;

// Following sea tide occurance from NOAA: https://tidesandcurrents.noaa.gov/noaatidepredictions/NOAATidesFacade.jsp?Stationid=8723050
// For a better understanding of what tide looks like and wolfram alpha: 1 - 1.5 * cos( ((2.619278+.5pi)/8) x - 0.5*pi)
public class SeaLevelManager : MonoBehaviour {

    public GameObject water;

    private Transform water_Y_axis;    // water transformation properties (x, y, z)
    private GameTime gameTime;
    private System.Random rand;
    private const double WATER_HEIGHT = 3.3;
    private double frequency,amplitude;
    private double min,hour;
    private double tide_ft;             // Measurement in feet (convert to meter -> tide_mt = tide_ft * 0.3048)
                                        // Period (p): horizontal distance before the y-axis begins to repeat
                                        // Frequency (f): number of complete waves for a given time Period
                                        // p = 2pi/f

    void Awake () {
        gameTime = GameObject.Find("HUDGameTime").GetComponent<GameTime>();

        water_Y_axis = water.GetComponent<Transform>();
        rand = new System.Random();
        frequency = (2.619278 + 0.5 * Math.PI) / 8;
    }

	
	void Update ()
    {
        hour = (double)gameTime.GetHour();
        min = ((double)gameTime.GetMin() * 5.0 / 3.0) / 100.0;
        amplitude = 1.5 + rand.Next(-50, 50) / 100;
        tide_ft = 1 - amplitude * Math.Cos(frequency * (hour + min) - 0.5 * Math.PI);

        Vector3 pos = water_Y_axis.transform.position;
        pos.y = (float)(WATER_HEIGHT + tide_ft * 0.3048);  // meters per hour, hour = 0 , tide = 1
        water_Y_axis.transform.position = pos;
    }

    //void OnGUI()
    //{
    //    GUI.Label(new Rect(0, 250, 500, 500), "Tide " + tide_ft);
    //    //GUI.Label(new Rect(0, 275, 500, 500), "Minute: " + min);
    //    GUI.Label(new Rect(0, 300, 500, 500), "Hour: " + hour);
    //    GUI.Label(new Rect(0, 325, 500, 500), "Tide y: " + water_Y_axis.transform.position.y);
    //}
}

