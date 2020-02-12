using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call CallStartWave",
                "CallStartWave")]
[AddComponentMenu("")]
public class CallStartWave : Command
{
    #region Public members

    public override void OnEnter()
    {
        WaveManagerScript.Instance.StartWave = true;
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

