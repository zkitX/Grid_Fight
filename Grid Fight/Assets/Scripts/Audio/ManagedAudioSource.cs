using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(AudioSource))]
public class ManagedAudioSource : MonoBehaviour
{
    public AudioSourceType type = AudioSourceType.Any;
    protected AudioBus bus = AudioBus.LowPrio;
    public AudioBus Bus
    {
        get
        {
            return bus;
        }
        set
        {
            bus = value;
            if(AudioManagerMk2.Instance.mixer.FindMatchingGroups(bus.ToString()).Length != 0)
            {
                source.outputAudioMixerGroup = AudioManagerMk2.Instance.mixer.FindMatchingGroups(bus.ToString())[0];
            }
        }
    }
    [HideInInspector] public AudioSource source = null; //GIORGIO: from protected to public, check if it's ok.
    protected AudioClipInfoClass audioClipInfo;
    public bool isPlaying
    {
        get
        {
            return source.isPlaying;
        }
    }

    public bool removeNamedOnComplete = false;

    public Transform parent = null;
    public AudioClip fallbackSound = null;

    public AudioClip CurrentClip
    {
        get
        {
            return audioClipInfo.clip;
        }
        set
        {

        }
    }
    public float Volume
    {
        get
        {
            return source.volume;
        }
        set
        {
            source.volume = value;
        }
    }

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void SetParent(Transform _parent)
    {
        parent = _parent;
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        enabled = false;
    }

    public void SetAudioClipInfo(AudioClipInfoClass _audioClipInfo)
    {
        audioClipInfo = _audioClipInfo;
        source.clip = audioClipInfo.Clip == null ? fallbackSound : audioClipInfo.ClipAndPlay;
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        source.volume = audioClipInfo.baseVolume * AudioManagerMk2.Instance.GetDampener(type, Bus);
        //SET THE VOLUME BASED ON THE BASE VOLUME OF THE CLIP INFO AND WHETHER THERE ARE DAMPENERS APPLIED BY THE MANAGER
    }

    public void PlaySound(UnityEngine.Audio.AudioMixerGroup audioBus, AudioBus priority, bool looped = false, float fadeInDuration = 0.0f)
    {
        source.loop = looped;
        //source.outputAudioMixerGroup = AudioManagerMk2.Instance.AssignMixerGroupPriority(priority);
        if (audioBus != null)
            source.outputAudioMixerGroup = audioBus;
        else
            source.outputAudioMixerGroup = AudioManagerMk2.Instance.AssignMixerGroupPriority(priority);

        if (fadeInDuration == 0.0f)
        {
            UpdateVolume();
            source.Play();
        }
        else
        {
            source.volume = 0.0f;
            StartCoroutine(StartFade(source, fadeInDuration, audioClipInfo.baseVolume * AudioManagerMk2.Instance.GetDampener(type, Bus)));
            source.Play();
            
        }
            
        if (!looped)
        {
            if (ResetAfterCompleteSequencer != null) StopCoroutine(ResetAfterCompleteSequencer);
            ResetAfterCompleteSequencer = ResetAfterCompleteSequence();
            StartCoroutine(ResetAfterCompleteSequencer);
        }
    }

    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        
        yield break;
    }
    
    IEnumerator ResetAfterCompleteSequencer = null;
    IEnumerator ResetAfterCompleteSequence()
    {
        while (source.isPlaying)
        {
            yield return new WaitForFixedUpdate();
        }
        ResetAfterCompleteSequencer = null;
        ResetSource();
    }

    public static IEnumerator ResetAfterFadeOut(ManagedAudioSource source)
    {
        while (source.Volume > 0.01f)
        {
            yield return new WaitForFixedUpdate();
        }
        source.ResetSource();
    }

    public void ResetSource()
    {
        Bus = AudioBus.LowPrio;
        source.loop = false;
        SetParent(AudioManagerMk2.Instance.transform);
        SetAudioClipInfo(new AudioClipInfoClass());
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Debug.Log("Audio Source Disabled");
    }
}

[System.Serializable]
public class AudioClipInfoClass
{
    [HideInInspector] public AudioClip clip = null;
    public AudioClip[] clips = null;
    int lastPlayedClip = 0;

    [HideInInspector] public AudioClip ClipAndPlay
    {
        get
        {
            if (clips != null && clips.Length > 0)
            {
                if (randomiseOrder)
                {
                    return clips[Random.Range(0, clips.Length)];
                }
                else
                {
                    AudioClip clipToReturn = clips[lastPlayedClip];
                    lastPlayedClip = (lastPlayedClip + 1) % (clips.Length);
                    return clipToReturn;
                }
            }
            else return AudioManagerMk2.Instance.useLegacySoundsWhenPossible ? clip : null;
        }
        set
        {
            
        }
    }
    [HideInInspector] public AudioClip Clip
    {
        get
        {
            if (clips != null && clips.Length > 0)
            {
                if (randomiseOrder)
                {
                    return clips[Random.Range(0, clips.Length)];
                }
                else
                {
                    AudioClip clipToReturn = clips[lastPlayedClip];
                    return clipToReturn;
                }
            }
            else return AudioManagerMk2.Instance.useLegacySoundsWhenPossible ? clip : null;
        }
        set
        {

        }
    }

    [HideInInspector][UnityEngine.Serialization.FormerlySerializedAs("audioBus")] public AudioBus audioPriority = AudioBus.LowPrio;
    public UnityEngine.Audio.AudioMixerGroup audioBus;

    [ConditionalField("moreThanOneClip", false)] public bool randomiseOrder = false;
    [HideInInspector] public bool moreThanOneClip = false;

    [HideInInspector] public enum AudioCooldownType { FrameWait, SecondWait }
    public AudioCooldownType cooldownType = AudioCooldownType.FrameWait;
    [ConditionalField("cooldownType", false, AudioCooldownType.SecondWait)] public float cooldownPeriod = 1f;

    [Range(0f,1f)]public float baseVolume = 1f;

    public AudioClipInfoClass()
    {

    }

    public AudioClipInfoClass(AudioClip _clip, float _volume = 1f, AudioCooldownType cdType = AudioCooldownType.FrameWait, int cdPeriod = 1)
    {
        clip = _clip;
        baseVolume = _volume;
        cooldownType = cdType;
        cooldownPeriod = cdPeriod;
    }

    public AudioClipInfoClass(AudioClip[] _clips, float _volume = 1f, AudioCooldownType cdType = AudioCooldownType.FrameWait, int cdPeriod = 1, AudioBus bus = AudioBus.LowPrio)
    {
        clips = _clips;
        baseVolume = _volume;
        cooldownType = cdType;
        cooldownPeriod = cdPeriod;
        audioPriority = bus;
    }
}
