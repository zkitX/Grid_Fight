﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Start Battle Timer",
                "Starts or stops the battle timer")]
[AddComponentMenu("")]
public class CallStartBattleTimer : Command
{
    public bool timerState = true;

    #region Public members

    public override void OnEnter()
    {
        if(timerState) StartCoroutine(WaveManagerScript.Instance.battleTime.standardReverseTicker);
        else StopCoroutine(WaveManagerScript.Instance.battleTime.standardReverseTicker);
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}
