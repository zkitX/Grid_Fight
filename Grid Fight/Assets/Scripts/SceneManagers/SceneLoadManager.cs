using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[RequireComponent(typeof(CanvasRenderer))]
public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance;
    public GameObject NavigatorPrefab = null;
    public GameObject RewiredPrefab = null;

    protected string startingSceneID = "";
    [SerializeField] protected const float loadingFadeTime = 2f;
    public CanvasGroup canv = null;
    protected bool loadingScene = false;
    [HideInInspector] public StageProfile stagePrimedToLoad = null;

    public CharacterLoadInformation[] loadedCharacters = new CharacterLoadInformation[0];
    public MaskLoadInformation[] loadedMasks = new MaskLoadInformation[1];

    [HideInInspector] public Dictionary<int, CharacterLoadInformation> squad = new Dictionary<int, CharacterLoadInformation>()
    {
        {0, new CharacterLoadInformation() },
        {1, new CharacterLoadInformation() },
        {2, new CharacterLoadInformation() },
        {3, new CharacterLoadInformation() }
    };
    #region SquadManagement

    public delegate void SquadChange();
    public event SquadChange SquadChangeEvent;

    public bool AddSquadMate(CharacterNameType charName)
    {
        CharacterLoadInformation loadInfo = loadedCharacters.Where(r => r.characterID == charName).FirstOrDefault();

        if (squad.Values.Where(r => r.characterID == loadInfo.characterID).FirstOrDefault() != null) return false; //If the character isn't already in the squad
        if (Instance.squad.Values.Where(r => r.characterID == CharacterNameType.None).FirstOrDefault() == null) return false; //If the squad has a free slot

        squad[squad.Where(r => r.Value.characterID == CharacterNameType.None).First().Key] = loadInfo;
        SquadChangeEvent?.Invoke();
        return true;
    }

    public bool RemoveSquadMate(CharacterNameType charName)
    {
        if (charName == CharacterNameType.Stage00_Character_Valley) return false;
        if (squad.Values.Where(r => r.characterID == charName).FirstOrDefault() == null) return false; //If the character isn't in the squad

        squad[squad.Where(r => r.Value.characterID == charName).First().Key] = new CharacterLoadInformation();
        CollapseSquad();
        SquadChangeEvent?.Invoke();
        return true;
    }

    public void CollapseSquad()
    {
        for (int i = 0; i < squad.Count; i++)
        {
            if (squad[i].characterID == CharacterNameType.None)
            {
                for (int j = i + 1; j < squad.Count; j++)
                {
                    int k = i - j;
                    if(squad[j].characterID != CharacterNameType.None)
                    {
                        squad[i] = squad[j];
                        squad[j] = new CharacterLoadInformation();
                        break;
                    }
                }
            }
        }
    }

    #endregion

    public float[] charLevelThresholds = { 2000, 4500, 7000 };
    public float[] maskLevelThresholds = { 2000, 4500, 7000 };

    private void Awake()
    {
        if (Instance != null)
        { 
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (InputController.Instance == null) Instantiate(RewiredPrefab).name = RewiredPrefab.name;
        DontDestroyOnLoad(this);

        startingSceneID = SceneManager.GetActiveScene().name;
        squad[0] = loadedCharacters.Where(r => r.characterID == CharacterNameType.Stage00_Character_Valley).FirstOrDefault();
    }

    private void Start()
    {
        StartCoroutine(InitialLoadCo());
    }

    IEnumerator InitialLoadCo()
    {
        //Do all the pregame loading here
        //
        //
        //
        //
        yield return null;

        yield return FadeLoadingInOut(true, 0f);
        if (Grid_UINavigator.Instance == null) Instantiate(NavigatorPrefab).name = NavigatorPrefab.name;
        yield return new WaitForSecondsRealtime(1f);
        yield return FadeLoadingInOut(false);
    }




    public void LoadScene(string sceneToLoad, string sceneToDeload = null, float gracePeriod = 2f)
    {
        LoadScenes(sceneToLoad == null ? new List<string>() : new List<string> { sceneToLoad },
            sceneToDeload == null ? new List<string>() : new List<string> { sceneToDeload });
    }

    public void LoadScenes(List<string> scenesToLoad, List<string> scenesToDeload = null, float gracePeriod = 2f)
    {
        if (currentlyLoading) return;

        StartCoroutine(LoadSceneCo(scenesToLoad == null ? new string[0] : scenesToLoad.ToArray(),
           scenesToDeload == null ? new string[0] : scenesToDeload.ToArray(), gracePeriod: gracePeriod));
    }




    bool currentlyLoading = false;
    int scenesLoading = 0;
    IEnumerator LoadSceneCo(string[] scenesToLoad, string[] scenesToDeload, bool instantCurtain = false, float gracePeriod = 2f)
    {
        currentlyLoading = true;
        scenesLoading = scenesToLoad.Length + scenesToDeload.Length;

        yield return FadeLoadingInOut(true, instantCurtain ? 0f : default);

        List<AsyncOperation> asyncs = new List<AsyncOperation>();

        if (scenesToLoad.Length != 0)
        {
            foreach (string scene in scenesToLoad) asyncs.Add(SceneManager.LoadSceneAsync(scene));
        }
        if(scenesToDeload.Length != 0)
        {
            foreach (string scene in scenesToDeload) asyncs.Add(SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(scene)));
        }
        foreach (AsyncOperation asy in asyncs)
        {
            asy.completed += SceneLoadSeqComplete;
        }


        while (scenesLoading != 0)
        {
            Debug.Log(asyncs[0].progress);
            yield return new WaitForSecondsRealtime(1f);
        }

        yield return null;

        yield return new WaitForSecondsRealtime(gracePeriod);

        foreach (string scene in scenesToLoad)
        {
            if (scene.Contains("BattleScene"))
            {
                yield return BattleManagerBaseObjectGeneratorScript.Instance.ConfigureBattleScene(stagePrimedToLoad.ID);
            }
        }

        yield return new WaitForSecondsRealtime(gracePeriod);

        yield return FadeLoadingInOut(false);

        currentlyLoading = false;
    }




    public void SceneLoadSeqComplete(AsyncOperation asy)
    {
        scenesLoading--;
    }






    IEnumerator FadeLoadingInOut(bool state, float loadingTime = loadingFadeTime)
    {
        float endOpacity = state ? 1f : 0f;
        loadingTime = loadingTime <= 0 ? 0.0001f : loadingTime;
        float timeLeft = loadingTime;
        while(timeLeft != 0f)
        {
            timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0f, 999f);
            canv.alpha = Mathf.Lerp(canv.alpha, endOpacity, 1f - (timeLeft / loadingTime));

            yield return null;
        }
    }



    private void OnEnable()
    {
        if (canv == null) canv = GetComponent<CanvasGroup>();
    }
    private void OnValidate()
    {
        foreach(CharacterLoadInformation loadInfo in loadedCharacters)
        {
            loadInfo.Name = loadInfo.displayName;
        }
    }
}


[System.Serializable]
public class CharacterLoadInformation
{
    [HideInInspector] public string Name = "";
    public Sprite charPortrait;
    public Sprite charImage;
    public string displayName = "";
    public string squadBonusDetails = ""; //Replace this with an actual class in the future, that contains this display info
    public CharacterNameType characterID = CharacterNameType.None;
    public CharacterClassType charClass = CharacterClassType.Desert;
    public Conversation[] availableChats = new Conversation[0];
    public float xp = 0f;
    public int Level
    {
        get
        {
            for (int i = 0; i < SceneLoadManager.Instance.charLevelThresholds.Length; i++)
            {
                if (SceneLoadManager.Instance.charLevelThresholds[i] > xp) return i + 1;
            }
            return SceneLoadManager.Instance.charLevelThresholds.Length + 1;
        }
    }
    public float XPInLevel
    {
        get
        {
            int levelIndex = Mathf.Clamp(Level - 2, 0, SceneLoadManager.Instance.charLevelThresholds.Length);
            if (levelIndex == Level - 1) return xp;
            return xp - SceneLoadManager.Instance.charLevelThresholds[levelIndex];
        }
    }
    public float ProgressToNextLevel
    {
        get
        {
            if (Level > SceneLoadManager.Instance.charLevelThresholds.Length) return 1;
            return XPInLevel / (SceneLoadManager.Instance.charLevelThresholds[Level - 1] - (Level - 2 < 0 ? 0 : SceneLoadManager.Instance.charLevelThresholds[Level - 2]));
        }
    }
    public float attackDamage;
    public float attackSpeed;
    public float stamina;
    public float health;
    public float defence;
    public float moveSpeed;
    public float shield;
    public enum EncounterState { Hidden, Encountered, Recruited }
    public EncounterState encounterState;
    public MaskTypes heldMask
    {
        get
        {
            return SceneLoadManager.Instance.loadedMasks.Where(r => r.maskHolder == characterID).FirstOrDefault() != null ?
                SceneLoadManager.Instance.loadedMasks.Where(r => r.maskHolder == characterID).First().maskType : MaskTypes.None;
        }
    }
}

[System.Serializable]
public class MaskLoadInformation
{
    public string Name = "";
    public Sprite maskImage;
    public MaskTypes maskType = MaskTypes.Stage1;
    public bool collected = false;
    public float xp = 0f;
    public int Level
    {
        get
        {
            for (int i = 0; i < SceneLoadManager.Instance.maskLevelThresholds.Length; i++)
            {
                if (SceneLoadManager.Instance.maskLevelThresholds[i] > xp) return i + 1;
            }
            return SceneLoadManager.Instance.maskLevelThresholds.Length + 1;
        }
    }
    public MaskAbilityInformationClass[] abilities = new MaskAbilityInformationClass[3];

    public CharacterNameType maskHolder = CharacterNameType.None;
}

[System.Serializable]
public class MaskAbilityInformationClass
{
    public string Name = "";
    public Sprite abilityImage;
    [TextArea(0, 2)] public string description = "";
}

[System.Serializable]
public class Conversation
{
    public string Name = "Generic Conversation";
    public string[] lines = new string[0];

    public Conversation()
    {
        Name = "Generic Conversation";
        lines = new string[0];
    }
}