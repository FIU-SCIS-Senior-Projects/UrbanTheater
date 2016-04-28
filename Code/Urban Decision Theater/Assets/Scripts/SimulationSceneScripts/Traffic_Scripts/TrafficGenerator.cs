using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class TrafficGenerator
{
    static bool poison;
    Thread m_Thread;

    public Queue<Stack<Node>> consumePathQ;
    AutoResetEvent myEvent = new AutoResetEvent(false);
    Stack<Node> path;

    public void Start()
    {
        Poison = false;
        m_Thread = new Thread(this.Run);
        m_Thread.Start();
    }

    public void Join()
    {
        myEvent.Set(); myEvent.Set();
        if (!m_Thread.Join(500))
            m_Thread.Abort();
    }

    private void Run()
    {
        while (!Poison)
        {
            try
            {
                WaitForQ();
                ConsumePath();
            }
            catch(Exception e) { Debug.Log(e.Message); }
        }
    }

    void ConsumePath()
    {
        Node currentNode;

        while (path.Count != 0)
        {
            if (Poison)
                break;
            currentNode = path.Pop();

            int waitTime = currentNode.trafficTimer.IncreaseTraffic();
            myEvent.WaitOne(waitTime);
        }
        
    }

    void WaitForQ()
    {
        lock (consumePathQ)
        {
            while (consumePathQ.Count == 0 && !Poison)
                Monitor.Wait(consumePathQ);         // RELEASES THE LOCK, waits for a Pulse, and re-acquires the lock

            if (!Poison)                            // application exit ;; dont touch queue anymore
                path = consumePathQ.Dequeue();      // we have the lock, dequeue next path
        }
    }
    
    public static bool Poison
    {
        get
        {
            return poison;
        }
        set
        {
            poison = value;
        }
    } 
}
