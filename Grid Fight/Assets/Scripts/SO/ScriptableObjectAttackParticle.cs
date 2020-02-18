using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Particles/AttackParticle")]
public class ScriptableObjectAttackParticle : ScriptableObject
{
    public AttackParticleTypes PSType;
    public GameObject CastRightPS;
    public GameObject ImpactRightPS;
    public GameObject CastLeftPS;
    public GameObject BulletLeftPS;
    public GameObject ImpactLeftPS;
    public GameObject CastLoopPS;
    public GameObject CastActivationPS;
    //public GameObject 
}



[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkillParticles")]
public class ScriptableObjectSkillParticles : ScriptableObject
{
    public CharacterClassType CharacterClass;
    public List<ScriptableObjectSkillParticlesClass> CastPS = new List<ScriptableObjectSkillParticlesClass>();
    public List<ScriptableObjectSkillParticlesClass> AttackPS = new List<ScriptableObjectSkillParticlesClass>();
    public List<ScriptableObjectSkillParticlesClass> EffectPS = new List<ScriptableObjectSkillParticlesClass>();
}


[System.Serializable]
public class ScriptableObjectSkillParticlesClass
{
    public string Name;
    public ElementalType ElementalT;
    public GameObject Particle;
}