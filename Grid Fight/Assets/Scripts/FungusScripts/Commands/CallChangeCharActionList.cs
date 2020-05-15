using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "CallChangeCharActionList",
                "Fires a single attack")]
[AddComponentMenu("")]
public class CallChangeCharActionList : Command
{
    public CharacterNameType characterID;
    public bool MoveAction;
    public bool WeakAttackAction;
    public bool StrongAttackAction;
    public bool Skill1Action;
    public bool Skill2Action;
    public bool Skill3Action;
    public bool DefenceAction;
    public bool SwitchCharacterAction;




    #region Public members

    public override void OnEnter()
    {
        BaseCharacter selectedChar = WaveManagerScript.Instance.WaveCharcters.Where(r => r.IsOnField && r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if(selectedChar == null)
        {
            selectedChar = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField && r.CharInfo.CharacterID == characterID).FirstOrDefault();
        }

        if (selectedChar == null)
        {
            Debug.LogError("Character is missing");
            Continue();
        }

        List<CharacterActionType> res = new List<CharacterActionType>();
        if(MoveAction)
        {
            res.Add(CharacterActionType.Move);
        }
        if (WeakAttackAction)
        {
            res.Add(CharacterActionType.WeakAttack);
        }
        if (StrongAttackAction)
        {
            res.Add(CharacterActionType.StrongAttack);
        }
        if (Skill1Action)
        {
            res.Add(CharacterActionType.Skill1);
        }
        if (Skill2Action)
        {
            res.Add(CharacterActionType.Skill2);
        }
        if (Skill3Action)
        {
            res.Add(CharacterActionType.Skill3);
        }
        if (DefenceAction)
        {
            res.Add(CharacterActionType.Defence);
        }
        if (SwitchCharacterAction)
        {
            res.Add(CharacterActionType.SwitchCharacter);
        }

        selectedChar.CharActionlist = res;

        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

