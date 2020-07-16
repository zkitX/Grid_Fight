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
    public List<AICheckClass> Checks = new List<AICheckClass>();
    public float CoolDown = 0;
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

    public BaseCharacter t;
    public List<AIStatsModifierClass> StatsToModify = new List<AIStatsModifierClass>();
    System.Reflection.FieldInfo parentField = null, field = null, B_field = null;
    string[] statToCheck;

    public ScriptableObjectParticle AIPs;

    public int CheckAvailability(CharacterInfoScript charInfo, List<AggroInfoClass> enemies, Vector2Int currentPos)
    {
        AggroInfoClass target = new AggroInfoClass(ControllerType.Player1, 0);
        int Score = 0;
        int charTargeting = 0;
        foreach (AggroInfoClass item in enemies)
        {
            charTargeting += item.Hit;
        }


        foreach (AggroInfoClass item in enemies)
        {
            int res = Random.Range(0, charTargeting);

            if (res <= item.Hit)
            {
                target = item;
                break;
            }
            else
            {
                charTargeting -= item.Hit;
            }
        }

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


        t = BattleManagerScript.Instance.CurrentSelectedCharacters[target.PlayerController].Character;

        switch (Vision)
        {
            case VisionType.Front_Near:
                if (t.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) < 3)
                {
                    Score += 100;
                }
                else
                {
                    Score -= 20;
                }
                break;
            case VisionType.Front_Far:
                if (t.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) > 3)
                {
                    Score += 100;
                }
                else
                {
                    Score -= 20;
                }
                break;
            case VisionType.UpDown_Near:
                if (t.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) < 3)
                {
                    Score += 100;
                }
                else
                {
                    Score -= 20;
                }
                break;
            case VisionType.UpDown_Far:
                if (t.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) > 3)
                {
                    Score += 100;
                }
                else
                {
                    Score -= 20;
                }
                break;
        }


        int i = 0;
        foreach (AICheckClass item in Checks)
        {
            switch (item.StatToCheck)
            {
                case StatsCheckType.None:
                    break;
                case StatsCheckType.Health:
                    if (CheckStatsValues(item.ValueChecker, charInfo.HealthPerc, item.PercToCheck))
                    {
                        Score += 100;
                        i++;
                    }
                    break;
                case StatsCheckType.Stamina:
                    if (CheckStatsValues(item.ValueChecker, charInfo.StaminaPerc, item.PercToCheck))
                    {
                        Score += 100;
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
                    if (CheckStatsValues(item.ValueChecker, WaveManagerScript.Instance.GetCurrentPartyHPPerc(), item.PercToCheck))
                    {
                        Score += 100;
                        i++;
                    }
                    break;
            }
        }
        if(i == Checks.Count && Checks.Count != 0)
        {
            Score += 300;
        }

       /* float partyHp = WaveManagerScript.Instance.GetCurrentPartyHPPerc();
        switch (PartyHp)
        {
            case PartyHPType.Over_5:
                if (partyHp >= 5)
                {
                    Score += 20;
                }
                else
                {
                    Score -= 20;
                }
                break;
            case PartyHPType.Over_30:
                if (partyHp >= 30)
                {
                    Score += 20;
                }
                else
                {
                    Score -= 20;
                }
                break;
            case PartyHPType.Over_50:
                if (partyHp >= 50)
                {
                    Score += 20;
                }
                else
                {
                    Score -= 20;
                }
                break;
            case PartyHPType.Over_90:
                if (partyHp >= 90)
                {
                    Score += 20;
                }
                else
                {
                    Score -= 20;
                }
                break;
        }*/
       if (t != null)
       {
           if (t.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) < 3)
           {
               Score += 10;
           }
           else if (t.UMS.CurrentTilePos.x == currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) > 3)
           {
               Score += 5;
           }        else if (t.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) < 3)
           {
               Score += 0;
           }
           else if (t.UMS.CurrentTilePos.x != currentPos.x && Mathf.Abs(t.UMS.CurrentTilePos.y - currentPos.y) > 3)
           {
               Score += 0;
           }

       /*    if (partyHp >= 90)
           {
               Score += 0;
           }
           else if (partyHp >= 60)
           {
               Score += 5;
           }
           else if (partyHp >= 30)
           {
               Score += 10;
           }
           else if (partyHp >= 5)
           {
               Score += 20;
           }*/
       }
        return Score;
    }



    private bool CheckStatsValues(ValueCheckerType valueChecker, float current, float perc)
    {
        switch (valueChecker)
        {
            case ValueCheckerType.LessThan:
                if(current < perc)
                {
                    return true;
                }
                break;
            case ValueCheckerType.EqualTo:
                if (current == perc)
                {
                    return true;
                }
                break;
            case ValueCheckerType.MoreThan:
                if (current > perc)
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
    public float PercToCheck;
}


[System.Serializable]
public class AIStatsModifierClass
{
    public ModificableStatsType ModificableStats;
    public float Multiplier;
}



