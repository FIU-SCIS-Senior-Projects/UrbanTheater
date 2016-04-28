using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    private double gameTimeSeconds;     // real time 1 sec -> game time 1440 sec = 24 min = 0.4 hr
                                        // real time 1 min -> game time 86400 sec = 24 hr
    private System.DateTime gameTime;
    private Text hudTime;

    void Start()
    {
        hudTime = GameObject.Find("TimeText").GetComponent<Text>();
        gameTime = System.DateTime.Now;
        gameTimeSeconds = 1440;     
    }

    // Update the time by adding 1440 seconds.
    void Update()
    {
        hudTime.text = gameTime.ToString("MM/dd/yyyy    h:mm tt");
        try
        {
            gameTime = gameTime.AddSeconds(gameTimeSeconds * Time.deltaTime);
        }
        catch (System.ArgumentOutOfRangeException)
        {
            // only if( parameter_value > MaxTicks - current_ticks || parameter_value < MinTicks - current_ticks )
            hudTime.text = "Time Boundary Exception";
            Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }

    public int GetHour()
    {
        return gameTime.Hour;
    }

    public double GetMin()
    {
        return gameTime.Minute;
    }

    void OnGUI()
    {
    //    //GUI.Label(new Rect(0, 200, 500, 500), "Total Seconds: " + totalGameSeconds);
    //    GUI.Label(new Rect(0, 225, 500, 500), "Seconds Per Second: " + secondsPerSecond);

    //    GUI.Label(new Rect(0, 250, 500, 500), "Second: " + (int)gameTime.Second);
    //    GUI.Label(new Rect(0, 275, 500, 500), "Minute: " + (int)gameTime.Minute);
    //    GUI.Label(new Rect(0, 300, 500, 500), "Hour: " + (int)gameTime.Hour % 24);
    //    GUI.Label(new Rect(0, 325, 500, 500), "Day: " + (int)gameTime.Day);
    //    GUI.Label(new Rect(0, 350, 500, 500), "Month: " + (int)gameTime.Month);
    //    GUI.Label(new Rect(0, 375, 500, 500), "Year: " + ((double)gameTime.Minute * 5.0 / 3.0)/100);
    }
}

