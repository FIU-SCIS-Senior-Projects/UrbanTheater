using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using System.Collections;

public class DisplayTraffic : MonoBehaviour
{
    bool isUpdated;
    int width;
    int height;

    public Grid grid;
    public Image trafficImage;
    public GameObject trafficDisplayPanel;

    Timer myTimer;
    Texture2D texture;
    Color[] textureColor;

    void Start()
    {
        isUpdated = false;
        width = grid.GridSizeX;
        height = grid.GridSizeY;

        texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        textureColor = new Color[width * height];

        myTimer = new Timer(1500);
        myTimer.AutoReset = true;
        myTimer.Elapsed += OnTimedEvent;        // Hook up the Elapsed event for the timer.

        StartCoroutine(DrawTexture());  // Initial draw
        myTimer.Start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            if (!trafficDisplayPanel.activeSelf)
                trafficDisplayPanel.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            trafficDisplayPanel.SetActive(false);
        }

        if (isUpdated)
        {
            texture.SetPixels(textureColor);
            texture.Apply();

            trafficImage.material.mainTexture = texture;
            isUpdated = false;
        }
    }

    IEnumerator DrawTexture()
    {
        foreach (Node n in grid.Nodes)
        {
            textureColor[n.gridX + n.gridY * width] = Color.white;
            if (n.IsRoad)
            {
                float initWeight = n.trafficTimer.InitialTrafficIntensity;
                float x = Mathf.Clamp(n.trafficTimer.TrafficIntensity, 0, initWeight);

                float red = (initWeight - x) / initWeight;
                float green = x / initWeight;

                textureColor[n.gridX + n.gridY * width] = Color.red * red + Color.green * green;
            }
        }
        texture.SetPixels(textureColor);
        texture.Apply();

        trafficImage.material.mainTexture = texture;

        yield return null;
    }

    // Elapsed event is raised if the Enabled property is true and the time interval (in milliseconds) defined by the Interval property elapses.
    void OnTimedEvent(object soruce, ElapsedEventArgs e)
    {
        float initWeight, trafficIntensity, red, green;
        foreach (Node n in grid.RoadSet)
        {
            lock (n)
            {
                initWeight = n.trafficTimer.InitialTrafficIntensity;
                trafficIntensity = Mathf.Clamp(n.trafficTimer.TrafficIntensity, 0, initWeight);
            }

            red = (initWeight - trafficIntensity) / initWeight;
            green = trafficIntensity / initWeight;

            if(trafficIntensity == 0)
                textureColor[n.gridX + n.gridY * width] = Color.blue;
            else
                textureColor[n.gridX + n.gridY * width] = Color.red * red + Color.green * green;
                
        }
        isUpdated = true;
    }

    void OnApplicationQuit()
    {
        myTimer.Dispose();
    }
}
