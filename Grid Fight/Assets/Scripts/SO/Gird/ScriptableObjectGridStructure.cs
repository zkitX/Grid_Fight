﻿using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GridStructure")]
public class ScriptableObjectGridStructure : ScriptableObject
{
    public List<BattleTileInfo> GridInfo = new List<BattleTileInfo>();
    public Vector2 minMaxAttackStats = new Vector2(0f, 1f);
    public Vector2 minMaxDefenceStats = new Vector2(0f, 1f);
    [HideInInspector] public int YGridSeparator;
   /* public Vector3 CameraPosition;
    public float OrthographicSize;*/


    private void OnEnable()
    {
        if (GridInfo.Count == 0)
        {
            for (int x = 0; x < 6; x++) //replace this in the future with a variable used by gridmanager script
            {
                for (int y = 0; y < 12; y++)
                {
                    GridInfo.Add(new BattleTileInfo(new Vector2Int(x, y)));
                }
            }
        }
        UpdateTileAdStats();
    }

    private void OnValidate()
    {
        GridInfo.OrderBy(r => Mathf.Sqrt(r.Pos.x) + Mathf.Sqrt(r.Pos.y));
        UpdateTileAdStats();
    }

    void UpdateTileAdStats()
    {
        for (int x = 0; x < 6; x++)
        {
            BattleTileInfo[] rowTiles = GridInfo.Where(r => r.Pos.x == x && r._BattleTileState != BattleTileStateType.NonUsable).ToArray();
            if(rowTiles != null && rowTiles.Length != 0)
            {

                BattleTileInfo[] allyRowTiles = rowTiles.Where(r => r.WalkingSide == WalkingSideType.LeftSide).ToArray();
                if(allyRowTiles != null)
                {
                    if (allyRowTiles.Length == 1) allyRowTiles[0].TileADStats = allyRowTiles[0].OverrideTileADStats ? allyRowTiles[0].TileADStats : new Vector2(minMaxAttackStats.y, minMaxDefenceStats.y);
                    else
                    {
                        for (int i = 0; i < allyRowTiles.Length; i++)
                        {
                            float progress = 0;
                            if (!allyRowTiles[i].OverrideTileADStats)
                            {
                                progress = ((float)i) / ((float)allyRowTiles.Length - 1f);
                                allyRowTiles[i].TileADStats.x = Mathf.Lerp(minMaxAttackStats.x, minMaxAttackStats.y, progress);
                                allyRowTiles[i].TileADStats.y = Mathf.Lerp(minMaxDefenceStats.y, minMaxDefenceStats.x, progress);
                            }
                        }
                    }
                }

                BattleTileInfo[] enemyRowTiles = rowTiles.Where(r => r.WalkingSide == WalkingSideType.RightSide).ToArray();
                if (enemyRowTiles != null)
                {
                    if (enemyRowTiles.Length == 1) enemyRowTiles[0].TileADStats = enemyRowTiles[0].OverrideTileADStats ? enemyRowTiles[0].TileADStats : new Vector2(minMaxAttackStats.y, minMaxDefenceStats.y);
                    else
                    {
                        for (int i = 0; i < enemyRowTiles.Length; i++)
                        {
                            float progress = 0;
                            if (!enemyRowTiles[i].OverrideTileADStats)
                            {
                                progress = ((float)i) / ((float)enemyRowTiles.Length - 1f);
                                enemyRowTiles[i].TileADStats.x = Mathf.Lerp(minMaxAttackStats.y, minMaxAttackStats.x, progress);
                                enemyRowTiles[i].TileADStats.y = Mathf.Lerp(minMaxDefenceStats.x, minMaxDefenceStats.y, progress);
                            }
                        }
                    }
                }

            }
        }
    }
}

[System.Serializable]
public class BattleTileInfo
{
    public string name;
    public Vector2Int Pos;
    public bool HasEffect = false;
    [ConditionalField("HasEffect", false)] public List<ScriptableObjectAttackEffect> Effects = new List<ScriptableObjectAttackEffect>();
    public BattleTileStateType _BattleTileState;
    //public BattleTileType BattleTileT;
    public WalkingSideType WalkingSide;
    public PortalType Portal;
    public int IDPortal;
    public Sprite TileSprite;
    public bool OverrideTileADStats = false;
    [ConditionalField("OverrideTileADStats")] public Vector2 TileADStats = Vector2.negativeInfinity;

    public BattleTileStateType BattleTileState
    {
        get
        {
            return _BattleTileState;
        }
        set
        {
            _BattleTileState = value;
        }
    }

    public BattleTileInfo(Vector2Int pos)
    {
        Pos = pos;
    }
}