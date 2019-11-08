using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManagerScript : MonoBehaviour
{

    public delegate void InitializationComplete();
    public event InitializationComplete InitializationCompleteEvent;
    public static GridManagerScript Instance;
    public List<BattleTileScript> BattleTiles = new List<BattleTileScript>();
    public Vector2Int BattleFieldSize;

    public List<PortalInfoClass> Portals = new List<PortalInfoClass>();

    private void Awake()
    {
        Instance = this;
        //Getting all the BattleTileScript
        foreach (BattleTileScript item in FindObjectsOfType<BattleTileScript>())
        {
            BattleTiles.Add(item);
        }
      
    }

    //Setup each single tiles of the grid
    public void SetupGrid(ScriptableObjectGridStructure gridStructure)
    {
        foreach (BattleTileInfo tile in gridStructure.GridInfo)
        {
            //Debug.Log(tile.Pos);
            BattleTiles.Where(r => r.Pos == tile.Pos).First().SetupTileFromBattleTileInfo(tile);
        }

        if(InitializationCompleteEvent != null)
        {
            InitializationCompleteEvent();
        }
    }

    public void ResetGrid()
    {
        foreach (BattleTileScript tile in BattleTiles)
        {
            BattleTiles.Where(r => r.Pos == tile.Pos).First().ResetTile();
        }
    }


    //Checking if the given position is part of the desired movent area
    public bool IsBattleTileInControllerArea(Vector2Int pos, bool isEnemyOrPlayer)
    {
        if (isEnemyOrPlayer)
        {
            return BattleTiles.Where(r => r.Pos == pos && r.TileOwner != ControllerType.Enemy).ToList().Count > 0 ? true : false;
        }
        else
        {
            return BattleTiles.Where(r => r.Pos == pos && r.TileOwner == ControllerType.Enemy).ToList().Count > 0 ? true : false;
        }
    }
    //Checking if the given positions are part of the desired movent area
    public bool AreBattleTilesInControllerArea(List<Vector2Int> pos, SideType isEnemyOrPlayer)
    {
        bool AreInControlledArea = false;
        if (isEnemyOrPlayer == SideType.LeftSide)
        {
            foreach (Vector2Int item in pos)
            {
                AreInControlledArea = BattleTiles.Where(r => r.Pos == item && r.TileOwner != ControllerType.Enemy).ToList().Count > 0 ? true : false;
                if(!AreInControlledArea)
                {
                    break;
                }
            }
        }
        else
        {
            foreach (Vector2Int item in pos)
            {
                AreInControlledArea = BattleTiles.Where(r => r.Pos == item && r.TileOwner == ControllerType.Enemy).ToList().Count > 0 ? true : false;
                if (!AreInControlledArea)
                {
                    break;
                }
            }
        }

        return AreInControlledArea;
    }

    

    public SideType GetSideTypeFromControllerType(ControllerType ct)
    {
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        switch (matchType)
        {
            case MatchType.PvE:
                if (ct == ControllerType.Enemy)
                {
                    return SideType.RightSide;
                }
                else
                {
                    return SideType.LeftSide;
                }
            case MatchType.PvP:
                if (ct == ControllerType.Player2)
                {
                    return SideType.RightSide;
                }
                else
                {
                    return SideType.LeftSide;
                }
            case MatchType.PPvE:
                if (ct == ControllerType.Enemy)
                {
                    return SideType.RightSide;
                }
                else
                {
                    return SideType.LeftSide;
                }
            case MatchType.PPvPP:
                if (ct == ControllerType.Player3 || ct == ControllerType.Player4)
                {
                    return SideType.RightSide;
                }
                else
                {
                    return SideType.LeftSide;
                }
        }

        return SideType.LeftSide;
    }

    public List<BattleTileScript> GetBattleTiles(List<Vector2Int> pos, SideType isEnemyOrPlayer)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        if (isEnemyOrPlayer == SideType.LeftSide)
        {
            foreach (Vector2Int item in pos)
            {
                res.Add(GetBattleTile(item, isEnemyOrPlayer));
            }
        }
        else
        {
            foreach (Vector2Int item in pos)
            {
                res.Add(GetBattleTile(item, isEnemyOrPlayer));
            }
        }
        return res;
    }

    public BattleTileScript GetBattleTile(Vector2Int pos)
    {
        return BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
    }

    //Get BattleTileScript of the tile
    public BattleTileScript GetBattleTile(Vector2Int pos, SideType isEnemyOrPlayer)//isEnemyOrPlayer = true/Player false/Enemy
    {
        if (isEnemyOrPlayer == SideType.LeftSide)
        {
            return BattleTiles.Where(r => r.Pos == pos && r.TileOwner != ControllerType.Enemy).FirstOrDefault();
        }
        else
        {
            return BattleTiles.Where(r => r.Pos == pos && r.TileOwner == ControllerType.Enemy).FirstOrDefault();
        }

    }

    public List<BattleTileScript> GetBattleTileInARowToDestination(Vector2Int destPos, SideType isEnemyOrPlayer)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        if(isEnemyOrPlayer == SideType.LeftSide)
        {
            for (int i = 6; i < destPos.y; i++)
            {
                res.Add(GetBattleTile(new Vector2Int(destPos.x, i)));
            }
        }
        else
        {
            for (int i = 5; i > destPos.y; i--)
            {
                res.Add(GetBattleTile(new Vector2Int(destPos.x, i)));
            }
        }



        return res;

    }


    public void SetBattleTileState(Vector2Int pos, BattleTileStateType battleTileState)
    {
        BattleTiles.Where(r => r.Pos == pos).FirstOrDefault().BattleTileState = battleTileState;
    }
    //Get free tile for a one tile character
    public BattleTileScript GetFreeBattleTile(SideType isEnemyOrPlayer)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        if (isEnemyOrPlayer == SideType.LeftSide)
        {
            emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && r.TileOwner != ControllerType.Enemy).ToList();
        }
        else
        {
            emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && r.TileOwner == ControllerType.Enemy).ToList();
        }
        
        int battletileCount = emptyBattleTiles.Count;
        
        return emptyBattleTiles[Random.Range(0, battletileCount)];
    }
    //Get free tile for a more than one tile character
    public BattleTileScript GetFreeBattleTile(SideType isEnemyOrPlayer, List<Vector2Int> occupiedTiles)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        if (isEnemyOrPlayer == SideType.LeftSide)
        {
            emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && r.TileOwner != ControllerType.Enemy).ToList();
        }
        else
        {
            emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && r.TileOwner == ControllerType.Enemy).ToList();
        }

        bool areOccupiedTileFree = true;
        BattleTileScript emptyTile = null;
        while (emptyBattleTiles.Count > 0)
        {
            emptyTile = emptyBattleTiles[Random.Range(0, emptyBattleTiles.Count)];
            emptyBattleTiles.Remove(emptyTile);
            areOccupiedTileFree = true;
            foreach (Vector2Int item in occupiedTiles)
            {
                BattleTileScript tileToCheck = BattleTiles.Where(r => r.Pos == emptyTile.Pos + item).FirstOrDefault();
                if (tileToCheck == null)
                {
                    areOccupiedTileFree = false;
                    break;
                }
                else
                {
                    if (tileToCheck.BattleTileState != BattleTileStateType.Empty)
                    {
                        areOccupiedTileFree = false;
                        break;
                    }
                } 
            }
            if(areOccupiedTileFree)
            {
                return emptyTile;
            }
        }
        return null;
    }

}


public class PortalInfoClass
{
    public BattleTileScript PortalPos;
    public PortalType Portal;
    public int IDPortal;

    public PortalInfoClass()
    {
    }

    public PortalInfoClass(BattleTileScript portalPos, PortalType portal, int idPortal)
    {
        PortalPos = portalPos;
        Portal = portal;
        IDPortal = idPortal;
    }
}