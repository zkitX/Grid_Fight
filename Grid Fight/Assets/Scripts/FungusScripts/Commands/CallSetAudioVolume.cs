using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Set Audio Volume",
                "Changes Volume to an Audio ID Smoothly")]
[AddComponentMenu("")]
public class CallSetAudioVolume : Command
{
    public string audioID = "";
    [SerializeField] float volume = 1f;
    [SerializeField] float fadeoutTime = 0.0f;

    protected void TheMethod()
    {
        AudioManagerMk2.Instance.SetAudioVolume(audioID, volume, fadeoutTime);
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
