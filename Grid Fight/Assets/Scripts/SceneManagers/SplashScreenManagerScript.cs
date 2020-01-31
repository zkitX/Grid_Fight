using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManagerScript : MonoBehaviour
{

    public static SplashScreenManagerScript Instance;
    public bool isloading = false;
    public bool ShowScene = false;
    public Animator Anim;
    public AudioSource AudioS;
    public AudioClip ButtonPressed;
    public AudioClip PressStart;
    public GameObject rewired;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_SWITCH && !UNITY_EDITOR
        PlayerPrefsSwitch.PlayerPrefsSwitch.Init();
      //  Invoke("GoToMainMenu", 2);
#endif
        InputController.Instance.ButtonPlusUpEvent += Instance_ButtonPlusUpEvent;

        Invoke("StartLoadingBattleScene", 1);
    }

    private void Instance_ButtonPlusUpEvent(int player)
    {
        if(!isloading)
        {
            AudioS.PlayOneShot(ButtonPressed);
            AudioS.clip = PressStart;
            AudioS.Play();
            isloading = true;
            Anim.SetBool("FadeOutIn", true);
        }
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone && !ShowScene)
        {
            yield return null;
        }
        InputController.Instance.ButtonPlusUpEvent -= Instance_ButtonPlusUpEvent;
       
        asyncLoad.allowSceneActivation = true;

        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("SplashScreenDemo"));
    }


    public void StartLoadingBattleScene()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    public void ShowBattleScene()
    {
        ShowScene = true;
        Destroy(rewired);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
