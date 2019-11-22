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
    public int YGridSeparator;

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
        YGridSeparator = gridStructure.YGridSeparator;
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
  
    //Checking if the given positions are part of the desired movent area
    public bool AreBattleTilesInControllerArea(List<Vector2Int> pos, SideType isEnemyOrPlayer)
    {
        bool AreInControlledArea = false;
        foreach (Vector2Int item in pos)
        {
            AreInControlledArea = BattleTiles.Where(r => r.Pos == item && (r.TileSide == isEnemyOrPlayer || isEnemyOrPlayer == SideType.Both)).ToList().Count > 0 ? true : false;
            if (!AreInControlledArea)
            {
                break;
            }
        }

        return AreInControlledArea;
    }

    public bool isPosOnField(Vector2Int pos)
    {
        return BattleTiles.Where(r => r.Pos == pos).ToList().Count > 0 ? true : false;
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

    public List<BattleTileScript> GetBattleTiles(List<Vector2Int> pos, FacingType isEnemyOrPlayer, SideType side)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        foreach (Vector2Int item in pos)
        {
            res.Add(GetBattleTile(item, side));
        }
        return res;
    }

    public BattleTileScript GetBattleTile(Vector2Int pos)
    {
        return BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
    }

    //Get BattleTileScript of the tile
    public BattleTileScript GetBattleTile(Vector2Int pos, SideType side)//isEnemyOrPlayer = true/Player false/Enemy
    {
        return BattleTiles.Where(r => r.Pos == pos && (r.TileSide == side || side == SideType.Both)).FirstOrDefault();
    }

    public List<BattleTileScript> GetBattleTileInARowToDestination(Vector2Int destPos, FacingType isEnemyOrPlayer, int startingColumn)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        if(isEnemyOrPlayer == FacingType.Right)
        {
            for (int i = YGridSeparator <= startingColumn ? startingColumn : YGridSeparator; i <= destPos.y; i++)
            {
                res.Add(GetBattleTile(new Vector2Int(destPos.x, i)));
            }
        }
        else
        {
            for (int i = YGridSeparator > startingColumn ? startingColumn : YGridSeparator - 1; i >= destPos.y; i--)
            {
                res.Add(GetBattleTile(new Vector2Int(destPos.x, i)));
            }
        }
        return res;
    }

    public float GetWorldDistanceBetweenTiles()
    {
        return Vector3.Distance(BattleTiles[0].transform.position, BattleTiles[1].transform.position);
    }

    public void SetBattleTileState(Vector2Int pos, BattleTileStateType battleTileState)
    {
        BattleTiles.Where(r => r.Pos == pos).FirstOrDefault().BattleTileState = battleTileState;
    }
    //Get free tile for a one tile character
    public BattleTileScript GetFreeBattleTile(SideType isEnemyOrPlayer)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && (r.TileSide == isEnemyOrPlayer || isEnemyOrPlayer == SideType.Both)).ToList();
        int battletileCount = emptyBattleTiles.Count;
        return emptyBattleTiles[Random.Range(0, battletileCount)];
    }
    //Get free tile for a more than one tile character
    public BattleTileScript GetFreeBattleTile(SideType isEnemyOrPlayer, List<Vector2Int> occupiedTiles)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && (r.TileSide == isEnemyOrPlayer || isEnemyOrPlayer == SideType.Both)).ToList();
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