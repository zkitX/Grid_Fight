using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

public class StatisticInfoManagerScript : MonoBehaviour
{
    public static StatisticInfoManagerScript Instance;
    public List<StatisticInfoClass> CharaterStats = new List<StatisticInfoClass>();


    private void Awake()
    {
        Instance = this;
    }

    //Return a combination of the xp stats class on the gameobject and the actual stats on the character
    public StatisticInfoClass GetCharacterStatsFor(CharacterNameType ID)
    {
        StatisticInfoClass returnable = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == ID).FirstOrDefault().Sic;
        StatisticInfoClass additive = CharaterStats.Where(r => r.CharacterId == ID).FirstOrDefault();
        returnable.BaseExp += additive.BaseExp;
        returnable.AccuracyExp += additive.AccuracyExp;
        returnable.DamageExp += additive.DamageExp;
        returnable.ReflexExp += additive.ReflexExp;
        return returnable;
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
            return (float)BulletHits / ((float)BulletFired != 0f ? (float)BulletFired : 1f);
        }
    }
    public int HitReceived;
    public int Defences;
    public int CompleteDefences;
    public float Reflexes
    {
        get
        {
            return (((float)Defences / ((float)HitReceived != 0f ? (float)HitReceived : 1f)) + ((float)CompleteDefences / ((float)Defences != 0f ? (float)Defences : 1f))) / 2f;
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
    protected float accuracyExp;
    public float AccuracyExp
    {
        get
        {
            return BattleManagerBaseObjectGeneratorScript.Instance.stage.bestAccuracyRating.UseMaximumRewardSystem ?
                Mathf.Clamp(accuracyExp, 0f, BattleManagerBaseObjectGeneratorScript.Instance.stage.bestAccuracyRating.MaximumReward) : accuracyExp;
        }
        set
        {
            accuracyExp = value;
        }
    }
    protected float damageExp;
    public float DamageExp
    {
        get
        {
            return BattleManagerBaseObjectGeneratorScript.Instance.stage.bestDamageRating.UseMaximumRewardSystem ?
                Mathf.Clamp(damageExp, 0f, BattleManagerBaseObjectGeneratorScript.Instance.stage.bestDamageRating.MaximumReward) : damageExp;
        }
        set
        {
            damageExp = value;
        }
    }
    protected float reflexExp;
    public float ReflexExp
    {
        get
        {
            return BattleManagerBaseObjectGeneratorScript.Instance.stage.bestReflexRating.UseMaximumRewardSystem ?
                Mathf.Clamp(reflexExp, 0f, BattleManagerBaseObjectGeneratorScript.Instance.stage.bestReflexRating.MaximumReward) : reflexExp;
        }
        set
        {
            reflexExp = value;
        }
    }
    public float baseExp;
    public float BaseExp
    {
        get
        {
            return baseExp;
        }
        set
        {
            baseExp = value;
        }
    }

    public StatisticInfoClass()
    {

    }

    public StatisticInfoClass(CharacterNameType characterId, List<ControllerType> playerController, float startingExperience = 0f)
    {
        CharacterId = characterId;
        PlayerController = playerController;
        BaseExp = startingExperience;
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

