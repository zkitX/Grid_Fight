using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call CallSetBattleSpeed",
                "Sets the game battle speed")]
[AddComponentMenu("")]
public class CallSetBattleSpeed : Command
{
    public float Speed;
    protected virtual void CallTheMethod()
    {
        BattleManagerScript.Instance.BattleSpeed = Speed;
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
