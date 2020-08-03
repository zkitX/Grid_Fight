using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Configure Battle Timer",
                "Sets up the values for and triggers the battle timer")]
[AddComponentMenu("")]
public class CallConfigureBattleTimer : Command
{
    public bool ChangeTimer = false;
    public GameTime battleTime = new GameTime();
    public bool StartStopState = true;
    public bool TriggerFungusOnComplete = false;
    [ConditionalField("TriggerFungusOnComplete")] public string BlockToTriggerOnComplete = "";

    #region Public members

    public override void OnEnter()
    {
        WaveManagerScript.Instance.SetBattleTimer(ChangeTimer, battleTime.hours, battleTime.minutes, battleTime.seconds, StartStopState, TriggerFungusOnComplete ? BlockToTriggerOnComplete : "");
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}
