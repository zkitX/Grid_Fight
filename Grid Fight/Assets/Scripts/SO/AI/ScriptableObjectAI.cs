using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
   // public AggroType Aggro;
    //public PartyHPType PartyHp;
    [HideInInspector]public List<AICheckClass> Checks = new List<AICheckClass>();
    public Vector2 _CoolDown = new Vector2(3, 5);
    [HideInInspector]
    public float CoolDown
    {
        get
        {
            return Random.Range(_CoolDown.x, _CoolDown.y);
        }
    }
    [Header("Move effects")]

    public bool IdleMovement = false;
    public bool UpdateAttckWill = false;
    [ConditionalField("UpdateAttckWill", false)] public int AttackWill = 20;
    public bool UpdateMoveForward = false;
    [ConditionalField("UpdateMoveForward", false)] public int MoveForward = 20;
    public bool UpdateMoveBackward = false;
    [ConditionalField("UpdateMoveBackward", false)] public int MoveBackward = 20;
    public bool UpdateMoveUpDown = false;
    [ConditionalField("UpdateMoveUpDown", false)] public int MoveUpDown = 20;

    [Header("State effects")]

    public List<AIStatsModifierClass> StatsToModify = new List<AIStatsModifierClass>();
    System.Reflection.FieldInfo parentField = null, field = null, B_field = null;
    string[] statToCheck;

    public ScriptableObjectParticle AIPs;

    [HideInInspector] public bool Show;

    public BaseCharacter GetAggro(BaseCharType baseCharacterType, Vector2Int currentPos, List<AggroInfoClass> enemies, out int Score)
    {
        Score = 0;

        if (baseCharacterType == BaseCharType.PlayerMinionType_Script || baseCharacterType == BaseCharType.CharacterType_Script)
        {
            if(WaveManagerScript.Instance.WaveCharcters.Count == 0)
            {
                return null;
            }
            return WaveManagerScript.Instance.WaveCharcters.Where(r => r.isActiveAndEnabled && r.IsOnField).ToArray().OrderBy(a => Mathf.Abs(a.UMS.CurrentTilePos.x - currentPos.x)).ToArray()[0];
        }


        AggroInfoClass target = new AggroInfoClass(ControllerType.Player1, 0);

        int charTargeting = 0;
        int split = 100 / BattleManagerScript.Instance.CurrentSelectedCharacters.Count;

        if (target.Hit <= 0)
        {
            Score -= 5;
        }
        else if (target.Hit >= 3 && target.Hit < 5)
        {
            Score += 5;
        }
        else if (target.Hit >= 5)
        {
            Score += 10;
        }

        foreach (AggroInfoClass item in enemies)
        {
            charTargeting += item.Hit + split;
        }


        foreach (AggroInfoClass item in enemies)
        {
            int res = Random.Range(0, charTargeting);

            if (res <= item.Hit + split)
            {
                target = item;
                break;
            }
            else
            {
                charTargeting -= item.Hit;
            }
        }


        if (BattleManagerScript.Instance.CurrentSelectedCharacters[target.PlayerController].Character != null)
        {
            return BattleManagerScript.Instance.CurrentSelectedCharacters[target.PlayerController].Character;
        }

        return null;
    }

    public int CheckAvailability(BaseCharacter bChar, List<AggroInfoClass> enemies, Vector2Int currentPos, ref BaseCharacter target)
    {
        target = GetAggro(bChar.CharInfo.BaseCharacterType, currentPos, enemies, out int Score);

        int i = 0;
        foreach (AICheckClass item in Checks)
        {
            switch (item.StatToCheck)
            {
                case StatsCheckType.None:
                    break;
                case StatsCheckType.Health:
                    if (CheckStatsValues(item, bChar.CharInfo.HealthPerc))
                    {
                        Score += 100 * item.CheckWeight;
                        i++;
                    }
                    break;
                case StatsCheckType.Stamina:
                    if (CheckStatsValues(item, bChar.CharInfo.StaminaPerc))
                    {
                        Score += 100 * item.CheckWeight;
                        i++;
                    }
                    break;
                case StatsCheckType.AttackSpeed:
                    break;
                case StatsCheckType.MovementSpeed:
                    break;
                case StatsCheckType.BaseSpeed:
                    break;
                case StatsCheckType.TeamTotalHpPerc:
                    if (CheckStatsValues(item, WaveManagerScript.Instance.GetCurrentPartyHPPerc()))
                    {
                        Score += 100 * item.CheckWeight;
                        i++;
                    }
                    break;
                case StatsCheckType.BuffDebuff:
                    if(bChar.HasBuffDebuff(item.BuffDebuff))
                    {
                        Score += 100 * item.CheckWeight;
                        i++;
                    }
                    break;
            }
        }
        if(i == Checks.Count && Checks.Count != 0)
        {
            Score += 300;
        }
     
       if (target != null)
       {
            switch (Vision)
            {
                case VisionType.Front_Near:
                    if (target.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) < 3)
                    {
                        Score += 100;
                    }
                    else
                    {
                        Score -= 20;
                    }
                    break;
                case VisionType.Front_Far:
                    if (target.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) > 3)
                    {
                        Score += 100;
                    }
                    else
                    {
                        Score -= 20;
                    }
                    break;
                case VisionType.UpDown_Near:
                    if (target.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) < 3)
                    {
                        Score += 100;
                    }
                    else
                    {
                        Score -= 20;
                    }
                    break;
                case VisionType.UpDown_Far:
                    if (target.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) > 3)
                    {
                        Score += 100;
                    }
                    else
                    {
                        Score -= 20;
                    }
                    break;
            }
           if (target.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) < 3)
           {
               Score += 10;
           }
           else if (target.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) > 3)
           {
               Score += 5;
           }
           else if (target.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) < 3)
           {
               Score += 0;
           }
           else if (target.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(target.UMS.CurrentTilePos.y - currentPos.y) > 3)
           {
               Score += 0;
           }
       }
        return Score;
    }



    private bool CheckStatsValues(AICheckClass aicc, float current)
    {
        switch (aicc.ValueChecker)
        {
            case ValueCheckerType.LessThan:
                if(current < aicc.PercToCheck)
                {
                    return true;
                }
                break;
            case ValueCheckerType.EqualTo:
                if (current == aicc.PercToCheck)
                {
                    return true;
                }
                break;
            case ValueCheckerType.MoreThan:
                if (current > aicc.PercToCheck)
                {
                    return true;
                }
                break;
            case ValueCheckerType.Between:
                if (current <= aicc.InBetween.x && current >= aicc.InBetween.y)
                {
                    return true;
                }
                break;
        }

        return false;
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
                field.SetValue(parentField.GetValue(charinfo), (Vector2)field.GetValue(parentField.GetValue(charinfo)) + (item.Multiplier * (Vector2)B_field.GetValue(parentField.GetValue(charinfo))));
            }
            else
            {
                field.SetValue(parentField.GetValue(charinfo), (float)field.GetValue(parentField.GetValue(charinfo)) + (item.Multiplier * (float)B_field.GetValue(parentField.GetValue(charinfo))));
            }

        }
    }

    public void ResetStats(CharacterInfoScript charinfo)
    {
        foreach (AIStatsModifierClass item in StatsToModify)
        {
            statToCheck = item.ModificableStats.ToString().Split('_');
            parentField = charinfo.GetType().GetField(statToCheck[0]);
            field = parentField.GetValue(charinfo).GetType().GetField(statToCheck[1]);
            B_field = parentField.GetValue(charinfo).GetType().GetField("B_" + statToCheck[1]);
            if (B_field.FieldType == typeof(Vector2))
            {
                field.SetValue(parentField.GetValue(charinfo), (Vector2)field.GetValue(parentField.GetValue(charinfo)) - (item.Multiplier * (Vector2)B_field.GetValue(parentField.GetValue(charinfo))));
            }
            else
            {
                field.SetValue(parentField.GetValue(charinfo), (float)field.GetValue(parentField.GetValue(charinfo)) - (item.Multiplier * (float)B_field.GetValue(parentField.GetValue(charinfo))));
            }

        }
    }
}

[System.Serializable]
public class AICheckClass
{
    public StatsCheckType StatToCheck;
    public ValueCheckerType ValueChecker;
    public int CheckWeight = 1;

    [HideInInspector]public float PercToCheck;
    [HideInInspector]public Vector2 InBetween = new Vector2(60, 40);
    [HideInInspector]public bool Show;
    [HideInInspector]public BuffDebuffStatsType BuffDebuff;
}


[System.Serializable]
public class AIStatsModifierClass
{
    public ModificableStatsType ModificableStats;
    public float Multiplier;
}



