using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ScriptableObjectAI))]
public class ScriptableObjectAIEditor : Editor
{
    ScriptableObjectAI origin;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        origin = (ScriptableObjectAI)target;

        List<AICheckClass> list = new List<AICheckClass>();
        list.AddRange(origin.Checks);
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Number of Checks", list.Count));
        while (newCount < list.Count)
            list.RemoveAt(list.Count - 1);
        while (newCount > list.Count)
            list.Add(new AICheckClass());
        origin.Checks.Clear();

        for (int i = 0; i < list.Count; i++)
        {
            list[i].Show = EditorGUILayout.Foldout(list[i].Show, "Checks  " + i);
            if (list[i].Show)
            {
                

                list[i].CheckOnTarget = EditorGUILayout.Toggle("CheckOnTarget", list[i].CheckOnTarget);
                list[i].CheckWeightMultiplier = EditorGUILayout.IntField("CheckWeightMultiplier", list[i].CheckWeightMultiplier);
                list[i].StatToCheck = (StatsCheckType)EditorGUILayout.EnumPopup("StatToCheck", list[i].StatToCheck);

                if (list[i].StatToCheck == StatsCheckType.BuffDebuff)
                {
                    list[i].BuffDebuff = (BuffDebuffStatsType)EditorGUILayout.EnumPopup("BuffDebuff", list[i].BuffDebuff);
                }
                else
                {
                    list[i].ValueChecker = (ValueCheckerType)EditorGUILayout.EnumPopup("ValueChecker", list[i].ValueChecker);
                    if (list[i].ValueChecker < ValueCheckerType.Between)
                    {
                        list[i].PercToCheck = EditorGUILayout.FloatField("PercToCheck", list[i].PercToCheck);
                    }
                    else
                    {
                        list[i].InBetween = EditorGUILayout.Vector2Field("InBetween", list[i].InBetween);
                    }
                }
               
            }
            origin.Checks.Add(list[i]);
        }

        EditorUtility.SetDirty(origin);
    }
}
