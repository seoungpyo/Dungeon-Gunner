using UnityEngine;

public class GridNodes
{
    private int width;
    private int height;

    private Node[,] gridNode;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        gridNode = new Node[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridNode[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    public Node GetGridNode(int xPosition, int yPostion)
    {
        if(xPosition <width && yPostion < height)
        {
            return gridNode[xPosition, yPostion];
        }
        else
        {
            Debug.Log("requested grid node is out of range");
            return null;
        }
    }
}
