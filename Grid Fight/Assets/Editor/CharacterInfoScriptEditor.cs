using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterInfoScript))]
public class CharacterInfoScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterInfoScript origin = (CharacterInfoScript)target;

        base.OnInspectorGUI();

        if(origin.RelationshipList.Count> 0)
        {
            foreach (RelationshipClass item in origin.RelationshipList)
            {
                if (item.name != item.CharacterId.ToString())
                {
                    item.name = item.CharacterId.ToString();
                }
                if(item.CharOwnerId != origin.CharacterID)
                {
                    item.CharOwnerId = origin.CharacterID;
                }
            }
        }
        EditorUtility.SetDirty(origin);
    }
}
