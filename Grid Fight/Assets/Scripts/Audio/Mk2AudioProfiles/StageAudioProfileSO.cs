using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage Audio Profile", menuName = "ScriptableObjects/Audio/Profiles/Stage")]
public class StageAudioProfileSO : BaseAudioProfileSO
{
    [Header("General")]
    public AudioClipInfoClass music;
    public AudioClipInfoClass ambience;

    protected override void OnEnable()
    {
        allAudioClips.Add(ambience);
        allAudioClips.Add(music);

        //Default AudioBus
        ambience.audioPriority = AudioBus.LowPrio;
        music.audioPriority = AudioBus.Music;
    }
}
