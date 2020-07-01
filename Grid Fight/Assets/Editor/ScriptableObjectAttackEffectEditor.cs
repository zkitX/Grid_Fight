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
        EditorUtility.SetDirty(origin);
    }

}
