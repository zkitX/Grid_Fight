using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(AudioSource))]
public class ManagedAudioSource : MonoBehaviour
{
    public AudioSourceType type = AudioSourceType.Any;
    public AudioBus bus = AudioBus.LowPriority;
    protected AudioSource source = null;
    protected AudioClipInfoClass audioClipInfo;

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
        source.volume = audioClipInfo.baseVolume * AudioManagerMk2.Instance.GetDampener(type, bus);
        //SET THE VOLUME BASED ON THE BASE VOLUME OF THE CLIP INFO AND WHETHER THERE ARE DAMPENERS APPLIED BY THE MANAGER
    }

    public void PlaySound(bool looped = false)
    {
        source.loop = looped;
        UpdateVolume();
        source.Play();
        if (!looped)
        {
            if (ResetAfterCompleteSequencer != null) StopCoroutine(ResetAfterCompleteSequencer);
            ResetAfterCompleteSequencer = ResetAfterCompleteSequence();
            StartCoroutine(ResetAfterCompleteSequencer);
        }
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

    public void ResetSource()
    {
        bus = AudioBus.LowPriority;
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

    [ConditionalField("moreThanOneClip", false)] public bool randomiseOrder = false;
    [HideInInspector] public bool moreThanOneClip = false;

    [Range(0f,1f)]public float baseVolume = 1f;

    public AudioClipInfoClass()
    {

    }

    public AudioClipInfoClass(AudioClip _clip, float _volume = 1f)
    {
        clip = _clip;
        baseVolume = _volume;
    }
}
