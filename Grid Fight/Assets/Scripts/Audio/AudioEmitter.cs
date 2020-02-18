using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
    public AudioEmitterProfile audioEmitterProfile = null;
    [Tooltip("The priority level for this particular sound")]
    [Range(-100, 100)] public int priority = 0;
    [Tooltip("The percentage volume to which all of the lesser priority sounds should be dampened")]
    [Range(0f, 1f)] public float dampenToPercent = 1f;
    public AudioSource audioSource { get; private set; }
    public float baseVolume = 1f;
    public float desiredVolume = 1f;
    public bool playOnEnabled = true;
    public bool autoDisableOnComplete = false;

    IEnumerator SmoothRecoverer = null;

    void Awake()
    {
        SetupAudioEmitter();
        if (playOnEnabled) PlayAudio();
    }

    void SetupAudioEmitter()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioEmitterProfile != null) SetupFromAudioTypeProfile();
        priority = Mathf.Clamp(priority, -100, 100);
        dampenToPercent = Mathf.Clamp(dampenToPercent, 0f, 1f);
        baseVolume = audioSource.volume;
    }

    void SetupFromAudioTypeProfile()
    {
        dampenToPercent = audioEmitterProfile.dampen;
        priority = audioEmitterProfile.priority;
    }

    public void PlayAudio()
    {
        if (AudioManager.Instance.ClipPlayedThisFrame(audioSource.clip))
        {
            return;
        }
        AudioManager.Instance.AddClipPlayed(audioSource.clip);
        AudioManager.Instance.AddAudio(gameObject);
        audioSource.Play();
        if (autoDisableOnComplete && gameObject.activeInHierarchy)
        {
            StartCoroutine(AutoDisableChecker());
        }
    }

    IEnumerator AutoDisableChecker()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        AudioManager.Instance.RemoveAudio(gameObject);
        gameObject.SetActive(false);
    }

    public void PauseAudio()
    {
        audioSource.Pause();
    }

    public void UpdateVolume()
    {
        float newVolume = baseVolume * AudioManager.Instance.GetMaxVolumeForPriority(priority);
        if (newVolume < audioSource.volume)
        {
            audioSource.volume = newVolume;
            desiredVolume = newVolume;
        }
        else
        {
            desiredVolume = newVolume;
            StartSmoothRecovery();
        }
    }

    public void SetVolume(float newVolume)
    {
        audioSource.volume = newVolume;
    }

    private void OnDisable()
    {
        audioSource.volume = baseVolume;
        if (SmoothRecoverer != null) StopCoroutine(SmoothRecoverer);
        AudioManager.Instance.RemoveAudio(gameObject);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.RemoveAudio(gameObject);
    }

    private void OnEnable()
    {
        if (playOnEnabled) PlayAudio();
    }

    public void ChangeClip(AudioClip clip)
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
    }

    void StartSmoothRecovery()
    {
        if (SmoothRecoverer != null) StopCoroutine(SmoothRecoverer);
        SmoothRecoverer = SmoothAFRecovery();
        StartCoroutine(SmoothRecoverer);
    }

    IEnumerator SmoothAFRecovery()
    {
        while(audioSource.volume != desiredVolume)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, desiredVolume, Time.deltaTime * 4f);
            audioSource.volume = Mathf.Clamp(audioSource.volume, audioSource.volume, desiredVolume);
            if (Mathf.Abs(audioSource.volume - desiredVolume) < 0.05f)
            {
                audioSource.volume = desiredVolume;
            }
            yield return null;
        }
    }

}
