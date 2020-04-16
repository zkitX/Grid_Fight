using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManagerMk2 : MonoBehaviour
{
    public static AudioManagerMk2 Instance = null;
    [SerializeField] protected List<ManagedAudioSource> sources = new List<ManagedAudioSource>();
    public GameObject sourceObjectPrefab;

    [Header("Source Type Configuration")]
    public int musicSourcesNum = 3;
    public int ambienceSourcesNum = 2;
    public int uiSourcesNum = 1;
    public int sourcesPerChar = 3;

    private void Awake()
    {
        Instance = this;
        GenerateSources();
    }

    void GenerateSources()
    {
        for (int i = 0; i < musicSourcesNum; i++) CreateSource(AudioSourceType.Music, AudioBus.Music);
        for (int i = 0; i < ambienceSourcesNum; i++) CreateSource(AudioSourceType.Ambience, AudioBus.LowPriority);
        for (int i = 0; i < uiSourcesNum; i++) CreateSource(AudioSourceType.Ui, AudioBus.LowPriority);
        for (int i = 0; i < sourcesPerChar * (WaveManagerScript.Instance.GetMaxEnemiesOnScreenAcrossAllWaves() +
            BattleInfoManagerScript.Instance.PlayerBattleInfo.Count); i++) CreateSource(AudioSourceType.Game, AudioBus.LowPriority);
    }

    void CreateSource(AudioSourceType type, AudioBus bus)
    {
        ManagedAudioSource tempSource;
        tempSource = Instantiate(sourceObjectPrefab, transform).GetComponent<ManagedAudioSource>();
        tempSource.type = type;
        tempSource.bus = bus;
        tempSource.SetParent(transform);
        tempSource.gameObject.SetActive(false);
        sources.Add(tempSource);
    }

    ManagedAudioSource GetFreeSource(AudioBus priority, AudioSourceType sourceType)
    {
        ManagedAudioSource source = sources.Where(r => !r.gameObject.activeInHierarchy && r.type == sourceType).FirstOrDefault();
        if (source == null) source = sources.Where(r => r.bus < priority && r.type == sourceType).FirstOrDefault();
        if (source == null)
        {
            source = sources.Where(r => !r.gameObject.activeInHierarchy && r.bus != AudioBus.Music).FirstOrDefault();
            source.type = sourceType;
        }
        if (source == null) Debug.LogError("Insufficient Sources");
        return source;
    }

    public ManagedAudioSource PlaySound(AudioSourceType sourceType, AudioClipInfoClass clipInfo, AudioBus priority, Transform sourceOrigin = null, bool loop = false)
    {
        ManagedAudioSource source = GetFreeSource(priority, sourceType);

        source.gameObject.SetActive(true);
        if (sourceOrigin != null) source.SetParent(sourceOrigin);
        source.SetAudioClipInfo(clipInfo);
        source.bus = priority;
        source.PlaySound(loop);

        return source;
    }

    public float GetDampener(AudioSourceType type, AudioBus priorityToCompare)
    {
        return sources.Where(r => r.bus == AudioBus.HighPriority && r.type == type && priorityToCompare != AudioBus.HighPriority).FirstOrDefault() == null ? 1f : 0.5f;
    }
}
