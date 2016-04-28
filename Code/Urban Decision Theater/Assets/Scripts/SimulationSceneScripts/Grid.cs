using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Grid : MonoBehaviour {

    public float nodeRadius;
    public bool displayGridGizmos = false;
    public Vector2 gridWorldSize;           // size of the grid
    public LayerMask sectorMask;            // layers that where selected
    public GameObject miamiColliders;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    Node aRoadNode;
    HashSet<string> sectorNames;
    Dictionary<string, Node[]> citySectors;
    HashSet<Node> roadSet;             // breath first search // used in traffic manager to put weights for traffic intensity
    ArrayList edgeSet;                 // used in traffic manage to select a node along the edge (the start node)
    Node[,] grid;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        initSectorNames();
        CreateGrid();
        disableColliders();
    }
    
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        citySectors = new Dictionary<string, Node[]>();
        roadSet = new HashSet<Node>();
        edgeSet = new ArrayList();
        Node[] sectorSpan;

        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool isSector = !(Physics.CheckSphere(worldPoint, nodeRadius, sectorMask)); // Physics.CheckBox(worldPoint, new Vector3(nodeRadius, nodeRadius, nodeRadius), Quaternion.Euler(0, 0, 0), sectorMask);
                grid[x, y] = new Node(isSector, worldPoint, x, y);

                if (rayCastRoad(worldPoint))
                {
                    grid[x, y].setIsRoad();
                    grid[x, y].trafficTimer.TrafficIntensity++;
                    aRoadNode = grid[x, y];
                    if((x < GridSizeX && (y == 0 || y == GridSizeY-1)) || (y < GridSizeY && (x == 0 || x == GridSizeX-1)))
                        edgeSet.Add(grid[x ,y]);
                    roadSet.Add(grid[x, y]);
                }
                
                string sectorName = rayCastSector(worldPoint);
                
                if (!isSector && sectorName != null && !citySectors.ContainsKey(sectorName))
                {
                    // found the first point of a sector
                    sectorSpan = new Node[2];
                    sectorSpan[0] = grid[x, y];
                    citySectors.Add(sectorName, sectorSpan);
                }
                else if (!isSector && sectorName != null)
                {
                    // keep updating the last point until the end of the sector
                    sectorSpan = citySectors[sectorName];
                    sectorSpan[1] = grid[x, y];
                    citySectors[sectorName] = sectorSpan;
                }
            }
        }
    }


    // cast a ray down to determine if it's a sector
    string rayCastSector(Vector3 pos)
    {
        RaycastHit hit;
        Ray ray;
        // check all corners and center if needed
        for(int x = -1; x < 2; x++)
        {
            for(int z = -1; z < 2; z++)
            {
                ray = new Ray(pos + (Vector3.up * 20) + new Vector3(nodeRadius*x, 0, nodeRadius*z), Vector3.down);
                if (Physics.Raycast(ray, out hit, sectorMask))
                {
                    string collideTag = hit.collider.tag;
                    if (sectorNames.Contains(collideTag))
                        return collideTag;
                }
            }
        }
        return null;
    }

    // cast a ray down to determine if it's a road
    bool rayCastRoad(Vector3 pos)
    {
        RaycastHit hit;
        int roadMask = 1 << 17;
        Ray ray;
        // check all corners and center if needed
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                ray = new Ray(pos + Vector3.up + new Vector3(nodeRadius * 0.3f * x, 0, nodeRadius * 0.3f * z), Vector3.down);
                if (Physics.Raycast(ray, out hit, roadMask))
                {
                    if (hit.collider.tag == "Road")
                        return true;
                }
            }
        }
        return false;
    }

    void initSectorNames()
    {
        sectorNames = new HashSet<string>();
        for(int i = 8; i < 17; i++)
        {
            if (((1 << i) & sectorMask) > 0)
                sectorNames.Add(LayerMask.LayerToName(i));
        }
    }

    void disableColliders()
    {
        foreach (Collider c in miamiColliders.GetComponentsInChildren<Collider>())
            c.enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                //Gizmos.color = (n.sector) ? Color.white : Color.red;
                Gizmos.color = Color.white;
                if (n.IsRoad)
                {
                    int initWeight = n.trafficTimer.InitialTrafficIntensity;
                    int x = Mathf.Clamp(n.trafficTimer.TrafficIntensity, 0, initWeight);
                    
                    float red = (initWeight - x) / initWeight;
                    float green = x / initWeight;
                    
                    if(x == 0)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = new Color(red, green, 0);
                    }
                    
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }

            //foreach (string name in citySectors.Keys)
            //{
            //    Node[] nSpan = citySectors[name];
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawCube(nSpan[0].worldPosition, Vector3.one * (nodeDiameter));
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawCube(nSpan[1].worldPosition, Vector3.one * (nodeDiameter));
            //}
        }
    }

    public int MaxSize
    {
        get{ return gridSizeX * gridSizeY; }
    }

    public int GridSizeX
    {
        get { return gridSizeX; }
    }

    public int GridSizeY
    {
        get { return gridSizeY; }
    }

    public Dictionary<string, Node[]> CitySectors
    {
        get{ return citySectors; }
    }

    public Node[,] Nodes 
    {
        get{ return grid; }
    }

    public HashSet<Node> RoadSet
    {
        get{ return roadSet; }
    }

    public ArrayList EdgeSet
    {
        get { return edgeSet; }
    }

    public Node ARoadNode
    {
        get{ return aRoadNode; }
    }

    // ------- Auxiliary Methods ------- //
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    public List<Node> BFSGetRoadNeighbours(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY 
                    && grid[checkX, checkY].IsRoad)
                {
                    node.trafficTimer.TrafficIntensity += 2;      // Make node bi-directional :: incoming = outgoing
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        node.trafficTimer.SetInitWeight();
        return neighbors;
    }

    public List<Node> PathGetRoadNeighbours(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY
                    && grid[checkX, checkY].IsRoad)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX); // always b/w 0-1 make sure it's not outside the world
        percentY = Mathf.Clamp01(percentY); // always b/w 0-1

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
    
}

