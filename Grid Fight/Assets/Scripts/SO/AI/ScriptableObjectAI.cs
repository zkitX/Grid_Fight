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
    System.Reflection.FieldInfo parentField = null, field = null, B_field = null;
    string[] statToCheck;
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

        switch (Vision)
        {
            case VisionType.Front_Near:
                break;
            case VisionType.Front_Far:
                break;
            case VisionType.UpDown_Near:
                break;
            case VisionType.UpDown_Far:
                break;
            default:
                break;
        }


        return true;
    }


    public void ModifyStats(CharacterInfoScript charinfo)
    {
        foreach (AIStatsModifierClass item in StatsToModify)
        {

            statToCheck = item.ModificableStats.ToString().Split('_');
            parentField = charinfo.GetType().GetField(statToCheck[0]);
            field = parentField.GetValue(charinfo).GetType().GetField(statToCheck[1]);
            B_field = parentField.GetValue(charinfo).GetType().GetField("B_" + statToCheck[1]);
            if(B_field.FieldType == typeof(Vector2))
            {
                field.SetValue(parentField.GetValue(charinfo), item.Multiplier * (Vector2)B_field.GetValue(charinfo));
            }
            else
            {
                field.SetValue(parentField.GetValue(charinfo), item.Multiplier * (float)B_field.GetValue(charinfo));
            }

        }
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

