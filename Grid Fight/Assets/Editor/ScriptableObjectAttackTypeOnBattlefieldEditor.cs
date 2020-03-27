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
                origin.BulletTrajectories[i].Delay = EditorGUILayout.FloatField("Delay", origin.BulletTrajectories[i].Delay);
                switch (origin.AtkType)
                {
                    case BattleFieldAttackType.OnAreaAttack:
                        Draw(origin.BulletTrajectories[i], new Vector2Int(0,6), new Vector2Int(0,12));
                        break;
                    case BattleFieldAttackType.OnTarget:
                        Draw(origin.BulletTrajectories[i], new Vector2Int(-3,4), new Vector2Int(-3, 4));
                        break;
                    case BattleFieldAttackType.OnItSelf:
                        Draw(origin.BulletTrajectories[i], new Vector2Int(-3,4), new Vector2Int(-3, 4));
                        break;
                    default:
                        break;
                }
            }
            firstOpen = false;
        }

    }

    
    public void Draw(BulletBehaviourInfoClassOnBattleFieldClass origin, Vector2Int horizontal, Vector2Int vertical)
    {
        bool showClose = true;

        BattleFieldTileInfo bfti = null;
        BattleFieldAttackTileClass bfatc;
        for (int x = horizontal.x; x < horizontal.y; x++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int y = vertical.x; y < vertical.y; y++)
            {
                //Debug.Log(x + "   " + y);

                bfatc = origin.BulletEffectTiles.Where(r => r.Pos == new Vector2Int(x, y)).FirstOrDefault();
                if (firstOpen && bfatc != null)
                {
                    bfti = new BattleFieldTileInfo(origin, bfatc);
                    TilesInfo.Add(bfti);
                }
                else 
                {
                    bfti = TilesInfo.Where(r => r.Tile.Pos == new Vector2Int(x, y)).FirstOrDefault();
                }
                showClose = EditorGUILayout.ToggleLeft(x + "," + y, bfatc != null ? true : false, GUILayout.Width(40));

                if (showClose)
                {
                    if (bfatc == null)
                    {
                        if (TilesInfo.Count == 0)
                        {
                            bfatc = new BattleFieldAttackTileClass(new Vector2Int(x, y));
                        }
                        else
                        {
                            ScriptableObjectAttackEffect[] copyOfEffects = new ScriptableObjectAttackEffect[TilesInfo[TilesInfo.Count - 1].Tile.Effects.Count];
                            TilesInfo[TilesInfo.Count - 1].Tile.Effects.CopyTo(copyOfEffects);
                            bfatc = new BattleFieldAttackTileClass(new Vector2Int(x, y), TilesInfo[TilesInfo.Count -1].Tile.HasEffect, copyOfEffects.ToList(),
                                TilesInfo[TilesInfo.Count - 1].Tile.HasDifferentParticles, TilesInfo[TilesInfo.Count - 1].Tile.ParticlesID, TilesInfo[TilesInfo.Count - 1].Tile.IsEffectOnTile,
                                TilesInfo[TilesInfo.Count - 1].Tile.TileParticlesID, TilesInfo[TilesInfo.Count - 1].Tile.DurationOnTile);
                        }
                        bfti = new BattleFieldTileInfo(origin, bfatc);
                        origin.BulletEffectTiles.Add(bfatc);
                        TilesInfo.Add(bfti);
                    }
                }
                else if (!showClose && bfatc != null)
                {
                    TilesInfo.Remove(bfti);
                    origin.BulletEffectTiles.Remove(bfatc);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (TilesInfo.Count > 0)
        {
            WriteInfo(origin);
        }
    }


    private void WriteInfo(BulletBehaviourInfoClassOnBattleFieldClass origin)
    {
        origin.Show = EditorGUILayout.Foldout(origin.Show, "Tiles");
        if (origin.Show)
        {
            foreach (BattleFieldTileInfo item in TilesInfo.Where(r => r.Parent == origin).ToList())
            {
                item.Show = EditorGUILayout.Foldout(item.Show, item.Tile.Pos.ToString());
                if (item.Show)
                {
                    ShowTileObject(ref item.Tile);
                }
            }
        }
    }


    private void ShowTileObject(ref BattleFieldAttackTileClass bfatc)
    {
        bfatc.HasEffect = EditorGUILayout.ToggleLeft("HasEffect", bfatc.HasEffect);
        if (bfatc.HasEffect)
        {
            bfatc.EffectChances = EditorGUILayout.FloatField("EffectChances", bfatc.EffectChances);
            var list = bfatc.Effects;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            for (int i = 0; i < list.Count; i++)
            {
                bfatc.Effects[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + i, bfatc.Effects[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
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
            bfatc.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", bfatc.DurationOnTile);
        }
    }
}

public class BattleFieldTileInfo
{
    public BulletBehaviourInfoClassOnBattleFieldClass Parent;
    public BattleFieldAttackTileClass Tile;
    public bool Show = true;
    public BattleFieldTileInfo(BulletBehaviourInfoClassOnBattleFieldClass parent, BattleFieldAttackTileClass tile)
    {
        Parent = parent;
        Tile = tile;
    }
}
