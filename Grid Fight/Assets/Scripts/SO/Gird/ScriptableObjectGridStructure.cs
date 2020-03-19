using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GridStructure")]
public class ScriptableObjectGridStructure : ScriptableObject
{
    [HideInInspector] public List<BattleTileInfo> GridInfo = new List<BattleTileInfo>();
    public int YGridSeparator;
    public Vector3 CameraPosition;
    public float OrthographicSize;

}

[System.Serializable]
public class BattleTileInfo
{
    public string name;
    public Vector2Int Pos;
    public ScriptableObjectAttackEffect Effect;
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
}