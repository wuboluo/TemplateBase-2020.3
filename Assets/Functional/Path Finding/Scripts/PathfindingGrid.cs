using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

[Serializable]
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}

public class PathfindingGrid : MonoBehaviour
{
    public enum Connections
    {
        Directional4,
        Directional8,
        Directional8DontCutCorners
    }

    public enum Heuristics
    {
        VectorMagnitude,
        Manhattan,
        Euclidean
    }

    //This is for showing calculated path. This can be used to debug paths. Can be removed.
    public static readonly List<Node> OpenList = new List<Node>();
    public static readonly List<Node> ClosedList = new List<Node>();
    public static bool PathFound;

    [SerializeField] private Vector2 gridWorldSize = new Vector2(100, 100);

    [SerializeField] private float nodeRadius = 1;

    [SerializeField] private float nearestNodeDistance = 10;

    [SerializeField] private float collisionRadius = 1;


    public LayerMask unWalkableMask;
    public TerrainType[] walkableRegions;


    public Connections connectionsOptions;
    public Heuristics heuristicMethod;

    //Using this value can decide whether algorithm should work more like dijkstra or greedy best first. If value is 1 this works like traditional A*.
    public float heuristicMultiplier;
    public bool showGrid;
    public bool showPathSearchDebug;
    private readonly Dictionary<int, int> _walkableRegionsDictionary = new Dictionary<int, int>();


    private int _currentIDThing = 1;

    private Node[,] _grid;
    private int _gridSizeX, _gridSizeY;
    private LayerMask _walkableMask;
    public static PathfindingGrid Instance { get; private set; }
    private float NodeDiameter => nodeRadius * 2;


    public static int Maxsize => Instance._gridSizeX * Instance._gridSizeY;

    public void Awake()
    {
        Instance = this;

        Init();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (showGrid)
        {
            if (_grid != null)
                foreach (var n in _grid)
                {
                    Gizmos.color = n.Walkable == NodeType.Walkable
                        ? new Color(255, 255, 255, 0.4f)
                        : new Color(255, 0, 0, 0.4f);
                    //if (path != null)
                    //    if (path.Contains(n))
                    //        Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.WorldPosition, Vector2.one * (NodeDiameter - .1f));
                }

            if (PathFound)
            {
                //Shows nodes added to open list
                Gizmos.color = Color.yellow;
                foreach (var t in OpenList)
                    Gizmos.DrawSphere(t.WorldPosition, nodeRadius - .1f);

                //Shows nodes added to closed list
                Gizmos.color = Color.red;
                foreach (var t in ClosedList)
                    Gizmos.DrawCube(t.WorldPosition, Vector2.one * (NodeDiameter - .1f) * 0.3f);

                //Draws line from node to it's parent
                Gizmos.color = Color.green;
                foreach (var t in ClosedList.Where(t => t.Parent != null))
                    Gizmos.DrawLine(t.WorldPosition, t.Parent.WorldPosition);
            }
        }
    }

    private void Init()
    {
        AddWalkableRegionsToDictionary();
        CreateGrid();
    }

    private void AddWalkableRegionsToDictionary()
    {
        foreach (var region in walkableRegions)
        {
            _walkableMask.value |= region.terrainMask.value;
            var terrainMask = (int) Mathf.Log(region.terrainMask.value, 2);
            if (_walkableRegionsDictionary.ContainsKey(terrainMask))
                _walkableRegionsDictionary[terrainMask] = region.terrainPenalty;
            else
                _walkableRegionsDictionary.Add(terrainMask, region.terrainPenalty);
        }
    }


    public void CreateGrid()
    {
        _gridSizeX = Mathf.RoundToInt(gridWorldSize.x / NodeDiameter);
        _gridSizeY = Mathf.RoundToInt(gridWorldSize.y / NodeDiameter);

        _grid = new Node[_gridSizeX, _gridSizeY];
        var worldBottomLeft = (Vector2) transform.position - Vector2.right * gridWorldSize.x / 2 -
                              Vector2.up * gridWorldSize.y / 2;
        for (var x = 0; x < _gridSizeX; x++)
        for (var y = 0; y < _gridSizeY; y++)
        {
            var worldPoint = worldBottomLeft + Vector2.right * (x * NodeDiameter + nodeRadius) +
                             Vector2.up * (y * NodeDiameter + nodeRadius);

            var walkable = Physics2D.OverlapCircle(worldPoint, nodeRadius * collisionRadius, unWalkableMask) == null;
            NodeType nodeType;
            nodeType = walkable ? NodeType.Walkable : NodeType.Obstacle;

            var movementPenalty = 0;

            var hit = Physics2D.OverlapCircleAll(worldPoint, nodeRadius, _walkableMask);
            foreach (var t in hit)
            {
                _walkableRegionsDictionary.TryGetValue(t.gameObject.layer, out var newPenalty);

                //Return terrain with highest movement penalty
                if (newPenalty > movementPenalty) movementPenalty = newPenalty;
            }

            _grid[x, y] = new Node(nodeType, worldPoint, x, y, movementPenalty);
        }

        var sw = new Stopwatch();
        sw.Start();
        for (var x = 0; x < _gridSizeX; x++)
        for (var y = 0; y < _gridSizeY; y++)
            GetNeighbours(_grid[x, y]);
        sw.Stop();
        // print("Walk: " + walk + " Obs: " + obs + "Time took create the grid" + sw.Elapsed);
        SetAreas();
    }

    public void SetAreas()
    {
        for (var x = 0; x < _gridSizeX; x++)
        for (var y = 0; y < _gridSizeY; y++)
            if (_grid[x, y].Walkable != NodeType.Obstacle && _grid[x, y].GridAreaID == 0)
            {
                SetGridAreas(_grid[x, y], _currentIDThing);
                _currentIDThing++;
            }
    }

    public void SetGridAreas(Node startNode, int currentAreaID)
    {
        var openSet = new Heap<Node>(Maxsize);
        var closedList = new Heap<Node>(Maxsize);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.RemoveFirst();
            closedList.Add(currentNode);

            foreach (var neighbour in currentNode.Neighbours)
            {
                if (neighbour == null || neighbour.Walkable == NodeType.Obstacle ||
                    closedList.Contains(neighbour)) continue;
                if (openSet.Contains(neighbour)) continue;
                neighbour.GridAreaID = currentAreaID;
                openSet.Add(neighbour);
            }
        }

        // print("Number of nodes:" + closedList.Count + ". Number of grid nodes:" + Maxsize);
    }


    public void ResetNodes()
    {
        for (var x = 0; x < _gridSizeX; x++)
        for (var y = 0; y < _gridSizeY; y++)
        {
            _grid[x, y].InClosedList = false;
            _grid[x, y].InOpenSet = false;
        }
    }

    public void GetNeighbours(Node node)
    {
        //Node[] neighbours = new Node[8];

        var index = 0;

        for (var x = -1; x <= 1; x++)
        for (var y = -1; y <= 1; y++)
        {
            if (connectionsOptions.Equals(Connections.Directional4) && Mathf.Abs(x) + Mathf.Abs(y) == 2) continue;
            //Skip center node, because it is current node
            if (x == 0 && y == 0)
                continue;

            var checkX = node.GridX + x;
            var checkY = node.GridY + y;

            if (checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY)
            {
                var newNode = _grid[checkX, checkY];

                if (node.Parent == newNode) continue;
                //Calculate obstacles while creating path
                //AStar.CheckIfNodeIsObstacle(newNode);

                //Prevent corner cutting
                if (connectionsOptions.Equals(Connections.Directional8DontCutCorners) &&
                    (_grid[checkX, checkY].Walkable == NodeType.Obstacle ||
                     _grid[checkX, node.GridY].Walkable == NodeType.Obstacle ||
                     _grid[node.GridX, checkY].Walkable == NodeType.Obstacle))
                {
                }
                else
                {
                    node.Neighbours[index] = newNode;
                    index++;
                }
            }
        }

        //return neighbours;
    }


    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        var positionOfNodeInGridX = worldPosition.x - transform.position.x;
        var positionOfNodeInGridY = worldPosition.y - transform.position.y;
        var percentX = (positionOfNodeInGridX + gridWorldSize.x / 2) / gridWorldSize.x;
        var percentY = (positionOfNodeInGridY + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        var x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        return _grid[x, y];
    }


    public Node ClosestNodeFromWorldPoint(Vector2 worldPosition, int nodeArea)
    {
        var positionOfNodeInGridX = worldPosition.x - transform.position.x;
        var positionOfNodeInGridY = worldPosition.y - transform.position.y;
        var percentX = (positionOfNodeInGridX + gridWorldSize.x / 2) / gridWorldSize.x;
        var percentY = (positionOfNodeInGridY + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        var x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        //If target node is inside collider return nearby node
        if (_grid[x, y].Walkable == NodeType.Obstacle || _grid[x, y].GridAreaID != nodeArea)
        {
            //Node[] neighbours 
            var neighbour = FindWalkableInRadius(x, y, 1, nodeArea);
            if (neighbour != null) return neighbour;
        }

        return _grid[x, y];
    }

    public static void CheckIfNodeIsObstacle(Node node)
    {
        ////Calculate obstacles while creating path
        var colliders = Physics2D.OverlapCircleAll(node.WorldPosition, Instance.nodeRadius * Instance.collisionRadius,
            Instance.unWalkableMask);
        node.Walkable = colliders.Length > 0 ? NodeType.Obstacle : NodeType.Walkable;
    }

    private Node FindWalkableInRadius(int centreX, int centreY, int radius, int nodeArea)
    {
        while (true)
        {
            if (radius > nearestNodeDistance)
            {
                Debug.LogWarning("Target area is not in nearestNodeDistance!");
                return null;
            }

            for (var i = -radius; i <= radius; i++)
            {
                var verticalSearchX = i + centreX;
                var horizontalSearchY = i + centreY;

                // top
                if (InBounds(verticalSearchX, centreY + radius))
                    if (_grid[verticalSearchX, centreY + radius].Walkable == NodeType.Walkable && _grid[verticalSearchX, centreY + radius].GridAreaID == nodeArea)
                        return _grid[verticalSearchX, centreY + radius];

                // bottom
                if (InBounds(verticalSearchX, centreY - radius))
                    if (_grid[verticalSearchX, centreY - radius].Walkable == NodeType.Walkable && _grid[verticalSearchX, centreY - radius].GridAreaID == nodeArea)
                        return _grid[verticalSearchX, centreY - radius];
                // right
                if (InBounds(centreY + radius, horizontalSearchY))
                    if (_grid[centreX + radius, horizontalSearchY].Walkable == NodeType.Walkable && _grid[centreX + radius, horizontalSearchY].GridAreaID == nodeArea)
                        return _grid[centreX + radius, horizontalSearchY];

                // left
                if (InBounds(centreY - radius, horizontalSearchY))
                    if (_grid[centreX - radius, horizontalSearchY].Walkable == NodeType.Walkable && _grid[centreX - radius, horizontalSearchY].GridAreaID == nodeArea)
                        return _grid[centreX - radius, horizontalSearchY];
            }

            radius++;
        }
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && x < _gridSizeX && y >= 0 && y < _gridSizeY;
    }
}