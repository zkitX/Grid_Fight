using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call RemoveCharFromBoard",
                "Removes the chatacter from the board, deselects them if they were selected")]
[AddComponentMenu("")]
public class CallRemoveCharFromBoard : Command
{
    public CharacterNameType charToRemove;

    protected virtual void CallTheMethod()
    {
        BattleManagerScript.Instance.RemoveNamedCharacterFromBoard(charToRemove);
        BattleManagerScript.Instance.DeselectCharacter(charToRemove);
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
