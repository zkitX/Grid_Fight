using Fungus;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CommandInfo("Scripting",
                "Call CallRecruitChar",
                "CallRecruitChar")]
[AddComponentMenu("")]
public class CallRecruitChar : Command
{

    public CharacterNameType CharacterId;

    #region Public members

    public override void OnEnter()
    {
        BattleManagerScript.Instance.RecruitCharFromWave(CharacterId);
        Continue();
    }

    
    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}