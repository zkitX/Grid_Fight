using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Effect/AttackEffect")]
public class ScriptableObjectAttackEffect : ScriptableObject
{

    public float Duration
    {
        get
        {
            return UnityEngine.Random.Range(_Duration.x, _Duration.y);
        }
    }

    public string Name;
    public BuffDebuffStatsType StatsToAffect;
    public StatsCheckerType StatsChecker;
    public Vector2 Value;
    public Vector2 _Duration;
    public CharacterAnimationStateType AnimToFire;
    public ParticlesType Particles;
    public StatusEffectType classification = StatusEffectType.Buff;
    public Sprite icon = null;
    public bool recolorCharUI = false;
    [ConditionalField("recolorCharUI")] public Color statusIconColor = Color.magenta;
    [HideInInspector] public ScriptableObjectAttackBase Atk;
    [HideInInspector] public List<ScriptableObjectAI> AIs = new List<ScriptableObjectAI>();
    [HideInInspector] public GameObject ClonePrefab = null;  //If left to null, the basic character will be used in a nerfed state
    [HideInInspector] public float ClonePowerScale = 0.5f; //How much of a nerf the clone receives upon creation
}