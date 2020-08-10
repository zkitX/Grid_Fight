using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptableObjectAttackEffect))]
public class ScriptableObjectAttackEffectEditor : Editor
{

    ScriptableObjectAttackEffect origin;
    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle();
        base.OnInspectorGUI();
        //test = false;
        origin = (ScriptableObjectAttackEffect)target;

        if (origin.StatsToAffect == BuffDebuffStatsType.AttackChange)
        {
            origin.Atk = (ScriptableObjectAttackBase)EditorGUILayout.ObjectField("Atk", origin.Atk, typeof(ScriptableObjectAttackBase), false);
        }

        if (origin.StatsToAffect == BuffDebuffStatsType.Rage)
        {
            origin.RageAI = (ScriptableObjectAI)EditorGUILayout.ObjectField("RageAI", origin.RageAI, typeof(ScriptableObjectAI), false);
        }
        if (origin.StatsToAffect == BuffDebuffStatsType.Zombification)
        {
            var list = origin.AIs;
            int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of AIs", list.Count));
            while (newCount < list.Count)
                list.RemoveAt(list.Count - 1);
            while (newCount > list.Count)
                list.Add(null);

            for (int i = 0; i < list.Count; i++)
            {
                origin.AIs[i] = (ScriptableObjectAI)EditorGUILayout.ObjectField("AI " + i, origin.AIs[i], typeof(ScriptableObjectAI), false);   //"Effect", bfatc.Effects, typeof(ScriptableObjectAttackEffect), false
            }
        }
        if(origin.StatsToAffect == BuffDebuffStatsType.Legion)
        {
            origin.ClonePrefab = (GameObject)EditorGUILayout.ObjectField("Clone Replacement Prefab ", origin.ClonePrefab, typeof(GameObject), false);
            origin.ClonePowerScale = EditorGUILayout.FloatField("Clone Power Multiplier", origin.ClonePowerScale);
            origin.CloneAsManyAsCurrentEnemies = EditorGUILayout.Toggle("Clone count matches enemies", origin.CloneAsManyAsCurrentEnemies);
            if (!origin.CloneAsManyAsCurrentEnemies) origin.CloneAmount = EditorGUILayout.IntField("Number of clones", origin.CloneAmount);
            origin.CloneStartingEffect = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Clone Starting Effect", origin.CloneStartingEffect, typeof(ScriptableObjectAttackEffect), false);
        }

        EditorUtility.SetDirty(origin);
    }

}
