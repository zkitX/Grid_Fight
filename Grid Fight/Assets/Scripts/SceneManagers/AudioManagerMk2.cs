using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class AudioManagerMk2 : MonoBehaviour
{
    public static AudioManagerMk2 Instance = null;
    [SerializeField] protected List<ManagedAudioSource> sources = new List<ManagedAudioSource>();
    public GameObject sourceObjectPrefab;

    public AudioMixer mixer;
    public AudioMixerGroup mg_music;
    public AudioMixerGroup mg_high;
    public AudioMixerGroup mg_mid;
    public AudioMixerGroup mg_low;

    public List<NamedManagedAudioSource> namedSources = new List<NamedManagedAudioSource>();

    [Header("Source Type Configuration")]
    public int musicSourcesNum = 3;
    public int ambienceSourcesNum = 2;
    public int uiSourcesNum = 1;
    public int sourcesPerChar = 3;

    public bool useLegacySoundsWhenPossible = true;

    protected List<AudioClip> audioPlayedLastFrame = new List<AudioClip>();

    private void Awake()
    {
        Instance = this;
        GenerateSources();
    }

    void GenerateSources()
    {
        for (int i = 0; i < musicSourcesNum; i++) CreateSource(AudioSourceType.Music, AudioBus.Music);
        for (int i = 0; i < ambienceSourcesNum; i++) CreateSource(AudioSourceType.Ambience, AudioBus.LowPrio);
        for (int i = 0; i < uiSourcesNum; i++) CreateSource(AudioSourceType.Ui, AudioBus.LowPrio);
        for (int i = 0; i < sourcesPerChar * (WaveManagerScript.Instance.GetMaxEnemiesOnScreenAcrossAllWaves() +
            BattleInfoManagerScript.Instance.PlayerBattleInfo.Count); i++) CreateSource(AudioSourceType.Game, AudioBus.LowPrio);
    }

    void CreateSource(AudioSourceType type, AudioBus bus)
    {
        ManagedAudioSource tempSource;
        tempSource = Instantiate(sourceObjectPrefab, transform).GetComponent<ManagedAudioSource>();
        tempSource.type = type;
        tempSource.Bus = bus;
        tempSource.SetParent(transform);
        tempSource.gameObject.SetActive(false);
        sources.Add(tempSource);
    }

    ManagedAudioSource GetFreeSource(AudioBus priority, AudioSourceType sourceType)
    {
        ManagedAudioSource source = sources.Where(r => !r.gameObject.activeInHierarchy && r.type == sourceType).FirstOrDefault();
        if (source == null) source = sources.Where(r => r.Bus < priority && r.type == sourceType).FirstOrDefault();
        if (source == null)
        {
            source = sources.Where(r => !r.gameObject.activeInHierarchy && r.Bus != AudioBus.Music).FirstOrDefault();
            source.type = sourceType;
        }
        if (source == null) Debug.LogError("Insufficient Sources");
        return source;
    }

    public ManagedAudioSource PlayNamedSource(string name, AudioSourceType sourceType, AudioClipInfoClass clipInfo, AudioBus priority, Transform sourceOrigin = null, bool loop = false)
    {
        ManagedAudioSource audioSource = PlaySound(sourceType, clipInfo, priority, sourceOrigin, loop);
        if (name == "") return null;
        namedSources.Add(new NamedManagedAudioSource(name, audioSource));
        audioSource.removeNamedOnComplete = true;
        return audioSource;
    }

    public void StopNamedSource(string name)
    {
        NamedManagedAudioSource audioSource = namedSources.Where(r => r.name == name).FirstOrDefault();
        if (audioSource == null) return;
        namedSources.Remove(audioSource);
        audioSource.source.ResetSource();
    }

    public ManagedAudioSource PlaySound(AudioSourceType sourceType, AudioClipInfoClass clipInfo, AudioBus priorityObsolete, Transform sourceOrigin = null, bool loop = false, float fadeInDuration = 0.0f)
    {
        if (ClipPlayedThisFrame(clipInfo.Clip)) return null;

        ManagedAudioSource source = GetFreeSource(clipInfo.audioBus, sourceType);

        source.removeNamedOnComplete = false;
        source.gameObject.SetActive(true);
        if (sourceOrigin != null) source.SetParent(sourceOrigin);
        source.SetAudioClipInfo(clipInfo);
        //source.Bus = priority;
        source.PlaySound(loop, fadeInDuration, clipInfo.audioBus);
        AddClipPlayedLastFrame(clipInfo);

        if (fadeInDuration == 0.0f) UpdateActiveAudioVolumes();
        return source;
    }

    public AudioMixerGroup AssignMixerGroupPriority (AudioBus _priority)
    {
        switch (_priority)
        {
            case AudioBus.HighPrio:
                return mg_high;
            case AudioBus.MidPrio:
                return mg_mid;
            case AudioBus.LowPrio:
                return mg_low;
            case AudioBus.Music:
                return mg_music;
            default:
                return mg_low;
        }
    }

    public float GetDampener(AudioSourceType type, AudioBus priorityToCompare)
    {
        return sources.Where(r => r.Bus == AudioBus.HighPrio && r.type == type && priorityToCompare != AudioBus.HighPrio).FirstOrDefault() == null ? 1f : 0.5f;
    }

    void UpdateActiveAudioVolumes()
    {
        foreach (ManagedAudioSource audioSource in sources.Where(r => r.gameObject.activeInHierarchy).ToList())
        {
            audioSource.UpdateVolume();
        }
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

    public void AddClipPlayedLastFrame(AudioClipInfoClass clip)
    {
        foreach(AudioClip aClip in clip.clips)
        {
            audioPlayedLastFrame.Add(aClip);
            StartCoroutine(ManageClipsPlayedLastFrame(aClip, clip.cooldownPeriod, clip.cooldownType));
        }
    }

    IEnumerator ManageClipsPlayedLastFrame(AudioClip clip, float waitTime, AudioClipInfoClass.AudioCooldownType cdType)
    {
        if (cdType == AudioClipInfoClass.AudioCooldownType.SecondWait) yield return new WaitForSeconds(waitTime);
        else yield return null;
        audioPlayedLastFrame.Remove(clip);
    }
}

public class NamedManagedAudioSource
{
    public string name = "";
    public ManagedAudioSource source;

    public NamedManagedAudioSource(string _name, ManagedAudioSource _source)
    {
        name = _name;
        source = _source;
    }
}
