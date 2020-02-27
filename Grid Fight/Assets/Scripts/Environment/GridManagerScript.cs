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
    public GameObject TargetIndicator;

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
            //Debug.Log(tile.Pos + "   " + tile.BattleTileState.ToString());
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
    public bool AreBattleTilesInControllerArea(List<Vector2Int> pos, WalkingSideType walkingSide)
    {
        bool AreInControlledArea = false;
        foreach (Vector2Int item in pos)
        {
            AreInControlledArea = BattleTiles.Where(r => r.Pos == item && (r.WalkingSide == walkingSide || walkingSide == WalkingSideType.Both) && r._BattleTileState == BattleTileStateType.Empty).ToList().Count > 0 ? true : false;
            if (!AreInControlledArea)
            {
                break;
            }
        }

        return AreInControlledArea;
    }

    public bool isPosOnField(Vector2Int pos)
    {
        return BattleTiles.Where(r => r.Pos == pos && r.BattleTileState > BattleTileStateType.Blocked).ToList().Count > 0 ? true : false;
    }

    public bool isPosOnFieldByHeight(Vector2Int pos)
    {
        return BattleTiles.Where(r => r.Pos.x == pos.x && r.BattleTileState != BattleTileStateType.Blocked).ToList().Count > 0 ? true : false;
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


    public Vector2Int GetVectorFromDirection(InputDirection dir)
    {
        switch (dir)
        {
            case InputDirection.Up:
                return new Vector2Int(-1, 0);
            case InputDirection.Down:
                return new Vector2Int(1, 0);
            case InputDirection.Left:
                return new Vector2Int(0, 1);
            case InputDirection.Right:
                return new Vector2Int(0, -1);
        }
        return new Vector2Int(0, -1);
    }

    public List<BattleTileScript> GetBattleTiles(List<Vector2Int> pos, WalkingSideType walkingSide)
    {
        List<BattleTileScript> res = new List<BattleTileScript>();
        foreach (Vector2Int item in pos)
        {
            res.Add(GetBattleTile(item, walkingSide));
        }
        return res;
    }

    public BattleTileScript GetBattleTile(Vector2Int pos)
    {
        return BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
    }


    public bool isPosFree(Vector2Int pos)
    {
        return GetBattleTile(pos).BattleTileState == BattleTileStateType.Empty ? true : false;
    }

    public BattleTileScript GetBattleBestTileInsideTheBattlefield(Vector2Int pos, FacingType facing)
    {
        BattleTileScript res = null;
        int startValue = pos.y;
        if (facing == FacingType.Left)
        {
            for (int i = startValue; i < YGridSeparator; i++)
            {
                pos.y = i;
                res = BattleTiles.Where(r => r.Pos == new Vector2Int(pos.x, i)).FirstOrDefault();
                if (res.BattleTileState != BattleTileStateType.Blocked)
                {
                    return res;
                }
            }
        }
        else
        {
            for (int i = startValue; i >= YGridSeparator; i--)
            {
                pos.y = i;
                res = BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
                if(res == null)
                {

                }

                if (res.BattleTileState != BattleTileStateType.Blocked)
                {
                    return res;
                }
            }
        }

        return res;
    }

    //Get BattleTileScript of the tile
    public BattleTileScript GetBattleTile(Vector2Int pos, WalkingSideType walkingSide)//isEnemyOrPlayer = true/Player false/Enemy
    {
        return BattleTiles.Where(r => r.Pos == pos && (r.WalkingSide == walkingSide || walkingSide == WalkingSideType.Both)).FirstOrDefault();
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
    public BattleTileScript GetFreeBattleTile(WalkingSideType walkingSide)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && (r.WalkingSide == walkingSide || walkingSide == WalkingSideType.Both)).ToList();
        int battletileCount = emptyBattleTiles.Count;
        return emptyBattleTiles[Random.Range(0, battletileCount)];
    }
    //Get free tile for a more than one tile character
    public BattleTileScript GetFreeBattleTile(WalkingSideType walkingSide, List<Vector2Int> occupiedTiles)
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        emptyBattleTiles = BattleTiles.Where(r => r.BattleTileState == BattleTileStateType.Empty && (r.WalkingSide == walkingSide || walkingSide == WalkingSideType.Both)).ToList();
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

    public void StartOnBattleFieldAttackCo(CharacterInfoScript cInfo, ScriptableObjectAttackTypeOnBattlefield atk, Vector2Int basePos, UnitManagementScript ums, BaseCharacter character)
    {

        basePos.y = ums.Facing == FacingType.Left ? YGridSeparator : YGridSeparator-1;

        foreach (BulletBehaviourInfoClassOnBattleField item in atk.BulletTrajectories)
        {
            foreach (Vector2Int target in item.BulletEffectTiles)
            {
                Vector2Int res = ums.Facing == FacingType.Left ? basePos - target : basePos + target;
                if (isPosOnField(res))
                {
                    BattleTileScript bts = GetBattleTile(res);
                    if(bts._BattleTileState != BattleTileStateType.Blocked)
                    {
                        bts.BattleTargetScript.SetAttack(item.Delay, BattleManagerScript.Instance.VFXScene ? cInfo.ParticleID : atk.ParticlesID, res, cInfo.DamageStats.BaseDamage, cInfo.Elemental, character);
                    }

                }

            }

        }

    }


    public bool IsEnemyOnTileAttackRange(List<Vector2Int> atkRange, Vector2Int basePos)
    {
        basePos.y = YGridSeparator;
        bool res = false;
        foreach (Vector2Int target in atkRange)
        {
            if (isPosOnField(basePos - target))
            {
                res = IsEnemyOnTile(basePos - target);
                if(res)
                {
                    return true;
                }
            }

        }
        return res;
    }

    public bool IsEnemyOnTile(Vector2Int pos)
    {
        return GetBattleTile(pos)._BattleTileState == BattleTileStateType.Occupied ? true : false;
    }



    public IEnumerator OnBattleFieldAttackCo(CharacterInfoScript cInfo, ScriptableObjectAttackTypeOnBattlefield atk, Vector2Int basePos, AttackParticleTypes atkPS)
    {
        basePos.y = YGridSeparator;
        foreach (BulletBehaviourInfoClassOnBattleField item in atk.BulletTrajectories)
        {
            float timer = 0;
            foreach (Vector2Int target in item.BulletEffectTiles)
            {
                if(isPosOnField(basePos - target))
                {
                    GameObject go;
                    go = Instantiate(TargetIndicator, GetBattleTile(basePos - target).transform.position, Quaternion.identity);
                    go.GetComponent<BattleTileTargetScript>().StartTarget(item.Delay, atkPS, basePos - target, cInfo.DamageStats.BaseDamage, cInfo.Elemental);
                }
            }
            while (timer <= item.Delay)
            {
                yield return BattleManagerScript.Instance.PauseUntil();
                timer += Time.fixedDeltaTime;
            }
        }
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