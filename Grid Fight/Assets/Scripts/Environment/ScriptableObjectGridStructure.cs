using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GridStructure")]
public class ScriptableObjectGridStructure : ScriptableObject
{
    public Vector2Int BattleFieldSize;
    public List<BattleTileInfo> GridInfo = new List<BattleTileInfo>();
    public CameraBasePosType CameraBasePos;
}

[System.Serializable]
public class BattleTileInfo
{
    public string name;
    public Vector2Int Pos;
    public BattleTileStateType BattleTileState;
    public BattleTileType BattleTileT;
    public ControllerType TileOwner;
}