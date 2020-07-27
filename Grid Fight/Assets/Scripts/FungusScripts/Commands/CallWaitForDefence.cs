using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Scripting",
                "Call CallWaitForDefence",
                "CallWaitForDefence")]
[AddComponentMenu("")]
public class CallWaitForDefence : Command
{

    bool isMoving = true;
    protected virtual void CallTheMethod()
    {
        isMoving = true;
        BattleManagerScript.Instance.CurrentSelectedCharacters[ControllerType.Player1].Character.CurrentCharStartingActionEvent += Character_CurrentCharStartingActionEvent;
        StartCoroutine(Defence());
        Continue();
    }

    private void Character_CurrentCharStartingActionEvent(ControllerType playerController, CharacterActionType action)
    {
        if(action == CharacterActionType.Defence)
        {
            isMoving = false;
        }
    }

    IEnumerator Defence()
    {
        while (isMoving)
        {
            if(BattleManagerScript.Instance.CurrentSelectedCharacters[ControllerType.Player1].Character.isDefending)
            {
                BattleManagerScript.Instance.BattleSpeed = 1;
            }
            else
            {
                BattleManagerScript.Instance.BattleSpeed = 0;
            }
            yield return null;
        }
        BattleManagerScript.Instance.BattleSpeed = 1;
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
