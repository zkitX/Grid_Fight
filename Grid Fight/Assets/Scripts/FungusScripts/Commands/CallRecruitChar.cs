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
    [Header("General")]
    public CharacterNameType CharacterId;
    [Header("Analytics")]
    public bool track = true;
    public bool onlyTrackFirstRecruitment = false;

    #region Public members

    public override void OnEnter()
    {
        BattleManagerScript.Instance.RecruitCharFromWave(CharacterId, track, onlyTrackFirstRecruitment);
        Continue();
    }

    
    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}