using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///   public bool IsRandomPos = true;
//[ConditionalField("IsRandomPos", true)] public Vector2Int SpawningPos;
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkillMask")]
public class ScriptableObjectSkillMask : ScriptableObject
{
    public Sprite MaskIcon;
    public ScriptableObjectAttackBase Skill1;
    public ScriptableObjectAttackBase Skill2;
    public ScriptableObjectAttackBase Skill3;
}
