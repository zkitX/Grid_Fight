using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Play Audio",
                "Play an audio clip through the audio manager")]
[AddComponentMenu("")]
public class CallPlayAudio : Command
{
    public AudioClip clip = null;
    [Range(0f, 1f)] public float volume = 1f;
    public AudioSourceType type = AudioSourceType.Game;
    public AudioBus bus = AudioBus.Music;

    [Space(10)]
    public bool loopSound = false;
    public string audioID = "";
    [ConditionalField("loopSound", inverse: true)] public bool pausedUntilPlayed = false;

    protected void TheMethod()
    {
        audioSource = AudioManagerMk2.Instance.PlayNamedSource(audioID, type, new AudioClipInfoClass(clip, volume), bus, loop: loopSound);
        StartCoroutine(WaitForSoundToEnd());
    }

    ManagedAudioSource audioSource = null;
    IEnumerator WaitForSoundToEnd()
    {
        if (audioSource == null || loopSound == true || !pausedUntilPlayed)
        {
            Continue();
            yield break;
        }

        yield return null;

        while (audioSource.isPlaying)
        {
            yield return null;
        }

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
