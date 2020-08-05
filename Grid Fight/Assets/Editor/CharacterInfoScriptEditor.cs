using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

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

        if(origin.transform.GetComponentsInChildren<Transform>().Where(r=> r.name == "Head").ToList().Count == 0)
        {
            GameObject head = Instantiate(new GameObject(), origin.transform);
            head.name = "Head";
            origin.Head = head.transform;
        }

        origin.testAtkEffect = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Debug BuffDebuff ", origin.testAtkEffect, typeof(ScriptableObjectAttackEffect), false);
        if (GUILayout.Button("Apply Debug BuffDebuff"))
        {
            origin.charOwner.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(),
                ElementalType.Neutral, origin.charOwner, origin.testAtkEffect));
        }
        EditorUtility.SetDirty(origin);
    }
}
