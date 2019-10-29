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
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    public IEnumerator LoadNewSceneWithLoading(string nextScene, string prevScene)
    {
        MainCanvasGroup.alpha = 1;
        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(prevScene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
       // asyncLoad.allowSceneActivation = false;
        asyncLoad.completed += AsyncLoad_completed;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            LoadingBar.fillAmount = asyncLoad.progress;
            yield return new WaitForFixedUpdate();
        }
    }

    private void AsyncLoad_completed(AsyncOperation obj)
    {
        //obj.allowSceneActivation = true;
    }
}
