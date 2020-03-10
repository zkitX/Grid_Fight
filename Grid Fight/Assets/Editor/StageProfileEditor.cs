using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StageProfile))]
public class StageProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        StageProfile origin = (StageProfile)target;

        EditorGUILayout.Space();
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("CHARACTERS:");
        foreach (CharacterBaseInfoClass charInfoClass in origin.BattleInfoManager.GetComponent<BattleInfoManagerScript>().PlayerBattleInfo)
        {
            EditorGUILayout.LabelField(charInfoClass.Name.ToString() + " || " + charInfoClass.CharacterName.ToString());
        }

        EditorGUILayout.Space();
        string theTypeOfStage = "";
        switch (origin.BattleInfoManager.GetComponent<BattleInfoManagerScript>().MatchInfoType)
        {
            case (MatchType.PPvE):
                theTypeOfStage = "Co-op";
                break;
            case (MatchType.PPvPP):
                theTypeOfStage = "Team Deathmatch";
                break;
            case (MatchType.PvE):
                theTypeOfStage = "Player vs Enemy";
                break;
            case (MatchType.PvP):
                theTypeOfStage = "Player Vs Player";
                break;
            default:
                theTypeOfStage = origin.BattleInfoManager.GetComponent<BattleInfoManagerScript>().MatchInfoType.ToString();
                break;
        }
        EditorGUILayout.LabelField("BATTLE TYPE: " + theTypeOfStage);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        WaveManagerScript waveM ;
        if (origin.Wave != null)
        {
            waveM = origin.Wave.GetComponent<WaveManagerScript>();
            EditorGUILayout.LabelField("WAVES:");
            EditorGUILayout.LabelField("Wave Count - " + waveM.WavePhases.Count.ToString());

            foreach (WavePhaseClass wave in waveM.WavePhases)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginFoldoutHeaderGroup(true, wave.name.ToUpper());
                foreach (WaveCharClass waveC in wave.ListOfEnemy)
                {
                    EditorGUILayout.LabelField("     " + waveC.NumberOfCharacter + " " + waveC.name + (waveC.NumberOfCharacter > 1 && waveC.name[waveC.name.Length - 1].ToString() != "s" ? "s" : ""));
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
