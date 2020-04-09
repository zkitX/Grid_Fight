using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ManagedAudioSource : MonoBehaviour
{
    public AudioSourceType type = AudioSourceType.Any;
    public AudioBus bus = AudioBus.LowPriority;
    protected AudioSource source = null;
    protected AudioClipInfoClass audioClipInfo;

    public Transform parent = null;

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
    }

    public void SetAudioClipInfo(AudioClipInfoClass _audioClipInfo)
    {
        audioClipInfo = _audioClipInfo;
        source.clip = audioClipInfo.clip;
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        source.volume = audioClipInfo.baseVolume;
        //SET THE VOLUME BASED ON THE BASE VOLUME OF THE CLIP INFO AND WHETHER THERE ARE DAMPENERS APPLIED BY THE MANAGER
    }

    public void PlaySound()
    {
        UpdateVolume();
        source.Play();
    }

    public void ResetSource()
    {
        bus = AudioBus.LowPriority;
        SetParent(AudioManagerMk2.Instance.transform);
        SetAudioClipInfo(new AudioClipInfoClass());
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ResetSource();
    }
}

[System.Serializable]
public class AudioClipInfoClass
{
    public AudioClip clip = null;
    [Range(0f,1f)]public float baseVolume = 1f;
}
