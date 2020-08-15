using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Stop Audio",
                "Stop playing a named audio clip")]
[AddComponentMenu("")]
public class CallStopAudio : Command
{
    public string audioID = "";
    [SerializeField] float fadeoutTime = 0.0f;

    protected void TheMethod()
    {
        StartCoroutine(StopAudio());
    }


    IEnumerator StopAudio()
    {
        AudioManagerMk2.Instance.StopNamedSource(audioID, fadeoutTime);
        yield return new WaitForEndOfFrame();
        Continue();
    }

    #region Public members

    public override void OnEnter()
    {
        TheMethod();
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
