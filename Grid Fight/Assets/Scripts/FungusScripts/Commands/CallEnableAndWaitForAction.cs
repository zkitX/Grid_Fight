using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallEnableAndWaitForAction",
                "CallEnableAndWaitForAction")]
[AddComponentMenu("")]
public class CallEnableAndWaitForAction : Command
{
    public ControllerType PlayerController;
    public CharacterActionType Action;

    public float WaitingTime = 1;

    #region Public members

    public override void OnEnter()
    {
        if(BattleManagerScript.Instance.CurrentSelectedCharacters[PlayerController].Character != null)
        {
            BattleManagerScript.Instance.CurrentSelectedCharacters[PlayerController].Character.CurrentCharStartingActionEvent += Character_CurrentCharStartingActionEvent;
        }
    }

    private void Character_CurrentCharStartingActionEvent(ControllerType playerController, CharacterActionType action)
    {
        if(Action == action)
        {
            StartCoroutine(WaitFor());
        }
    }

    IEnumerator WaitFor()
    {
        yield return BattleManagerScript.Instance.WaitFor(WaitingTime, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

