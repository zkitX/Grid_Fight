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
    private void Awake()
    {
        Instance = this;

        foreach (BattleTileScript item in FindObjectsOfType<BattleTileScript>())
        {
            BattleTiles.Add(item);
        }
      
    }

    private void Start()
    {
        
    }
    //Setup each single tiles of the grid
    public void SetupGrid(ScriptableObjectGridStructure gridStructure)
    {
        foreach (BattleTileInfo tile in gridStructure.GridInfo)
        {
            //Debug.Log(tile.Pos);
            BattleTiles.Where(r => r.Pos == tile.Pos).First().SetupTile(tile);
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

    //Get BattleTileScript of the tile
    public BattleTileScript GetBattleTile(Vector2Int pos, bool isEnemyOrPlayer)//isEnemyOrPlayer = true/Player false/Enemy
    {
        if (isEnemyOrPlayer)
        {
            return BattleTiles.Where(r => r.Pos == pos && r.TileOwner != ControllerType.Enemy).FirstOrDefault();
        }
        else
        {
            return BattleTiles.Where(r => r.Pos == pos && r.TileOwner == ControllerType.Enemy).FirstOrDefault();
        }
        
    }

    public BattleTileScript GetBattleTile(Vector2Int pos)
    {
        return BattleTiles.Where(r => r.Pos == pos).FirstOrDefault();
    }

    public void SetBattleTileState(Vector2Int pos, BattleTileStateType battleTileState)
    {
        BattleTiles.Where(r => r.Pos == pos).FirstOrDefault().BattleTileState = battleTileState;
    }

    public BattleTileScript GetFreeBattleTile(bool isEnemyOrPlayer)//isEnemyOrPlayer = true/Player false/Enemy
    {
        List<BattleTileScript> emptyBattleTiles = new List<BattleTileScript>();
        if (isEnemyOrPlayer)
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
}
