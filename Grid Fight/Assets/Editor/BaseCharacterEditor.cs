using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(BaseCharacter))]
public class BaseCharacterEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BaseCharacter origin = (BaseCharacter)target;
        origin.testAtkEffect = (ScriptableObjectAttackEffect)EditorGUILayout.ObjectField("Debug BuffDebuff ", origin.testAtkEffect, typeof(ScriptableObjectAttackEffect), false);
        if (GUILayout.Button("Apply Debug BuffDebuff"))
        {
            origin.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(),
                ElementalType.Neutral, origin, origin.testAtkEffect));
        }
    }
   
}


[CustomEditor(typeof(CharacterType_Script))]
public class CharacterType_ScriptEditor : BaseCharacterEditor
{

}


[CustomEditor(typeof(MinionType_Script))]
public class MinionType_ScriptEditor : BaseCharacterEditor
{

}
