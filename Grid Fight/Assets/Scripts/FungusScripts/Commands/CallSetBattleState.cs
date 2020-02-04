using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call SetBattleState",
                "Sets the game battle state")]
[AddComponentMenu("")]
public class CallSetBattleState : Command
{
    public BattleState battleState = BattleState.Battle;

    protected virtual void CallTheMethod()
    {
        BattleManagerScript.Instance.CurrentBattleState = battleState;
    }

    #region Public members

    public override void OnEnter()
    {
        CallTheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}
