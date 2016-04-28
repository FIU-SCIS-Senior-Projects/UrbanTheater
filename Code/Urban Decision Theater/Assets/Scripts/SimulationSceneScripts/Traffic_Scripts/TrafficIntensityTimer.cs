using UnityEngine;
using System;
using System.Timers;

public class TrafficIntensityTimer
{
    const int MAX_TIME = 6000;
    const int MIN_TIME = 2000;
    const double FREQUENCY = (2.619278 + 0.5 * Math.PI) / 4.0;
    int trafficIntensity;
    int initTrafficIntensity = -1;
    public static GameTime gameTime;
    System.Random randNumber;
    Timer myTimer;
    Node node;

    public TrafficIntensityTimer(Node _node)
    {
        randNumber = new System.Random();
        myTimer = new Timer();
        myTimer.Elapsed += OnTimedEvent;        // Hook up the Elapsed event for the timer.
        node = _node;
        trafficIntensity = 0;
    }

    public int IncreaseTraffic()
    {
        float capTime = 0;  // b/w 0.0 - 1.0
        lock (node)
        {
            if (TrafficIntensity > 0)
                TrafficIntensity--;
            capTime = ((float)InitialTrafficIntensity - (float)TrafficIntensity) / (float)InitialTrafficIntensity;
            BeginTimer();
        }
        
        return TrafficTimeOfDay(capTime);
    }

    // Cosine function is b/w 0.1 - 1.1 (using FREQUENCY)
    // Other Cosine is b/w 0.1 - 0.8
    int TrafficTimeOfDay(float capTime)
    {
        double hour = gameTime.GetHour();
        double min = (gameTime.GetMin() * 5.0 / 3.0) / 100.0;
        double frequency = hour >= 18.0 || hour < 6 ? (1.0 / 8.0) : FREQUENCY;
        double amTime = hour >= 18.0 || hour < 6 ? 100.0 : 1.0;
        return (int)(capTime * (0.6 - 0.5 * Math.Cos(frequency * (hour + min) - 2.0)) * (amTime * 100) + 20);
    }

    // Elapsed event is raised if the Enabled property is true and the time interval (in milliseconds) defined by the Interval property elapses.
    void OnTimedEvent(object soruce, ElapsedEventArgs e)
    {
        lock (node)
        {
            if (trafficIntensity < initTrafficIntensity)
                trafficIntensity++;

            if (trafficIntensity < InitialTrafficIntensity)
            {
                myTimer.Interval = randNumber.Next(MIN_TIME, MAX_TIME);
                myTimer.Start();
            }
        }
    }

    public void BeginTimer()
    {
        lock (myTimer)
        {
            if (myTimer.Enabled)
                myTimer.Stop();
            myTimer.Interval = randNumber.Next(MIN_TIME, MAX_TIME);
            myTimer.Start();
        }
    }

    public void StopTimer()
    {
        lock (myTimer)
        {
            if (myTimer.Enabled)
                myTimer.Stop();
        }
    }

    public void SetInitWeight()
    {
        if (initTrafficIntensity == -1)
            initTrafficIntensity = trafficIntensity;
    }

    public void DisposeTimer()
    {
        myTimer.Dispose();
    }

    public int TrafficIntensity
    {
        get
        {
            return trafficIntensity;
        }
        set
        {
            trafficIntensity = value;
        }
    }

    public int InitialTrafficIntensity
    {
        get
        {
            return initTrafficIntensity;
        }
    }

}
