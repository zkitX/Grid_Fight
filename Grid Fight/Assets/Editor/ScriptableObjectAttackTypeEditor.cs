using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObjectAttackBase))]
public class ScriptableObjectAttackTypeEditor : Editor
{

    public List<BattleFieldTileInfo> TilesInfo = new List<BattleFieldTileInfo>();
    bool firstOpen = true;
    ScriptableObjectAttackBase origin;
    public GameObject ChargingActivationPs;
    public GameObject ChargingLoopPs;
    public GameObject PlaceHolder;


    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        base.OnInspectorGUI();
        //test = false;
        origin = (ScriptableObjectAttackBase)target;


        if (origin.Particles.Left.Cast == null)
        {
            origin.Particles.Left.Cast = PlaceHolder;
        }
        if (origin.Particles.Left.Bullet == null)
        {
            origin.Particles.Left.Bullet = PlaceHolder;
        }
        if (origin.Particles.Left.Hit == null)
        {
            origin.Particles.Left.Hit = PlaceHolder;
        }

        if (origin.Particles.Right.Cast == null)
        {
            origin.Particles.Right.Cast = PlaceHolder;
        }
        if (origin.Particles.Right.Bullet == null)
        {
            origin.Particles.Right.Bullet = PlaceHolder;
        }
        if (origin.Particles.Right.Hit == null)
        {
            origin.Particles.Right.Hit = PlaceHolder;
        }




        if (origin.Particles.CastActivationPS == null)
        {
            origin.Particles.CastActivationPS = ChargingActivationPs;
        }

        if (origin.Particles.CastLoopPS == null)
        {
            origin.Particles.CastLoopPS = ChargingLoopPs;
        }
        
        if(origin.AttackInput > AttackInputType.Strong)
        {
            origin.ExperiencePoints = EditorGUILayout.FloatField("ExperiencePoints", origin.ExperiencePoints);
        }


        if (origin.CurrentAttackType == AttackType.Particles)
        {
            if (origin.TrajectoriesNumber > 0)
            {

                origin.ParticlesAtk.BulletTrajectories = RefreshList(origin.TrajectoriesNumber, origin.ParticlesAtk.BulletTrajectories);
                for (int i = 0; i < origin.TrajectoriesNumber; i++)
                {
                    DrawParticlesAtk(origin.ParticlesAtk.BulletTrajectories[i]);
                }
            }
        }
        else if (origin.CurrentAttackType == AttackType.Tile)
        {
            origin.TilesAtk.AtkType = (BattleFieldAttackType)EditorGUILayout.EnumPopup("BattleFieldAttackType", origin.TilesAtk.AtkType);
            if (origin.TrajectoriesNumber > 0)
            {
                origin.TilesAtk.PercToCheck = EditorGUILayout.FloatField("PercToCheck", origin.TilesAtk.PercToCheck);
                origin.TilesAtk.StatToCheck = (WaveStatsType)EditorGUILayout.EnumPopup("StatToCheck", origin.TilesAtk.StatToCheck);
                origin.TilesAtk.ValueChecker = (ValueCheckerType)EditorGUILayout.EnumPopup("ValueChecker", origin.TilesAtk.ValueChecker);
                origin.TilesAtk.Chances = EditorGUILayout.IntField("Chances", origin.TilesAtk.Chances);
                origin.TilesAtk.BulletTrajectories = RefreshList(origin.TrajectoriesNumber, origin.TilesAtk.BulletTrajectories);
                for (int i = 0; i < origin.TrajectoriesNumber; i++)
                {
                    EditorGUILayout.Space();
                    if(i == 0)
                    {
                        origin.TilesAtk.BulletTrajectories[i].ExplosionChances = 100;
                    }
                    origin.TilesAtk.BulletTrajectories[i].Delay = EditorGUILayout.FloatField("Delay", origin.TilesAtk.BulletTrajectories[i].Delay);
                    if((i == 0 && origin.AttackInput == AttackInputType.Strong) || origin.AttackInput == AttackInputType.Weak)
                    {
                        origin.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile = EditorGUILayout.FloatField("BulletTravelDurationPerTile", origin.TilesAtk.BulletTrajectories[i].BulletTravelDurationPerTile);
                        origin.TilesAtk.BulletTrajectories[i].Trajectory_Y = EditorGUILayout.CurveField("Trajectory_Y", origin.TilesAtk.BulletTrajectories[i].Trajectory_Y);
                        origin.TilesAtk.BulletTrajectories[i].Trajectory_Z = EditorGUILayout.CurveField("Trajectory_Z", origin.TilesAtk.BulletTrajectories[i].Trajectory_Z);
                    }
                    origin.TilesAtk.BulletTrajectories[i].ExplosionChances = EditorGUILayout.IntField("ExplosionChances", origin.TilesAtk.BulletTrajectories[i].ExplosionChances);
                    switch (origin.TilesAtk.AtkType)
                    {
                        case BattleFieldAttackType.OnAreaAttack:
                            DrawBattleTileAtk(origin.TilesAtk.BulletTrajectories[i], new Vector2Int(0, 6), new Vector2Int(0, 12));
                            break;
                        case BattleFieldAttackType.OnTarget:
                            DrawBattleTileAtk(origin.TilesAtk.BulletTrajectories[i], new Vector2Int(-3, 4), new Vector2Int(-3, 4));
                            break;
                        case BattleFieldAttackType.OnItSelf:
                            DrawBattleTileAtk(origin.TilesAtk.BulletTrajectories[i], new Vector2Int(-3, 4), new Vector2Int(-3, 4));
                            break;
                        default:
                            break;
                    }
                }
                firstOpen = false;
            }
        }
      
        EditorUtility.SetDirty(origin);
    }


    public List<T> RefreshList<T>(int nextVal, List<T> currentList) where T : new()
    {
        while (nextVal < currentList.Count && currentList.Count > 0)
            currentList.RemoveAt(currentList.Count - 1);
        while (nextVal > currentList.Count)
            currentList.Add(new T());

        return currentList;
    }



    public void DrawParticlesAtk(BulletBehaviourInfoClass particlesTrajectory)
    {
        EditorGUILayout.Space();
        particlesTrajectory.Show = EditorGUILayout.Foldout(particlesTrajectory.Show, "trajectory");
        if (particlesTrajectory.Show)
        {
            particlesTrajectory.BulletDistanceInTile = EditorGUILayout.Vector2IntField("BulletDistanceInTile", particlesTrajectory.BulletDistanceInTile);
            particlesTrajectory.Trajectory_Y = EditorGUILayout.CurveField("Trajectory_Y", particlesTrajectory.Trajectory_Y);
            particlesTrajectory.Trajectory_Z = EditorGUILayout.CurveField("Trajectory_Z", particlesTrajectory.Trajectory_Z);
            particlesTrajectory.BulletGapStartingTile = EditorGUILayout.Vector2IntField("BulletGapStartingTile", particlesTrajectory.BulletGapStartingTile);

            var list = particlesTrajectory.BulletEffectTiles;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("TileAffectedByExplosion", particlesTrajectory.BulletEffectTiles.Count));

            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(new Vector2Int());

            for (int i = 0; i < list.Count; i++)
            {
                particlesTrajectory.BulletEffectTiles[i] = EditorGUILayout.Vector2IntField("BulletEffectTile " + i,
                    particlesTrajectory.BulletEffectTiles[i]);
            }

            particlesTrajectory.HasEffect = EditorGUILayout.ToggleLeft("HasEffect", particlesTrajectory.HasEffect);
            if (particlesTrajectory.HasEffect)
            {
                particlesTrajectory.EffectChances = EditorGUILayout.FloatField("EffectChances", particlesTrajectory.EffectChances);
                var listEffect = particlesTrajectory.Effects;
                int newCountEffect = Mathf.Max(0, EditorGUILayout.IntField("Effects", particlesTrajectory.Effects.Count));
                while (newCountEffect < listEffect.Count)
                    listEffect.RemoveAt(listEffect.Count - 1);
                while (newCountEffect > listEffect.Count)
                    listEffect.Add(null);

                for (int a = 0; a < listEffect.Count; a++)
                {
                    particlesTrajectory.Effects[a] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + a,
                        particlesTrajectory.Effects[a], typeof(ScriptableObjectAttackEffect), false);
                }
            }
        }
    }

    public void DrawBattleTileAtk(BulletBehaviourInfoClassOnBattleFieldClass BattleTileTrajectory, Vector2Int horizontal, Vector2Int vertical)
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

                bfatc = BattleTileTrajectory.BulletEffectTiles.Where(r => r.Pos == new Vector2Int(x, y)).FirstOrDefault();
                if (firstOpen && bfatc != null)
                {
                    bfti = new BattleFieldTileInfo(BattleTileTrajectory, bfatc);
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
                                TilesInfo[TilesInfo.Count - 1].Tile.IsEffectOnTile, TilesInfo[TilesInfo.Count - 1].Tile.TileParticlesID, TilesInfo[TilesInfo.Count - 1].Tile.DurationOnTile,
                            TilesInfo[TilesInfo.Count - 1].Tile.EffectsOnTile);
                        }
                        bfti = new BattleFieldTileInfo(BattleTileTrajectory, bfatc);
                        BattleTileTrajectory.BulletEffectTiles.Add(bfatc);
                        TilesInfo.Add(bfti);
                    }
                }
                else if (!showClose && bfatc != null)
                {
                    TilesInfo.Remove(bfti);
                    BattleTileTrajectory.BulletEffectTiles.Remove(bfatc);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (TilesInfo.Count > 0)
        {
            WriteInfo(BattleTileTrajectory);
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

        bfatc.IsEffectOnTile = EditorGUILayout.ToggleLeft("HasEffectOnTile", bfatc.IsEffectOnTile);
        if (bfatc.IsEffectOnTile)
        {
            bfatc.TileParticlesID = (ParticlesType)EditorGUILayout.EnumPopup("ParticleType", bfatc.TileParticlesID);
            bfatc.DurationOnTile = EditorGUILayout.FloatField("DurationOnTile", bfatc.DurationOnTile);
            var list = bfatc.EffectsOnTile;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Effects", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            for (int i = 0; i < list.Count; i++)
            {
                bfatc.EffectsOnTile[i] = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Effect " + i, bfatc.EffectsOnTile[i], typeof(ScriptableObjectAttackEffect), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
            }
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
