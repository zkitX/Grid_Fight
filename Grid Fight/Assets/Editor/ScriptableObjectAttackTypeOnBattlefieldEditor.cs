using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObjectAttackTypeOnBattlefield))]
public class ScriptableObjectAttackTypeOnBattlefieldEditor : Editor
{

    public List<BattleFieldTileInfo> TilesInfo = new List<BattleFieldTileInfo>();
    bool firstOpen = true;
    BattleFieldAttackTileClass differentbfatc = null;

    BattleFieldAttackTileClass[] selection;
    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        base.OnInspectorGUI();
        //test = false;
        ScriptableObjectAttackTypeOnBattlefield origin = (ScriptableObjectAttackTypeOnBattlefield)target;
        
        
        if (origin.BulletTrajectories.Count > 0)
        {
            if (firstOpen)
            {
                selection = new BattleFieldAttackTileClass[origin.BulletTrajectories.Count];
            } 

            for (int i = 0; i < origin.BulletTrajectories.Count; i++)
            {
                
                EditorGUILayout.Space();
                switch (origin.AtkType)
                {
                    case BattleFieldAttackType.OnAreaAttack:
                        OnAreaAtk(origin, i);
                        break;
                    case BattleFieldAttackType.OnTarget:
                        OnTargetAtk(origin, i);
                        break;
                    case BattleFieldAttackType.OnItSelf:
                        OnHerSelf(origin, i);
                        break;
                    default:
                        break;
                }
            }
            firstOpen = false;
        }

    }

    public void OnAreaAtk(ScriptableObjectAttackTypeOnBattlefield origin, int i)
    {
        bool showClose = true;

        BattleFieldTileInfo bfti = null;
        BattleFieldAttackTileClass bfatc;
        for (int x = 0; x < 6; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = 0; y < 12; y++)
            {
                //Debug.Log(x + "   " + y);
                bfatc = origin.BulletTrajectories[i].BulletEffectTiles.Where(r => r.Pos == new Vector2Int(x, y)).FirstOrDefault();
                if (firstOpen && bfatc != null)
                {
                    bfti = new BattleFieldTileInfo(origin.BulletTrajectories[i], bfatc);
                    TilesInfo.Add(bfti);
                }
                showClose = EditorGUILayout.ToggleLeft(x + "," + y, bfatc != null ? true : false, GUILayout.Width(40));
                if (showClose)
                {
                    if (bfatc == null)
                    {
                        bfatc = new BattleFieldAttackTileClass(new Vector2Int(x, y));
                        bfti = new BattleFieldTileInfo(origin.BulletTrajectories[i], bfatc);
                        origin.BulletTrajectories[i].BulletEffectTiles.Add(bfatc);
                        TilesInfo.Add(bfti);
                    }

                    differentbfatc = bfatc;
                }
                else if (!showClose && bfatc != null)
                {
                    TilesInfo.Remove(bfti);
                    origin.BulletTrajectories[i].BulletEffectTiles.Remove(bfatc);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (differentbfatc != null)
        {
            selection[i] = differentbfatc;
            ShowTileObject(ref differentbfatc);
        }
        else if (selection[i] != null)
        {
            ShowTileObject(ref selection[i]);
        }
        differentbfatc = null;


    }

    public void OnTargetAtk(ScriptableObjectAttackTypeOnBattlefield origin, int i)
    {
        bool showClose = true;

        BattleFieldTileInfo bfti = null;
        BattleFieldAttackTileClass bfatc;
        for (int x = -3; x < 4; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = -3; y < 4; y++)
            {
                //Debug.Log(x + "   " + y);
              
                bfatc = origin.BulletTrajectories[i].BulletEffectTiles.Where(r => r.Pos == new Vector2Int(x, y)).FirstOrDefault();
                if (firstOpen && bfatc != null)
                {
                    bfti = new BattleFieldTileInfo(origin.BulletTrajectories[i], bfatc);
                    TilesInfo.Add(bfti);
                }
                if (x == 0 && y == 0)
                {
                    showClose = EditorGUILayout.ToggleLeft("T", bfatc != null ? true : false, GUILayout.Width(30));
                }
                else
                {
                    showClose = EditorGUILayout.ToggleLeft("", bfatc != null ? true : false, GUILayout.Width(30));
                }

                if (showClose)
                {
                    if (bfatc == null)
                    {
                        bfatc = new BattleFieldAttackTileClass(new Vector2Int(x, y));
                        bfti = new BattleFieldTileInfo(origin.BulletTrajectories[i], bfatc);
                        origin.BulletTrajectories[i].BulletEffectTiles.Add(bfatc);
                        TilesInfo.Add(bfti);
                    }

                    differentbfatc = bfatc;
                }
                else if (!showClose && bfatc != null)
                {
                    TilesInfo.Remove(bfti);
                    origin.BulletTrajectories[i].BulletEffectTiles.Remove(bfatc);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (differentbfatc != null)
        {
            selection[i] = differentbfatc;
            ShowTileObject(ref differentbfatc);
        }
        else if (selection[i] != null)
        {
            ShowTileObject(ref selection[i]);
        }
        differentbfatc = null;
    }

    public void OnHerSelf(ScriptableObjectAttackTypeOnBattlefield origin, int i)
    {
        bool showClose = true;

        BattleFieldTileInfo bfti = null;
        BattleFieldAttackTileClass bfatc;
        for (int x = -3; x < 4; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = -3; y < 4; y++)
            {
                //Debug.Log(x + "   " + y);

                bfatc = origin.BulletTrajectories[i].BulletEffectTiles.Where(r => r.Pos == new Vector2Int(x, y)).FirstOrDefault();
                if (firstOpen && bfatc != null)
                {
                    bfti = new BattleFieldTileInfo(origin.BulletTrajectories[i], bfatc);
                    TilesInfo.Add(bfti);
                }
                if (x == 0 && y == 0)
                {
                    showClose = EditorGUILayout.ToggleLeft("T", bfatc != null ? true : false, GUILayout.Width(30));
                }
                else
                {
                    showClose = EditorGUILayout.ToggleLeft("", bfatc != null ? true : false, GUILayout.Width(30));
                }

                if (showClose)
                {
                    if (bfatc == null)
                    {
                        bfatc = new BattleFieldAttackTileClass(new Vector2Int(x, y));
                        bfti = new BattleFieldTileInfo(origin.BulletTrajectories[i], bfatc);
                        origin.BulletTrajectories[i].BulletEffectTiles.Add(bfatc);
                        TilesInfo.Add(bfti);
                    }

                    differentbfatc = bfatc;
                }
                else if (!showClose && bfatc != null)
                {
                    TilesInfo.Remove(bfti);
                    origin.BulletTrajectories[i].BulletEffectTiles.Remove(bfatc);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (differentbfatc != null)
        {
            selection[i] = differentbfatc;
            ShowTileObject(ref differentbfatc);
        }
        else if (selection[i] != null)
        {
            ShowTileObject(ref selection[i]);
        }
        differentbfatc = null;
    }


    private void ShowTileObject(ref BattleFieldAttackTileClass bfatc)
    {
        bfatc.HasEffect = EditorGUILayout.ToggleLeft("HasEffect", bfatc.HasEffect);
        if (bfatc.HasEffect)
        {
            var list = bfatc.Effects;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("size", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            for (int i = 0; i < list.Count; i++)
            {
                bfatc.Effects[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect", bfatc.Effects[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
            }
        }

        bfatc.HasDifferentParticles = EditorGUILayout.ToggleLeft("HasDifferentParticles", bfatc.HasDifferentParticles);
        if (bfatc.HasDifferentParticles)
        {
            bfatc.ParticlesID = (AttackParticleType)EditorGUILayout.EnumPopup("AttackParticleType", bfatc.ParticlesID);
        }

        bfatc.IsEffectOnTile = EditorGUILayout.ToggleLeft("HasEffectOnTile", bfatc.IsEffectOnTile);
        if (bfatc.IsEffectOnTile)
        {
            bfatc.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", bfatc.TileParticlesID);
            bfatc.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", 0);
        }
    }
}

public class BattleFieldTileInfo
{
    public BulletBehaviourInfoClassOnBattleFieldClass Parent;
    public BattleFieldAttackTileClass Tile;

    public BattleFieldTileInfo(BulletBehaviourInfoClassOnBattleFieldClass parent, BattleFieldAttackTileClass tile)
    {
        Parent = parent;
        Tile = tile;
    }
}
