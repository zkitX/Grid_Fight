using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Pause For Button Press",
                "Allow time for a button press, and action according to if it was pressed or not")]
[AddComponentMenu("")]
public class CallPauseForButtonPress : Command
{
    public InputButtonType buttonToWaitFor = InputButtonType.Plus;
    public bool waitIndefinitely = true;
    [ConditionalField("waitIndefinitely", inverse: true)] public float secondsToWaitFor = 10f;

    public enum ConditionAction { FireBlock, Continue }
    public ConditionAction successAction = ConditionAction.FireBlock;
    [ConditionalField("successAction", false, ConditionAction.FireBlock)] public string blockToFireOnSuccess = "";
    public ConditionAction failureAction = ConditionAction.Continue;
    [ConditionalField("failureAction", false, ConditionAction.FireBlock)] public string blockToFireOnFailure = "";

    IEnumerator TheMethod()
    {
        float timePassed = 0f;
        while (timePassed < secondsToWaitFor || waitIndefinitely)
        {
            if (EventManager.Instance.GetButtonWasPressedLastFrame(buttonToWaitFor))
            {
                IfAction();
                yield break;
            }
            timePassed += Time.deltaTime;
            yield return null;
        }
        ElseAction();
    }

    void IfAction()
    {
        if (successAction == ConditionAction.Continue) Continue();
        else SetNextBlockFromName(blockToFireOnSuccess);
    }

    void ElseAction()
    {
        if (failureAction == ConditionAction.Continue) Continue();
        else SetNextBlockFromName(blockToFireOnFailure);
    }

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(TheMethod());
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