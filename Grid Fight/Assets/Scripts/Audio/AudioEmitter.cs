﻿using System.Collections;
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
    public bool playOnEnabled = true;

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
        AudioManager.Instance.AddAudio(this);
    }

    void SetupFromAudioTypeProfile()
    {
        dampenToPercent = audioEmitterProfile.dampen;
        priority = audioEmitterProfile.priority;
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }

    public void PauseAudio()
    {
        audioSource.Pause();
    }

    public void UpdateVolume()
    {
        audioSource.volume = baseVolume * AudioManager.Instance.GetMaxVolumeForPriority(priority);
    }

    public void SetVolume(float newVolume)
    {
        audioSource.volume = newVolume;
    }

    private void OnDisable()
    {
        AudioManager.Instance.RemoveAudio(this);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.RemoveAudio(this);
    }

    private void OnEnable()
    {
        SetupAudioEmitter();
        if (playOnEnabled) PlayAudio();
    }

    public void ChangeClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

}