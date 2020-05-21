using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallClearCharBoard",
                "CallStartWave")]
[AddComponentMenu("")]
public class CallClearCharBoard : Command
{
    #region Public members
    public override void OnEnter()
    {
        BattleManagerScript.Instance.RemoveAllNonUsedCharFromBoard(new List<CharacterNameType>());
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

