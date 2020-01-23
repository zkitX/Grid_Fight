using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Particles/AttackParticle")]
public class ScriptableObjectAttackParticle : ScriptableObject
{
    public AttackParticleTypes PSType;
    public GameObject CastPS;
    public GameObject AttackPS;
    public GameObject EffectPS;
    public GameObject CastLoopPS;
    public GameObject CastActivationPS;
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