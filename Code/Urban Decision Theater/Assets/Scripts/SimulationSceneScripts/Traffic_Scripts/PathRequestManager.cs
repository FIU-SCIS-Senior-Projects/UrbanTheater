using UnityEngine;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    FindPath pathFinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathFinding = GetComponent<FindPath>();
    }

    public static void RequestPath(Node pathStart, Node pathEnd, Action<Stack<Node>, bool> callBack)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callBack);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Stack<Node> path, bool success)
    {
        currentPathRequest.callBack(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    public int PathRequestQueueSize
    {
        get
        {
            return pathRequestQueue.Count;
        }
    }

    struct PathRequest
    {
        public Node pathStart;
        public Node pathEnd;
        public Action<Stack<Node>, bool> callBack;

        public PathRequest(Node _pathStart, Node _pathEnd, Action<Stack<Node>, bool> _callBack)
        {
            pathStart = _pathStart;
            pathEnd = _pathEnd;
            callBack = _callBack;
        }
    }
}
