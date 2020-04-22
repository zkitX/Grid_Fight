using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class BattleManagerBaseObjectGeneratorScript : MonoBehaviour
{
    public float loadingTime = 5f;
    public string selectedStageID = "";
    protected StageProfile stage;
    public StageProfile[] stages;
    protected string currentStageID;
    protected List<GameObject> StageObjects = new List<GameObject>();
    protected GameObject Rewired;
    protected GameObject AudioManager;
    protected GameObject BattleInfoManager;
    protected GameObject BattleManager;
    protected GameObject BaseEnvironment;
    protected GameObject Battle_UI;
    protected GameObject EventManager;
    protected GameObject Wave;

    private void Start()
    {
        SelectStage();
        LoadStage();
    }

    void SelectStage()
    {
        foreach(StageProfile stageP in stages)
        {
            if (stageP.ID == selectedStageID)
            {
                stage = stageP;
                return;
            }
        }
        Debug.LogError("Stage with StageID " + selectedStageID + " not found in Stages list");
    }

    public void LoadStage()
    {
        StartCoroutine(SetupScene());
    }

    private IEnumerator SetupScene()
    {
        InfoUIManager.Instance.EnableLoadingScreen(true, false);
        StartCoroutine(LevelLoadingSequence());
        yield return new WaitForSeconds(loadingTime);
        InfoUIManager.Instance.EnableLoadingScreen(false, true);
    }

    public void ChangeStage(string newStageID, bool loadAsWell = true)
    {
        StartCoroutine(ChangeStageSequence(newStageID, loadAsWell));
    }

    private IEnumerator ChangeStageSequence(string newStageID, bool loadAsWell)
    {
        if (stage.ID == newStageID) yield break;

        selectedStageID = newStageID;
        SelectStage();

        if (!loadAsWell) yield break;

        yield return DeloadSceneSequence();
        yield return LevelLoadingSequence();
    }

    private IEnumerator DeloadSceneSequence()
    {
        foreach (GameObject stageObject in StageObjects) Destroy(stageObject);
        StageObjects.Clear();
        yield return null;
    }

    private IEnumerator LevelLoadingSequence()
    {
        Rewired = Instantiate(stage.Rewired);
        StageObjects.Add(Rewired);

        yield return InputController.Instance.Applet(2);

        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Character_Testing_Scene")
        {
            while (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("BattleScene"))
            {
                yield return null;
            }
        }

        if (Rewired == null)
        {
           Rewired = Instantiate(stage.Rewired);
           StageObjects.Add(Rewired);
        }
        yield return null;

        currentStageID = stage.ID;

        BaseEnvironment = Instantiate(stage.BaseEnvironment);
        StageObjects.Add(BaseEnvironment);

        yield return null;

        BattleManager = Instantiate(stage.BattleManager);
        StageObjects.Add(BattleManager);

        if (stage.Wave != null)
        {
            Wave = Instantiate(stage.Wave);
            StageObjects.Add(Wave);
        }

        if (stage.EventManager != null)
        {
            EventManager = Instantiate(stage.EventManager);
            StageObjects.Add(EventManager);
        }

        BattleInfoManager = Instantiate(stage.BattleInfoManager);
        StageObjects.Add(BattleInfoManager);

        Battle_UI = Instantiate(stage.UI_Battle);
        StageObjects.Add(Battle_UI);

        yield return BattleManagerScript.Instance.InstanciateAllChar();
        if (Wave != null) yield return WaveManagerScript.Instance.WaveCharCreator();

        yield return null;

        AudioManager = Instantiate(stage.AudioManager);
        StageObjects.Add(AudioManager);

        yield return null;

        if (AudioManager.GetComponent<AudioManagerMk2>() != null && stage.StageAudioProfile != null)
        {
            if(stage.StageAudioProfile.music != null)
                AudioManager.GetComponent<AudioManagerMk2>().PlaySound(AudioSourceType.Music, stage.StageAudioProfile.music, AudioBus.Music, loop: true);
            if(stage.StageAudioProfile.ambience != null)
                AudioManager.GetComponent<AudioManagerMk2>().PlaySound(AudioSourceType.Ambience, stage.StageAudioProfile.ambience, AudioBus.Music, loop: true);
        }   

        UserInputManager.Instance.StartUserInputManager();
        BattleManagerScript.Instance.SetupBattleState();
    }

}


