using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
///   public bool IsRandomPos = true;
//[ConditionalField("IsRandomPos", true)] public Vector2Int SpawningPos;
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AI")]
public class ScriptableObjectAI : ScriptableObject
{
    public AIType AI_Type;

    public VisionType Vision;
    public AggroType Aggro;

    public List<AICheckClass> AIChecks = new List<AICheckClass>();

    [Header("State effects")]
    public bool UpdateMoveForward = false;
    [ConditionalField("UpdateMoveForward", false)] public float MoveForward = 20;
    public bool UpdateMoveBackward = false;
    [ConditionalField("UpdateMoveBackward", false)] public float MoveBackward = 20;
    public bool UpdateMoveUpDown = false;
    [ConditionalField("UpdateMoveUpDown", false)] public float MoveUpDown = 20;


    public List<AIStatsModifierClass> StatsToModify = new List<AIStatsModifierClass>();

    public bool CheckAvailability(CharacterInfoScript charInfo)
    {
        foreach (AICheckClass item in AIChecks)
        {
            switch (item.StatToCheck)
            {
                case StatsCheckType.Health:
                switch (item.ValueChecker)
                {
                    case ValueCheckerType.LessThan:
                        if (charInfo.HealthPerc > item.PercToCheck)
                        {
                            return false;
                        }
                        break;
                    case ValueCheckerType.EqualTo:
                        if (charInfo.HealthPerc != item.PercToCheck)
                        {
                            return false;
                        }
                        break;
                    case ValueCheckerType.MoreThan:
                        if (charInfo.HealthPerc < item.PercToCheck)
                        {
                            return false;
                        }
                        break;
                }
                break;
            case StatsCheckType.Stamina:
                switch (item.ValueChecker)
                {
                    case ValueCheckerType.LessThan:
                        if (charInfo.StaminaPerc > item.PercToCheck)
                        {
                                return false;
                        }
                        break;
                    case ValueCheckerType.EqualTo:
                        if (charInfo.StaminaPerc != item.PercToCheck)
                        {
                                return false;
                        }
                        break;
                    case ValueCheckerType.MoreThan:
                        if (charInfo.StaminaPerc < item.PercToCheck)
                        {
                                return false;
                        }
                        break;
                }
                break;
                case StatsCheckType.TeamTotalHpPerc:
                    float partyHPPerch = WaveManagerScript.Instance.GetCurrentPartyHPPerc();
                    switch (item.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            if (partyHPPerch > item.PercToCheck)
                            {
                                return false;
                            }
                            break;
                        case ValueCheckerType.EqualTo:
                            if (partyHPPerch != item.PercToCheck)
                            {
                                return false;
                            }
                            break;
                        case ValueCheckerType.MoreThan:
                            if (partyHPPerch < item.PercToCheck)
                            {
                                return false;
                            }
                            break;
                    }
                    break;
            }
        }

        return true;
    }
}

[System.Serializable]
public class AICheckClass
{
    public StatsCheckType StatToCheck;
    public ValueCheckerType ValueChecker;
    public float PercToCheck;
}


[System.Serializable]
public class AIStatsModifierClass
{
    public ModificableStatsType ModificableStats;
    public float Multiplier;
}

