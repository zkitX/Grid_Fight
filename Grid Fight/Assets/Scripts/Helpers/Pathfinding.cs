using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    bool[,] navGrid = new bool[0, 0];
    Vector2Int navGridSize
    {
        get
        {
            return new Vector2Int(navGrid.GetLength(0), navGrid.GetLength(1));
        }
    }

    //should return a vector 2 of next moves starting at the first next tile from the start tile and ending on the destination v2i
    public Vector2Int[] GetPathTo(Vector2Int destination, Vector2Int start, bool[,] navicableGrid)
    {
        navGrid = navicableGrid;
        List<PathNode> nodes = new List<PathNode> { new PathNode(start, 0, GetHeuristic(start, destination), null) };
        PathNode curNode = nodes[0];

        while (nodes.Where(r => !r.closed).FirstOrDefault() != null && curNode.pos != destination && navGrid[destination.x, destination.y] != false)
        {
            //Get the next least weighted node that isnt closed off
            nodes.OrderBy(r => r.weight);
            foreach(PathNode node in nodes)
            {
                if (node.closed == false)
                {
                    curNode = node;
                    break;
                }
            }

            //Add its children to the list of nodes
            for (int i = 0; i < 4; i++)
            {
                Vector2Int curCheck = new Vector2Int(curNode.pos.x + (i >= 2 ? i % 2 == 0 ? -1 : 1 : 0), curNode.pos.y + (i < 2 ? i % 2 == 0 ? -1 : 1 : 0));

                //Is the potential next neighbor actually on the grid and walkable?
                if (curCheck.x < navGridSize.x && curCheck.x < navGridSize.y && curCheck.x >= 0 && curCheck.y >= 0 && navGrid[curCheck.x, curCheck.y] == true)
                {
                    PathNode curNeighbor = nodes.Where(r => r.pos == curCheck && r.closed != true).FirstOrDefault();

                    //Is the potential next neighbor already probed or not, and if so, does it currently have a higher weight, and if so, update with a new pathprobe
                    if (curNeighbor == null || curNeighbor.weight > curNode.weight + 1)
                    {
                        bool wasNeighborNull = curNeighbor == null;
                        curNeighbor = new PathNode(curCheck, (byte)(curNode.weight + 1), GetHeuristic(curCheck, destination), curNode);
                        if(wasNeighborNull) nodes.Add(curNeighbor);
                    }
                }
            }
            curNode.closed = true;
        }

        

        if(curNode.pos == destination)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            PathNode curLevel = curNode;
            while(curLevel.previous != null)
            {
                path.Add(curLevel.pos);
                curLevel = curLevel.previous;
            }
            Vector2Int[] arPath = path.ToArray();
            Array.Reverse(arPath);
            return arPath;
        }

        return new Vector2Int[] { };
    }

    byte GetHeuristic(Vector2 start, Vector2 end)
    {
        return (byte)(Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y));
    }
}

public class PathNode
{
    public Vector2Int pos
    {
        get
        {
            return new Vector2Int(posX, posY);
        }
    }
    byte posX = 0;
    byte posY = 0;
    byte gVal = 0;
    byte hVal = 0;
    public ushort weight
    {
        get
        {
            return (ushort)(gVal + hVal);
        }
    }
    public bool closed = false;
    public PathNode previous = null;

    public PathNode(Vector2Int gridPosition, byte gValue, byte hValue, PathNode originNode)
    {
        posX = (byte)gridPosition.x;
        posY = (byte)gridPosition.y;
        gVal = gValue;
        hVal = hValue;
        closed = false;
        previous = originNode;
    }
}
