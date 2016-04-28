using UnityEngine;
using System.Collections.Generic;

public class FindPath : MonoBehaviour {

    Heap<Node> openSet;         // The set of currently discovered nodes still to be evaluated.
                                // Initially, only the start node is known.
    HashSet<Node> closedSet;    // The set of nodes already evaluated.
    PathRequestManager requestManager;
    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Node _startNode, Node _targetNode)
    {
        CreatePath(_startNode, _targetNode);
    }

    void CreatePath(Node startNode, Node targetNode)
    {
        bool pathSuccess = false;
        int traffic;
        int trafficCap;
        openSet = new Heap<Node>(grid.RoadSet.Count);
        closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        
        while (openSet.Count > 0)
        {
            // remove the smallest fCost from Heap
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                pathSuccess = true;
                break;
            }
            
            foreach (Node neighbour in grid.PathGetRoadNeighbours(currentNode))
            {
                if (closedSet.Contains(neighbour))
                    continue;

                lock (currentNode)
                    traffic = currentNode.trafficTimer.TrafficIntensity;
                trafficCap = currentNode.trafficTimer.InitialTrafficIntensity;
                int penalty = 17 - 17 * traffic / (trafficCap + 17/trafficCap);

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbour) + penalty;
                
                if (newMovementCostToNeighbor < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbor;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;


                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);
                }
            }
        }
        requestManager.FinishedProcessingPath(ReverseParents(startNode, targetNode), pathSuccess);
    }

    // pythagorean theorem, digonal sqrt(2) = 1.4; horizontal/vertical is 1 unit
    // remove decimal, so multiply 10
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    // Path found reverse the parents
    Stack<Node> ReverseParents(Node startNode, Node targetNode)
    {
        Stack<Node> parentOf = new Stack<Node>();
        Node currentNode = targetNode;
        while (currentNode != startNode)
        {
            parentOf.Push(currentNode);
            currentNode = currentNode.parent;
        }
        //parentOf.Push(currentNode);
        return parentOf;
    }
}
