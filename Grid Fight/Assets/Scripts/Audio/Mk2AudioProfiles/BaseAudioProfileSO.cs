using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAudioProfileSO : ScriptableObject
{
    //The base audio profile behaviour
    protected List<AudioClipInfoClass> allAudioClips = new List<AudioClipInfoClass>();

    protected virtual void OnEnable()
    {
        
    }

    private void OnValidate()
    {
        foreach(AudioClipInfoClass audioClip in allAudioClips)
        {
            audioClip.moreThanOneClip = audioClip.clips != null ? audioClip.clips.Length > 1 ? true : false : false;
        }
    }



}
