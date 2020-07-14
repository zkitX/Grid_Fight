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

    List<PathNode> nodes = new List<PathNode>();

    BattleTileScript currentTile;
    BattleTileScript nextTile;

    //should return a vector 2 of next moves starting at the first next tile from the start tile and ending on the destination v2i
    public Vector2Int[] GetPathTo(Vector2Int destination, Vector2Int start, bool[,] navicableGrid)
    {
        navGrid = navicableGrid;
        nodes.Clear();
        nodes.Add(new PathNode(start, 0, null));
        currentTile = GridManagerScript.Instance.GetBattleTile(start);
        PathNode curNode = nodes[0];
        curNode.Checked = true;
        bool nextTileFounded = false;
        int i = 0;
        while (nodes.Where(r => r.Closed).FirstOrDefault() == null)
        {
            nextTileFounded = false;
            if (curNode == null)
            {
                curNode = nodes.Where(r => !r.Checked).OrderBy(a => a.Weight).FirstOrDefault();
                if(curNode == null)
                {
                    continue;
                }
                else
                {
                    curNode.Checked = true;
                }
            }

            if (destination.x < curNode.Pos.x)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos.x - 1, curNode.Pos.y), new Vector2Int(curNode.Pos.x, curNode.Pos.y + 1), new Vector2Int(curNode.Pos.x, curNode.Pos.y - 1)))
                {
                    nextTileFounded = true;
                } 
            }
            if (destination.x > curNode.Pos.x)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos.x + 1, curNode.Pos.y), new Vector2Int(curNode.Pos.x, curNode.Pos.y + 1), new Vector2Int(curNode.Pos.x, curNode.Pos.y - 1)))
                {
                    nextTileFounded = true;
                }
            }
            if (destination.y > curNode.Pos.y)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos.x, curNode.Pos.y + 1), new Vector2Int(curNode.Pos.x + 1, curNode.Pos.y), new Vector2Int(curNode.Pos.x - 1, curNode.Pos.y)))
                {
                    nextTileFounded = true;
                }
            }
            if (destination.y < curNode.Pos.y)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos.x, curNode.Pos.y - 1), new Vector2Int(curNode.Pos.x + 1, curNode.Pos.y), new Vector2Int(curNode.Pos.x - 1, curNode.Pos.y)))
                {
                    nextTileFounded = true;
                }
            }
            curNode = null;
            i++;
            if(!nextTileFounded || i > 10)
            {
                return new Vector2Int[] { };
            }
        }

        curNode = nodes.Where(r => r.Closed).First();
        if (curNode.Pos == destination)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            PathNode curLevel = curNode;
            while(curLevel.Previous != null)
            {
                path.Add(curLevel.Pos);
                curLevel = curLevel.Previous;
            }
            Vector2Int[] arPath = path.ToArray();
            Array.Reverse(arPath);
            return arPath;
        }

        return new Vector2Int[] { };
    }



    public bool TileAvailabilityCheck(PathNode curNode, Vector2Int destination, Vector2Int next, Vector2Int chance2, Vector2Int chance3)
    {
        if (GridManagerScript.Instance.IsWalkableAndFree(next, currentTile.WalkingSide) && (curNode.Previous != null ? next != curNode.Previous.Pos : true))
        {
            if(!nodes.Contains(new PathNode(next, curNode.Weight + 1, curNode)))
            {
                nodes.Add(new PathNode(next, curNode.Weight + 1, curNode));
            }
            if (next == destination)
            {
                nodes.Last().Closed = true;
            }
            return true;
        }
        else
        {
            if (GridManagerScript.Instance.IsWalkableAndFree(chance2, currentTile.WalkingSide) && (curNode.Previous != null ? chance2 != curNode.Previous.Pos : true))
            {
                nodes.Add(new PathNode(chance2, curNode.Weight + 1, curNode));
                if (chance2 == destination)
                {
                    nodes.Last().Closed = true;
                }
                return true;
            }
            if (GridManagerScript.Instance.IsWalkableAndFree(chance3, currentTile.WalkingSide) && (curNode.Previous != null ? chance3 != curNode.Previous.Pos : true))
            {
                nodes.Add(new PathNode(chance3, curNode.Weight + 1, curNode));
                if (chance3 == destination)
                {
                    nodes.Last().Closed = true;
                }
                return true;
            }
        }

        return false;
    }


    byte GetHeuristic(Vector2 start, Vector2 end)
    {
        return (byte)(Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y));
    }
}

public class PathNode
{
    public Vector2Int Pos;
    public int Weight = 0;
    public bool Closed = false;
    public bool Checked = false;
    public PathNode Previous = null;

    public PathNode()
    {

    }

    public PathNode(Vector2Int pos, int weight, PathNode originNode)
    {
        Pos = pos;
        Weight = weight;
        Previous = originNode;
    }
}
