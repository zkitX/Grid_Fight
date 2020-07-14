using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallUpdatePartyRelationship",
                "CallUpdatePartyRelationship")]
[AddComponentMenu("")]
public class CallUpdatePartyRelationship : Command
{
    #region Public members

    public override void OnEnter()
    {
        List<TargetRecruitableClass> rel = new List<TargetRecruitableClass>();


        foreach (var item in BattleManagerScript.Instance.AllCharactersOnField.Where(r=> r.CharInfo.CharacterID != CharacterNameType.CleasTemple_Character_Valley_Donna))
        {
            rel.Add(new TargetRecruitableClass(item.CharInfo.CharacterID, 1));
        }
        BattleManagerScript.Instance.UpdateCharactersRelationship(true, new List<CharacterNameType>(), rel);
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

