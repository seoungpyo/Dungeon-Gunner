using System.Collections.Generic;
using UnityEngine;

public static class Astar
{
    /// <summary>
    /// Build a path for the room, from the startGridPosition to the endGridPosition, and adds
    /// movement steps to the return stack. return null if no path is found
    /// </summary>
    /// <param name="room"></param>
    /// <param name="startGridPosition"></param>
    /// <param name="endGridPosition"></param>
    /// <returns></returns>
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startGridPosition, Vector3Int endGridPosition)
    {
        // adjust position by lower bounds
        startGridPosition -= (Vector3Int)room.templateLowerBounds;
        endGridPosition -= (Vector3Int)room.templateLowerBounds;

        // create open list and closed hashset
        List<Node> openNodeList = new List<Node>();
        HashSet<Node> closeNodeHashSet = new HashSet<Node>();

        // create gridnodes for path finding
        GridNodes gridNodes = new GridNodes(room.templateUpperBounds.x - room.templateLowerBounds.x + 1,
            room.templateUpperBounds.y - room.templateLowerBounds.y + 1);

        Node startNode = gridNodes.GetGridNode(startGridPosition.x, startGridPosition.y);
        Node targetNode = gridNodes.GetGridNode(endGridPosition.x, endGridPosition.y);

        Node endPathNode = FindShortestPath(startNode, targetNode, gridNodes, openNodeList, closeNodeHashSet, room.instantiatedRoom);
    
        if(endPathNode != null)
        {
            return CreatePathStack(endPathNode, room);
        }

        return null;
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList, HashSet<Node> closedNodeHashSet,
        InstantiatedRoom instantiatedRoom)
    {
        // add start node to open list 
        openNodeList.Add(startNode);

        // loop through open node list until empty
        while(openNodeList.Count > 0)
        {
            openNodeList.Sort();

            Node currentNode = openNodeList[0];
            openNodeList.RemoveAt(0);

            if(currentNode == targetNode)
            {
                return currentNode;
            }

            closedNodeHashSet.Add(currentNode);

            EvaluateCurrentNodeNeighbours(currentNode, targetNode, gridNodes, openNodeList, closedNodeHashSet, instantiatedRoom); 
        }

        return null;
    }

    private static void EvaluateCurrentNodeNeighbours(Node currentNode, Node targetNode, GridNodes gridNodes, List<Node> openNodeList,
        HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        Vector2Int currentNodeGridPosition = currentNode.gridPostion;

        Node validNeighbourNode;
        
        for(int i = -1; i<=1; i++)
        {
            for(int j = -1; j<= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                validNeighbourNode = GetValidNodeNeighbour(currentNodeGridPosition.x + i, currentNodeGridPosition.y + j, gridNodes,
                    closedNodeHashSet, instantiatedRoom);

                if(validNeighbourNode != null)
                {
                    int newCostToNeighbour;

                    int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[validNeighbourNode.gridPostion.x,
                        validNeighbourNode.gridPostion.y];

                    newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, validNeighbourNode) + movementPenaltyForGridSpace;

                    bool isValidNeighbourNodeInOpenList = openNodeList.Contains(validNeighbourNode);

                    if(newCostToNeighbour < validNeighbourNode.gCost || !isValidNeighbourNodeInOpenList)
                    {
                        validNeighbourNode.gCost = newCostToNeighbour;
                        validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);
                        validNeighbourNode.parentNode = currentNode;

                        if (!isValidNeighbourNodeInOpenList)
                        {
                            openNodeList.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }
    }

    private static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPostion.x - nodeB.gridPostion.x);
        int dstY = Mathf.Abs(nodeA.gridPostion.y - nodeB.gridPostion.y);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY); 
        }

        return 14 * dstX + 10 * (dstY - dstX);
    }


    private static Node GetValidNodeNeighbour(int neighbourNodeXPosition, int neighbourNodeYPosition, GridNodes gridNodes,
        HashSet<Node> closedNodeHashSet, InstantiatedRoom instantiatedRoom)
    {
        if(neighbourNodeXPosition >= instantiatedRoom.room.templateUpperBounds.x - instantiatedRoom.room.templateLowerBounds.x || neighbourNodeXPosition < 0
            || neighbourNodeYPosition >= instantiatedRoom.room.templateUpperBounds.y - instantiatedRoom.room.templateLowerBounds.y || neighbourNodeYPosition < 0)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(neighbourNodeXPosition, neighbourNodeYPosition);

        int movementPenaltyForGridSpace = instantiatedRoom.aStarMovementPenalty[neighbourNodeXPosition, neighbourNodeYPosition];

        if (movementPenaltyForGridSpace == 0 || closedNodeHashSet.Contains(neighbourNode))
        {
            return null;
        }
        else
        {
            return neighbourNode;
        }
    }

    /// <summary>
    /// Create a Stacck<Vector3> containing the movement path
    /// </summary>
    /// <param name="targetNode"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private static Stack<Vector3> CreatePathStack(Node targetNode, Room room)
    {
        Stack<Vector3> movementPathStack = new Stack<Vector3>();

        Node nextNode = targetNode;

        Vector3 cellMidPoint = room.instantiatedRoom.grid.cellSize * 0.5f;
        cellMidPoint.z = 0f;

        while(nextNode != null)
        {
            // convert grid position to world position
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(new Vector3Int(nextNode.gridPostion.x + room.templateLowerBounds.x,
                nextNode.gridPostion.y + room.templateLowerBounds.y,0));

            worldPosition += cellMidPoint;

            movementPathStack.Push(worldPosition);

            nextNode = nextNode.parentNode;
        }

        return movementPathStack;
    }
}
