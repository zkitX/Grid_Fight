using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterType_Script))]
public class CharacterType_ScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterType_Script origin = (CharacterType_Script)target;

        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DEBUG TOOLS");
        if(GUILayout.Button("Jump Character"))
        {
            EnvironmentManager.Instance.MoveToNewGrid(EnvironmentManager.Instance.currentGridIndex == 0 ? 1 : 0, 1.5f);
        }
    }
}
