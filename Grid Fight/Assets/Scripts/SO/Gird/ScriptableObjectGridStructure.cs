using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GridStructure")]
public class ScriptableObjectGridStructure : ScriptableObject
{
    public List<BattleTileInfo> GridInfo = new List<BattleTileInfo>();
    public int YGridSeparator;
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