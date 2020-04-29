using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticInfoManagerScript : MonoBehaviour
{
    public static StatisticInfoManagerScript Instance;
    public List<StatisticInfoClass> CharaterStats = new List<StatisticInfoClass>();

    private void Awake()
    {
        Instance = this;
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
    public int HitReceived;
    public int Defences;
    public int CompleteDefences;
    public float HPGotBySkill;
    public float HPHealed;
    public int PotionPicked;

    public StatisticInfoClass()
    {

    }

    public StatisticInfoClass(CharacterNameType characterId, List<ControllerType> playerController)
    {
        CharacterId = characterId;
        PlayerController = playerController;
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

    public GlobalCharacterStatisticInfoClass()
    {

    }

}

