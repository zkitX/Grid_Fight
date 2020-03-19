using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(ScriptableObjectGridStructure))]
public class ScriptableObjectGridStructureEditor : Editor
{

    public Dictionary<GridTileInfo, bool> TilesInfo = new Dictionary<GridTileInfo, bool>();
    bool firstOpen = true;
    GridTileInfo differentGti = null;


    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        bool showClose = true;
        base.OnInspectorGUI();
        //test = false;
        ScriptableObjectGridStructure origin = (ScriptableObjectGridStructure)target;
        BattleTileInfo bti;
        GridTileInfo gti = null;
        if(origin.GridInfo.Count > 0)
        {
            EditorGUILayout.Space();
            for (int x = 0; x < 6; x++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int y = 0; y < 12; y++)
                {
                    //Debug.Log(x + "   " + y);
                    bti = origin.GridInfo.Where(r => r.Pos == new Vector2Int(x, y)).First();
                    if (firstOpen)
                    {
                        gti = new GridTileInfo(new Vector2Int(x, y), bti);
                        TilesInfo.Add(gti, bti.BattleTileState == BattleTileStateType.Empty ? true : false);
                    }
                    else
                    {
                        gti = TilesInfo.Where(r => r.Key.Pos == new Vector2Int(x, y)).First().Key;
                    }

                    showClose = EditorGUILayout.ToggleLeft(x + "," + y, TilesInfo.Where(r => r.Key.Pos == new Vector2Int(x, y)).First().Value, GUILayout.Width(40));
                    if (showClose != TilesInfo[gti])
                    {
                        if (showClose)
                        {
                            differentGti = gti;

                        }
                        else
                        {
                            differentGti = null;

                        }
                    }
                    TilesInfo[gti] = showClose;
                    bti.BattleTileState = showClose ? BattleTileStateType.Empty : BattleTileStateType.Blocked;
                    //Debug.Log(showClose);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (differentGti != null)
            {
                ShowTileObject(ref differentGti.Tile);
            }
            firstOpen = false;
        }
        
    }


    private void ShowTileObject(ref BattleTileInfo bti)
    {
        //bti.BattleTileT = (BattleTileType)EditorGUILayout.EnumPopup("BattleTileType", bti.BattleTileT);
        bti.WalkingSide = (WalkingSideType)EditorGUILayout.EnumPopup("WalkingSideType", bti.WalkingSide);
        bti.Effect = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect", bti.Effect, typeof(ScriptableObjectAttackEffect), true);
        bti.TileSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", bti.TileSprite, typeof(Sprite), true, GUILayout.Width(512), GUILayout.Height(512));
    }
}





public class GridTileInfo
{
    public Vector2Int Pos;
    public BattleTileInfo Tile;

    public GridTileInfo(Vector2Int pos, BattleTileInfo tile)
    {
        Pos = pos;
        Tile = tile;
    }
}