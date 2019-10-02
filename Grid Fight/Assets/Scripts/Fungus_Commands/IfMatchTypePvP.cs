using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;


[CommandInfo("Flow",
                "IfPvP",
                "If the test expression is true, execute the following command block.")]
[AddComponentMenu("")]
public class IfMatchTypePvP : VariableCondition
{
    protected override bool HasNeededProperties()
    {
        return true;
    }
    protected override bool EvaluateCondition()
    {
        if (BattleInfoManagerScript.Instance.MatchInfoType == MatchType.PvP)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override Color GetButtonColor()
    {
        return new Color32(253, 253, 150, 255);
    }

}
