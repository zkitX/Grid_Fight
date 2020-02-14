using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public GameObject genericAudioEmitterPrefab;
    public AudioClip[] unboundClips;
    public List<AudioEmitter> genericEmitters = new List<AudioEmitter>();
    public List<GameObject> audioEmitters = new List<GameObject>();
    [SerializeField] protected AudioEmitter dominant = null;

    [Tooltip("The background music for the level")] public AudioClip musicClip;
    protected AudioSource musicSource;

    [SerializeField] protected List<AudioClip> audioPlayedLastFrame = new List<AudioClip>();

    void Awake()
    {
        Instance = this;
        musicSource = GetComponent<AudioSource>();
        PlayMusic();
    }

    public void AddAudio(GameObject audio)
    {
        audioEmitters.Add(audio);
        UpdateAudioEmitters();
    }

    public void PlayMusic()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void RemoveAudio(GameObject audio)
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
        foreach(GameObject audioEmitter in audioEmitters)
        {
            if (dom == null) dom = audioEmitter.GetComponent<AudioEmitter>();
            else
            {
                if(dom.priority <= audioEmitter.GetComponent<AudioEmitter>().priority)
                {
                    if(dom.dampenToPercent >= audioEmitter.GetComponent<AudioEmitter>().dampenToPercent)
                    {
                        dom = audioEmitter.GetComponent<AudioEmitter>();
                    }
                }
            }
        }
        dominant = dom;
    }

    void UpdateVolumes()
    {
        foreach(GameObject audioEmitter in audioEmitters)
        {
            audioEmitter.GetComponent<AudioEmitter>().UpdateVolume();
        }
    }

    public float GetMaxVolumeForPriority(int priority)
    {
        if (priority >= dominant.priority) return 1f;
        else
        {
            float dampenAmount = 1f;
            foreach (GameObject audioEmitter in audioEmitters)
            {
                if (audioEmitter.GetComponent<AudioEmitter>().priority >= priority && dampenAmount > audioEmitter.GetComponent<AudioEmitter>().dampenToPercent)
                {
                    dampenAmount = audioEmitter.GetComponent<AudioEmitter>().dampenToPercent;
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
            emitterObject.SetActive(true);
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

    public bool ClipPlayedThisFrame(AudioClip clip)
    {
        if (audioPlayedLastFrame.Where(r => r == clip).FirstOrDefault() != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddClipPlayed(AudioClip clip)
    {
        audioPlayedLastFrame.Add(clip);
        StartCoroutine(ManageClipsPlayedLastFrame(clip));
    }

    IEnumerator ManageClipsPlayedLastFrame(AudioClip clip)
    {
        yield return null;
        audioPlayedLastFrame.Remove(clip);

    }

}
