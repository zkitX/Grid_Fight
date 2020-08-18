using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Win Match",
                "Trigger the win state for the match")]
[AddComponentMenu("")]
public class CallWinMatch : Command
{

    protected void TheMethod()
    {
        BattleManagerScript.Instance.WonMatch();
    }

    #region Public members

    public override void OnEnter()
    {
        TheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    public override void OnValidate()
    {
        base.OnValidate();
    }
    #endregion
}
