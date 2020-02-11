using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public GameObject genericAudioEmitterPrefab;
    public AudioClip[] unboundClips;
    public List<AudioEmitter> genericEmitters = new List<AudioEmitter>();
    public List<AudioEmitter> audioEmitters = new List<AudioEmitter>();
    protected AudioEmitter dominant = null;

    void Awake()
    {
        Instance = this;
    }

    public void AddAudio(AudioEmitter audio)
    {
        audioEmitters.Add(audio);
        UpdateAudioEmitters();
    }

    public void RemoveAudio(AudioEmitter audio)
    {
        audioEmitters.Remove(audio);
        UpdateAudioEmitters();
    }

    void UpdateAudioEmitters()
    {
        UpdateDominant();
        UpdateVolumes();
    }

    void UpdateDominant()
    {
        AudioEmitter dom = null;
        foreach(AudioEmitter audioEmitter in audioEmitters)
        {
            if (dom == null) dom = audioEmitter;
            else
            {
                if(dom.priority <= audioEmitter.priority)
                {
                    if(dom.dampenToPercent >= audioEmitter.dampenToPercent)
                    {
                        dom = audioEmitter;
                    }
                }
            }
        }
        dominant = dom;
    }

    void UpdateVolumes()
    {
        foreach(AudioEmitter audioEmitter in audioEmitters)
        {
            audioEmitter.UpdateVolume();
        }
    }

    public float GetMaxVolumeForPriority(int priority)
    {
        if (priority >= dominant.priority) return 1f;
        else
        {
            float dampenAmount = 1f;
            foreach (AudioEmitter audioEmitter in audioEmitters)
            {
                if (audioEmitter.priority >= priority && dampenAmount > audioEmitter.dampenToPercent)
                {
                    dampenAmount = audioEmitter.dampenToPercent;
                }
            }
            return dampenAmount;
        }
    }

    public void PlayGeneric(string unboundClipName)
    {
        if (unboundClips.Length == 0) return;
        else if (unboundClips.Where(r => r.name == unboundClipName).FirstOrDefault() == null) return;
        GameObject emitterObject;
        if (genericEmitters.Count != 0 && genericEmitters.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault() != null)
        {
            emitterObject = genericEmitters.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault().gameObject;
        }
        else
        {
            emitterObject = Instantiate(genericAudioEmitterPrefab, transform);
            genericEmitters.Add(emitterObject.GetComponent<AudioEmitter>());
        }
        AudioEmitter emitter = emitterObject.GetComponent<AudioEmitter>();
        emitter.ChangeClip(unboundClips.Where(r => r.name == unboundClipName).FirstOrDefault());
        emitter.PlayAudio();
    }
}
