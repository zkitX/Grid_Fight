using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallItemIsPickedAndWait",
                "CallItemIsPickedAndWait")]
[AddComponentMenu("")]
public class CallItemIsPickedAndWait : Command
{
    public float WaitingTime = 1;
    public bool StopPotionSpawning = false;

    #region Public members

    public override void OnEnter()
    {
        ItemSpawnerManagerScript.Instance.ItemPickedUpEvent += Instance_ItemPickedUpEvent;
    }

    private void Instance_ItemPickedUpEvent()
    {
        StartCoroutine(WaitFor());
        if(StopPotionSpawning)
        {
            ItemSpawnerManagerScript.Instance.PauseSpawning();
        }
    }

    IEnumerator WaitFor()
    {
        yield return BattleManagerScript.Instance.WaitFor(WaitingTime, () => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        ItemSpawnerManagerScript.Instance.ItemPickedUpEvent -= Instance_ItemPickedUpEvent;
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

