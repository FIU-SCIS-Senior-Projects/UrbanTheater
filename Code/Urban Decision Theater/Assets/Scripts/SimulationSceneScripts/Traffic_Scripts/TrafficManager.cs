using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;

public class TrafficManager : MonoBehaviour {

    const int PATH_PRODUCER_LIMIT = 100;
    const int PATH_CONSUMER_LIMIT = 40;

    bool poison;
    Grid grid;
    Thread producerPath_Thread;
    PathRequestManager requestManager;
    Queue<Stack<Node>> consumePathQ;
    TrafficGenerator[] consumerPath_Threads;

    void Start ()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
        consumePathQ = new Queue<Stack<Node>>();
        TrafficIntensityTimer.gameTime = GameObject.Find("HUDGameTime").GetComponent<GameTime>();
        poison = false;

        ModifiedBFS();
        initProducer();
        initConsumers();
    }

    void OnApplicationQuit()
    {
        poison = true;
        if (producerPath_Thread != null)
            producerPath_Thread.Join();

        TrafficGenerator.Poison = poison;
        if (consumePathQ != null)
        {
            lock (consumePathQ)
            {
                if (consumePathQ.Count == 0)
                {
                    consumePathQ.Enqueue(null);
                    Monitor.PulseAll(consumePathQ);
                }
            }
        }
        if (consumerPath_Threads != null)
        {
            for (int i = 0; i < PATH_CONSUMER_LIMIT; i++)
            {
                if (consumerPath_Threads[i] != null)         // quit before all threads are created. check if null
                {
                    consumerPath_Threads[i].Join();
                }
            }
        }

        foreach (Node n in grid.RoadSet)
            n.trafficTimer.DisposeTimer();
        //Resources.UnloadUnusedAssets();

    }

    void ModifiedBFS()
    {
        HashSet<Node> tempRoadSet = new HashSet<Node>(grid.RoadSet);
        Queue<Node> trafficQ = new Queue<Node>();
        trafficQ.Enqueue(grid.ARoadNode);

        while (tempRoadSet.Count != 0)
        {
            while (trafficQ.Count > 0)
            {
                Node current = trafficQ.Dequeue();
                if (tempRoadSet.Remove(current))
                {
                    foreach (Node n in grid.BFSGetRoadNeighbours(current))     // GetRoadNeighbors adds in/out => trafficWeight
                    {
                        if(tempRoadSet.Contains(n))
                            trafficQ.Enqueue(n);
                    }
                }
            }

            // Stranded roads
            // There maybe a chance that we take the small set. Need to improve this more.
            while (tempRoadSet.Count > 0)
            {
                HashSet<Node>.Enumerator node = tempRoadSet.GetEnumerator();
                if (node.MoveNext())
                {
                    grid.RoadSet.Remove(node.Current);     // remove stranded road
                    tempRoadSet.Remove(node.Current);
                    node.Current.IsRoad = false;
                }
            }
        }
    }
    
    void initProducer()
    {
        producerPath_Thread = new Thread(this.ProducePaths);
        producerPath_Thread.Start();
    }

    void ProducePaths()
    {
        bool qCount;
        int start, target;
        Node startNode, targetNode;
        System.Random randomNum = new System.Random();
        Node[] roadSetCopy = new Node[grid.RoadSet.Count];
        grid.RoadSet.CopyTo(roadSetCopy);

        while (!poison)
        {
            lock (consumePathQ)
                qCount = consumePathQ.Count < PATH_PRODUCER_LIMIT;
            
            if (qCount)
            {
                try
                {
                    int selector = randomNum.Next(0, 5);
                    if(selector >= 2)
                    {
                        start = randomNum.Next(0, grid.EdgeSet.Count);
                        startNode = (Node)grid.EdgeSet[start];
                        target = randomNum.Next(0, grid.EdgeSet.Count);
                        targetNode = (Node)grid.EdgeSet[target];
                    }
                    else
                    {
                        start = randomNum.Next(0, roadSetCopy.Length);
                        startNode = roadSetCopy[start];
                        target = randomNum.Next(0, roadSetCopy.Length);
                        targetNode = roadSetCopy[target];
                    }

                    while (true)
                    {
                        if (start == target)
                        {
                            target = randomNum.Next(0, roadSetCopy.Length);
                            targetNode = roadSetCopy[target];
                        }
                        else
                            break;
                    }
                    PathRequestManager.RequestPath(startNode, targetNode, OnPathFound);
                }
                catch (Exception e) { Debug.Log(e.Message); }
            }
            else
            {
                Thread.Sleep(500);
            }
        }
    }

    void initConsumers()
    {
        consumerPath_Threads = new TrafficGenerator[PATH_CONSUMER_LIMIT];

        for (int i = 0; i < PATH_CONSUMER_LIMIT; i++)
        {
            consumerPath_Threads[i] = new TrafficGenerator();
            consumerPath_Threads[i].consumePathQ = consumePathQ;
            consumerPath_Threads[i].Start();
        }
    }

    public void OnPathFound(Stack<Node> newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            lock (consumePathQ)
            {
                consumePathQ.Enqueue(newPath);

                if(consumePathQ.Count == 1)          // wait until the queue fills a little
                    Monitor.PulseAll(consumePathQ);  // the worker maybe waiting - wake it up
            }                
        }
    }
}

