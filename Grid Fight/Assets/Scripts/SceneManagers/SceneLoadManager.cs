using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasRenderer))]
public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance;
    public GameObject NavigatorPrefab = null;
    protected string startingSceneID = "";
    [SerializeField] protected const float loadingFadeTime = 2f;
    public CanvasGroup canv = null;
    protected bool loadingScene = false;
    [HideInInspector] public StageProfile stagePrimedToLoad = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
        DontDestroyOnLoad(this);

        startingSceneID = SceneManager.GetActiveScene().name;
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
        if(Grid_UINavigator.Instance == null) Instantiate(NavigatorPrefab).name = NavigatorPrefab.name;
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

        foreach(string scene in scenesToLoad)
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
}
