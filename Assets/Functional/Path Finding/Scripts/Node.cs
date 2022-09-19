using UnityEngine;

public class Node : IHeapItem<Node>
{
    public int GCost;

    public int GridAreaID = 0;
    public readonly int GridX;
    public readonly int GridY;
    public int HCost;


    public bool InClosedList;
    public bool InOpenSet;
    public readonly int MovementPenalty;
    public readonly Node[] Neighbours = new Node[8];

    public Node Parent;
    public NodeType Walkable;
    public Vector3 WorldPosition;

    public Node(NodeType walkable, Vector3 worldPos, int gridX, int gridY, int penalty)
    {
        Walkable = walkable;
        WorldPosition = worldPos;
        GridX = gridX;
        GridY = gridY;
        MovementPenalty = penalty;
    }

    public int FCost => GCost + HCost;

    public int HeapIndex { get; set; }

    public int CompareTo(Node nodeToCompare)
    {
        var compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0) compare = HCost.CompareTo(nodeToCompare.HCost);
        return -compare;
    }
}

public enum NodeType
{
    Obstacle,
    Walkable
}