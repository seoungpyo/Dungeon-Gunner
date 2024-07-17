using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPostion;
    public int gCost = 0; // distance from starting node
    public int hCost = 0; // distance from finishing node
    public int FCost { get { return gCost + hCost; } }

    public Node parentNode;

    public Node(Vector2Int gridPosition)
    {
        this.gridPostion = gridPosition;
        parentNode = null;
    }

    public int CompareTo(Node nodeToCompare)
    {
        //compare will be <0 if this instance fcost is less than nodeToCompare.Fcost.
        //compare will be >0 if this instance fcost is greater than nodeToCompare.Fcost.
        //compare will be == 0 if the value are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return compare;
    }
}
