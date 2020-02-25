using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPAX : MonoBehaviour
{

    public bool isloading = false;
    public bool ShowScene = false;
    public GameObject rewired;
    public List<Animator> Buttons = new List<Animator>();

    private Animator currentSelected;
    public float TimeOffset = 0;
    public float CoolDown = 0.5f;
    private int selectedButton = 0;

    public Animator BlackCoverAnim;

    public float TimeToWaitBeforeInputEnabled = 2;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("StartInput", TimeToWaitBeforeInputEnabled);
    }


    private void StartInput()
    {
        SelectButton();
        InputController.Instance.ButtonADownEvent += Instance_ButtonADownEvent;
        InputController.Instance.LeftJoystickUsedEvent += Instance_LeftJoystickUsedEvent;
    }

    private void Instance_LeftJoystickUsedEvent(int player, InputDirection dir)
    {
        if(Time.time > TimeOffset + CoolDown)
        {
            Debug.Log(dir.ToString());
            switch (dir)
            {
                case InputDirection.Up:
                    selectedButton--;
                    TimeOffset = Time.time;
                    break;
                case InputDirection.Down:
                    selectedButton++;
                    TimeOffset = Time.time;
                    break;
            }
            selectedButton = selectedButton >= Buttons.Count ? Buttons.Count - 1 : selectedButton < 0 ? 0 : selectedButton;
            SelectButton();
        }
    }


    private void SelectButton()
    {
        if(currentSelected != null)
        {
            currentSelected.SetBool("Active", false);
        }
        currentSelected = Buttons[selectedButton];
        currentSelected.SetBool("Active", true);
    }


    private void Instance_ButtonADownEvent(int player)
    {
        GoToBattleScene(selectedButton == 0 ? "BattleScene-00-PAX" : selectedButton == 1 ? "BattleScene-04-PAX" : "SpeedRun-00-PAX");
    }


    public void GoToBattleScene(string sceneName)
    {
        StartCoroutine(LoadYourAsyncScene(sceneName));
        InputController.Instance.ButtonADownEvent -= Instance_ButtonADownEvent;
        InputController.Instance.LeftJoystickUsedEvent -= Instance_LeftJoystickUsedEvent;
    }


    IEnumerator LoadYourAsyncScene(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        BlackCoverAnim.SetBool("InOut", false);
        Invoke("ShowBattleScene", 1);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone && !ShowScene)
        {
            yield return null;
        }


        yield return new WaitForSecondsRealtime(0.5f);
        asyncLoad.allowSceneActivation = true;
        yield return new WaitForSecondsRealtime(0.5f);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("PaxMainMenu"));
    }

    public void ShowBattleScene()
    {
        ShowScene = true;
        Destroy(rewired);
    }

}
