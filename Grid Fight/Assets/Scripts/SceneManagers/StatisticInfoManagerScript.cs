using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatisticInfoManagerScript : MonoBehaviour
{
    public static StatisticInfoManagerScript Instance;
    public List<StatisticInfoClass> CharaterStats = new List<StatisticInfoClass>();

    private void Awake()
    {
        Instance = this;
    }

    public StatisticInfoClass CharacterStatsFor(CharacterNameType ID)
    {
        return CharaterStats.Where(r => r.CharacterId == ID).FirstOrDefault();
    }
}

[System.Serializable]
public class StatisticInfoClass
{
    public List<ControllerType> PlayerController;
    public CharacterNameType CharacterId;
    public float DamageMade;
    public float DamageReceived;
    public float TimeOnField;
    public int BulletFired;
    public int BulletHits;
    public float Accuracy
    {
        get
        {
            return BulletHits / BulletFired;
        }
    }
    public int HitReceived;
    public int Defences;
    public int CompleteDefences;
    public float Reflexes
    {
        get
        {
            return ((Defences / HitReceived) + (CompleteDefences / Defences)) / 2f;
        }
    }
    public float HPGotBySkill;
    public float HPHealed;
    public int PotionPicked;
    public float Exp
    {
        get
        {
            return AccuracyExp + DamageExp + ReflexExp;
        }
    }
    public float AccuracyExp;
    public float DamageExp;
    public float ReflexExp;
    public float StartExp;

    public StatisticInfoClass()
    {

    }

    public StatisticInfoClass(CharacterNameType characterId, List<ControllerType> playerController, float startingExperience = 0f)
    {
        CharacterId = characterId;
        PlayerController = playerController;
        StartExp = startingExperience;
    }
}

[System.Serializable]
public class GlobalCharacterStatisticInfoClass
{
    public float Experience;
    public float TotalDamageMade;
    public float TotalDamageReceived;
    public float TotalTimeOnField;
    public int TotalBulletFired;
    public int TotalBulletHits;
    public int TotalHitReceived;
    public int TotalDefences;
    public int TotalCompleteDefences;
    public float TotalHPGotBySkill;
    public float TotalHPHealed;
    public int TotalPotionPicked;
    public float TotalExp;

    public GlobalCharacterStatisticInfoClass()
    {

    }

}

