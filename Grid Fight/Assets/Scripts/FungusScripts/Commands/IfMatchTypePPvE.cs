using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;


[CommandInfo("Flow",
                "IfPPvE",
                "If the test expression is true, execute the following command block.")]
[AddComponentMenu("")]
public class IfMatchTypePPvE : VariableCondition
{
    protected override bool HasNeededProperties()
    {
        return true;
    }
    protected override bool EvaluateCondition()
    {
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        if (matchType == MatchType.PPvE)
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
