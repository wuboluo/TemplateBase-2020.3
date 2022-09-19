using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class AStar
{
    //Max nodes to count. This will prevent counting the whole grid if target is unreachable.
    private const int ClosedListMaxCount = 100000;

    /// <summary>
    ///     Creates path from startPos to targetPos using A*.
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    public static Vector2[] FindPath(Node startNode, Node goalNode)
    {
        //How long will path founding take
        var sw = new Stopwatch();
        //For showing path counting process. Resets grid.

        if (PathfindingGrid.Instance.showPathSearchDebug)
        {
            sw.Start();

            PathfindingGrid.OpenList.Clear();
            PathfindingGrid.ClosedList.Clear();
            PathfindingGrid.PathFound = false;
        }

        PathfindingGrid.Instance.ResetNodes();
        var nodeCount = 0;

        var openSet = new Heap<Node>(PathfindingGrid.Maxsize);


        //if (goalNode.gridAreaID != startNode.gridAreaID) {
        //    UnityEngine.Debug.Log("Node is unreachable!");
        //    return null;

        //}
        if (goalNode.Walkable == NodeType.Obstacle || startNode.Walkable == NodeType.Obstacle)
        {
            Debug.Log("Start or goal inside collider.");
            return null;
        }


        openSet.Add(startNode);
        //For showing path counting 
        if (PathfindingGrid.Instance.showGrid) PathfindingGrid.OpenList.Add(startNode);
        nodeCount++;

        //Node[] neighbours;
        Node neighbour;
        Node currentNode;

        while (openSet.Count > 0)
        {
            currentNode = openSet.RemoveFirst();
            currentNode.InClosedList = true;

            //For showing path counting 
            if (PathfindingGrid.Instance.showGrid) PathfindingGrid.ClosedList.Add(currentNode);


            if (currentNode == goalNode)
            {
                //For testing path calculation. Can be removed from final version.
                sw.Stop();
                if (PathfindingGrid.Instance.showPathSearchDebug)
                {
                    var path = RetracePath(startNode, goalNode);
                    Debug.Log("<color=Blue>Path found! </color> Time took to calculate path: " + sw.Elapsed +
                              "ms. Number of nodes counted " + nodeCount + ". Path lenght: " + path.Length +
                              ". Heurastics: " + PathfindingGrid.Instance.heuristicMethod);
                    PathfindingGrid.PathFound = true;
                }

                return RetracePath(startNode, goalNode);
            }

            //if (openSet.Count > closedListMaxCount) {
            //    return null;
            //}
            //UnityEngine.Debug.Log("Neigg" + currentNode.neighbours[0].gridX);
            //PathfindingGrid.Instance.GetNeighbours(currentNode);

            for (var i = 0; i < currentNode.Neighbours.Length; i++)
            {
                neighbour = currentNode.Neighbours[i];

                //Calculate obstacles while creating path
                //CheckIfNodeIsObstacle(neighbour);

                if (neighbour == null || neighbour.Walkable == NodeType.Obstacle || neighbour.InClosedList) continue;

                var newCostToNeighbour =
                    currentNode.GCost + GetDistance(currentNode, neighbour) + neighbour.MovementPenalty;
                if (newCostToNeighbour < neighbour.GCost || neighbour.InOpenSet == false)
                {
                    neighbour.GCost = newCostToNeighbour;
                    neighbour.HCost = Mathf.RoundToInt(GetDistance(neighbour, goalNode) *
                                                       PathfindingGrid.Instance.heuristicMultiplier);
                    neighbour.Parent = currentNode;

                    if (neighbour.InOpenSet == false)
                    {
                        openSet.Add(neighbour);
                        neighbour.InOpenSet = true;

                        //For showing path counting 
                        if (PathfindingGrid.Instance.showGrid) PathfindingGrid.OpenList.Add(neighbour);
                        nodeCount++;
                    }
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        if (PathfindingGrid.Instance.showPathSearchDebug)
        {
            sw.Stop();
            Debug.Log("<color=red>Path not found! </color> Time took to calculate path: " + sw.ElapsedMilliseconds +
                      "ms.");
        }

        return null;
    }

    /// <summary>
    ///     Creates path from startNode to targetNode
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public static Vector2[] RetracePath(Node startNode, Node targetNode)
    {
        var path = new List<Vector2>();
        var currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.WorldPosition);
            currentNode = currentNode.Parent;
        }

        var waypoints = new Vector2[path.Count];

        var pathLength = 0;
        for (var i = path.Count - 1; i >= 0; i--)
        {
            waypoints[pathLength] = path[i];
            pathLength++;
        }

        //Vector2[] waypoints = path.ToArray();
        //Array.Reverse(waypoints);
        return waypoints;
    }

    /// <summary>
    ///     Way to reduce number of nodes from path. Only adds nodes that have new direction.This is useful if you have grid
    ///     based game, but more nodes is better for non grid based game so path looks smooter.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Vector2[] SimplifyPath(List<Node> path)
    {
        var waypoints = new List<Vector2>();
        var directionOld = Vector2.zero;

        for (var i = 1; i < path.Count; i++)
        {
            var directionNew = new Vector2(path[i - 1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);
            if (directionNew != directionOld) waypoints.Add(path[i].WorldPosition);
            directionOld = directionNew;
        }

        waypoints.Add(path[path.Count - 1].WorldPosition);
        return waypoints.ToArray();
    }

    /// <summary>
    ///     Reduces number of nodes from path. Only adds nodes that are blocked by obstacle. This is useful if you want to have
    ///     short path, but you can create smoother looking path using dynamic collider check.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Vector2[] PathSmooter(Vector2[] path)
    {
        var waypoints = new List<Vector2>();
        var currentNode = 0;
        waypoints.Add(path[0]);

        var security = 0;
        for (var i = 1; i < path.Length; i++)
        {
            security++;
            if (security >= 1000)
            {
                Debug.LogError("Crash");
                break;
            }

            bool cantSeeTarget =
                Physics2D.Linecast(path[currentNode], path[i], PathfindingGrid.Instance.unWalkableMask);
            if (cantSeeTarget)
            {
                waypoints.Add(path[i - 1]);
                currentNode = i - 1;
            }
        }

        waypoints.Add(path[path.Length - 1]);
        return waypoints.ToArray();
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        if (PathfindingGrid.Instance.heuristicMethod == PathfindingGrid.Heuristics.VectorMagnitude)
            return GetDistance2(nodeA, nodeB);
        if (PathfindingGrid.Instance.heuristicMethod == PathfindingGrid.Heuristics.Euclidean)
            return GetDistance3(nodeA, nodeB);
        return GetDistance1(nodeA, nodeB);
    }

    /// <summary>
    ///     Gets heuristic distance from nodeA to nodeB using Manhattan distance
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private static int GetDistance1(Node nodeA, Node nodeB)
    {
        var dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        var dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    /// <summary>
    ///     Gets heuristic distance from nodeA to nodeB using basic distance
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private static int GetDistance2(Node nodeA, Node nodeB)
    {
        var distance = (int) (nodeA.WorldPosition - nodeB.WorldPosition).magnitude;
        return distance;
    }

    /// <summary>
    ///     Gets heuristic distance from nodeA to nodeB using Euclidean distance
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private static int GetDistance3(Node nodeA, Node nodeB)
    {
        var dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        var dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        return 10 * (int) Mathf.Sqrt(dstX * dstX + dstY * dstY);
    }
}