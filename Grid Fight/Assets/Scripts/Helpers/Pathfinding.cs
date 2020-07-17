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
    WalkingSideType walkingSide;
    List<BattleTileScript> currentTiles;
    List<BattleTileScript> nextTiles;

    //should return a vector 2 of next moves starting at the first next tile from the start tile and ending on the destination v2i
    public Vector2Int[] GetPathTo(Vector2Int destination, List<Vector2Int> start, bool[,] navicableGrid)
    {
        navGrid = navicableGrid;
        nodes.Clear();
        nodes.Add(new PathNode(start, 0, null));
        currentTiles = GridManagerScript.Instance.GetBattleTiles(start);
        walkingSide = currentTiles[0].WalkingSide;
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
                    return new Vector2Int[] { };
                }
              
            }
            curNode.Checked = true;

            if (destination.x < curNode.Pos[0].x)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos[0].x - 1, curNode.Pos[0].y), new Vector2Int(curNode.Pos[0].x, curNode.Pos[0].y + 1), new Vector2Int(curNode.Pos[0].x, curNode.Pos[0].y - 1)))
                {
                    nextTileFounded = true;
                    curNode = nodes.Last();
                } 
            }
            if (destination.x > curNode.Pos[0].x)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos[0].x + 1, curNode.Pos[0].y), new Vector2Int(curNode.Pos[0].x, curNode.Pos[0].y + 1), new Vector2Int(curNode.Pos[0].x, curNode.Pos[0].y - 1)))
                {
                    nextTileFounded = true;
                    curNode = nodes.Last();
                }
            }
            if (destination.y > curNode.Pos[0].y)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos[0].x, curNode.Pos[0].y + 1), new Vector2Int(curNode.Pos[0].x + 1, curNode.Pos[0].y), new Vector2Int(curNode.Pos[0].x - 1, curNode.Pos[0].y)))
                {
                    nextTileFounded = true;
                    curNode = nodes.Last();
                }
            }
            if (destination.y < curNode.Pos[0].y)
            {
                if (TileAvailabilityCheck(curNode, destination, new Vector2Int(curNode.Pos[0].x, curNode.Pos[0].y - 1), new Vector2Int(curNode.Pos[0].x + 1, curNode.Pos[0].y), new Vector2Int(curNode.Pos[0].x - 1, curNode.Pos[0].y)))
                {
                    nextTileFounded = true;
                    curNode = nodes.Last();
                }
            }
            curNode = null;
            i++;
            if(nodes.Last().Weight > 10)
            {
                return new Vector2Int[] { };
            }
        }

        curNode = nodes.Where(r => r.Closed).First();
        if (curNode.Pos[0] == destination)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            PathNode curLevel = curNode;
            while(curLevel.Previous != null)
            {
                path.Add(curLevel.Pos[0]);
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
        if (GridManagerScript.Instance.IsWalkableAndFree(curNode.Pos, next, walkingSide) && (curNode.Previous != null ? curNode.Previous.Pos.Contains(next) : true))
        {
            List<Vector2Int> res = new List<Vector2Int>();
            curNode.Pos.ForEach(r => res.Add((r - curNode.Pos[0]) + next));
            if (!nodes.Contains(new PathNode(res, curNode.Weight + 1, curNode)))
            {
                nodes.Add(new PathNode(res, curNode.Weight + 1, curNode));
            }
            if (next == destination)
            {
                nodes.Last().Closed = true;
            }
            return true;
        }
        else
        {
            if (GridManagerScript.Instance.IsWalkableAndFree(curNode.Pos, chance2, walkingSide) && (curNode.Previous != null ? curNode.Previous.Pos.Contains(chance2) : true))
            {
                List<Vector2Int> res = new List<Vector2Int>();
                curNode.Pos.ForEach(r => res.Add((r - curNode.Pos[0]) + chance2));
                nodes.Add(new PathNode(res, curNode.Weight + 1, curNode));
                if (chance2 == destination)
                {
                    nodes.Last().Closed = true;
                }
                return true;
            }
            if(GridManagerScript.Instance.IsWalkableAndFree(curNode.Pos, chance3, walkingSide) && (curNode.Previous != null ? curNode.Previous.Pos.Contains(chance3) : true))
            {
                List<Vector2Int> res = new List<Vector2Int>();
                curNode.Pos.ForEach(r => res.Add((r - curNode.Pos[0]) + chance3));
                nodes.Add(new PathNode(res, curNode.Weight + 1, curNode));
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
    public List<Vector2Int> Pos;
    public int Weight = 0;
    public bool Closed = false;
    public bool Checked = false;
    public PathNode Previous = null;

    public PathNode()
    {

    }

    public PathNode(List<Vector2Int> pos, int weight, PathNode originNode)
    {
        Pos = pos;
        Weight = weight;
        Previous = originNode;
    }
}
