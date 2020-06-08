using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Audio Mixer Snapshot Transition",
                "Trigger a Snapshot transition")]
[AddComponentMenu("")]
public class CallAudioMixerSnapshotTransition : Command
{
    [SerializeField] UnityEngine.Audio.AudioMixerSnapshot snapshot;
    [SerializeField] float transitionTime = 0.1f;

    protected void TheMethod()
    {
        snapshot.TransitionTo(transitionTime);
        Continue();
    }

    //ManagedAudioSource audioSource = null;
    //IEnumerator WaitForSoundToEnd()
    //{
    //    if (audioSource == null || loopSound == true || !pausedUntilPlayed)
    //    {
    //        Continue();
    //        yield break;
    //    }

    //    yield return null;

    //    while (audioSource.isPlaying)
    //    {
    //        yield return null;
    //    }

    //    Continue();
    //}

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
