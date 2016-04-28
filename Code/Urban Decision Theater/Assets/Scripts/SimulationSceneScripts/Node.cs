using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool sector;
    public int gridX;
    public int gridY;    
    public int gCost;   // distance from the starting node
    public int hCost;   // distance from end node (opposite from the gCost)
                        // if two nodes have the same fCost then the next node will be node with the smallest hCost
    public Vector3 worldPosition;
    public Node parent;
    public TrafficIntensityTimer trafficTimer;

    bool isRoad;
    int heapIndex;
    float waterData;
    float populationData;
    
    public Node(bool _sector, Vector3 _worldPos, int _gridX, int _gridY)
    {
        sector = _sector;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        isRoad = false;
    }

    public void setIsRoad()
    {
        if(trafficTimer == null)
        {
            trafficTimer = new TrafficIntensityTimer(this);
            isRoad = true;
        }
    }

    public bool IsRoad
    {
        get
        {
            return isRoad;
        }
        set
        {
            isRoad = value;
        }
    }

    // Used to check surounding nodes for the best Cost
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return -compare;
    }
}
