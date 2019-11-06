using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoaderManagerScript : MonoBehaviour
{

    public static LoaderManagerScript Instance;
    public Image LoadingBar;
    public CanvasGroup MainCanvasGroup;
    public List<CharacterBaseInfoClass> PlayerBattleInfo = new List<CharacterBaseInfoClass>();
    public MatchType MatchInfoType;
    private void Awake()
    {
        Instance = this;
    }

    public void LoadNewSceneWithLoading(string nextScene, string prevScene)
    {
        StartCoroutine(LoadNewSceneWithLoading_co(nextScene, prevScene));
    }
    // Start is called before the first frame update
    public IEnumerator LoadNewSceneWithLoading_co(string nextScene, string prevScene)
    {
        MainCanvasGroup.alpha = 1;

        if(nextScene == "BattleScene")
        {

        }

        SceneManager.UnloadSceneAsync(prevScene);
        //Begin to load the Scene you specify
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        //Don't let the Scene activate until you allow it to
        asyncLoad.allowSceneActivation = false;
        //When the load is still in progress, output the Text and progress bar
        while (!asyncLoad.isDone)
        {
            //Output the current progress
            LoadingBar.fillAmount = asyncLoad.progress;

            // Check if the load has finished
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return new WaitForEndOfFrame();
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
    }


}
