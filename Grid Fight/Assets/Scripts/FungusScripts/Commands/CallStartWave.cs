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


    public string WaveName;
    public string NextBlockToFire;

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(Wave());
    }

    private IEnumerator Wave()
    {
        yield return WaveManagerScript.Instance.StartWaveByName(WaveName);
        SetNextBlockFromName(NextBlockToFire);
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

