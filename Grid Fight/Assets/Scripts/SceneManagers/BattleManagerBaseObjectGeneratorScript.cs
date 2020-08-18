using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class BattleManagerBaseObjectGeneratorScript : MonoBehaviour
{

    public delegate void StartMatch();
    public event StartMatch StartMatchEvent;


    public static BattleManagerBaseObjectGeneratorScript Instance;
    bool loadFromGameScene = false;
    public float loadingTime = 5f;
    public string selectedStageID = "";
    [HideInInspector] public StageProfile stage;
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
        Instance = this;
        loadFromGameScene = SceneLoadManager.Instance != null;
        if (loadFromGameScene) return;
        StartCoroutine(ConfigureBattleScene(selectedStageID));
    }

    public IEnumerator ConfigureBattleScene(string stageName)
    {
        SelectStage(stageName);

        yield return SetupScene();
    }

    void StageStartAnalytics()
    {
        AnalyticsManager.Instance?.Track_LevelPhase(AnalyticsManager.PhaseEvent.Started);

        foreach (CharacterBaseInfoClass character in BattleInfoManagerScript.Instance.PlayerBattleInfo)
        {
            if (character == null || character.CharacterName == CharacterNameType.None) continue;

            AnalyticsManager.Instance?.Track_CharacterEvent(character.CharacterName, AnalyticsManager.CharEvent.Selected);
        }
    }

    void SelectStage(string stageName)
    {
        foreach (StageProfile stageP in stages)
        {
            if (stageP.ID == stageName)
            {
                stage = stageP;
                return;
            }
        }
        Debug.LogError("Stage with StageID " + stageName + " not found in Stages list");
    }

    private IEnumerator SetupScene()
    {
        InfoUIManager.Instance?.EnableLoadingScreen(!loadFromGameScene, false);
        yield return LevelLoadingSequence();
        yield return new WaitForSeconds(loadingTime);
        if (!loadFromGameScene) InfoUIManager.Instance?.EnableLoadingScreen(false, true);
    }

    public void ChangeStage(string newStageID, bool loadAsWell = true)
    {
        StartCoroutine(ChangeStageSequence(newStageID, loadAsWell));
    }

    private IEnumerator ChangeStageSequence(string newStageID, bool loadAsWell)
    {
        if (stage.ID == newStageID) yield break;

        SelectStage(newStageID);

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
        if(InputController.Instance == null)
        {
            Rewired = Instantiate(stage.Rewired);
        }
        else
        {
            Rewired = InputController.Instance.gameObject;
        }
        StageObjects.Add(Rewired);

        yield return InputController.Instance.Applet(2);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Character_Testing_Scene")
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
        else
        {
            //InputController.Instance.ResetEventSubscription();
        }
        yield return null;

        currentStageID = stage.ID;

        BaseEnvironment = Instantiate(stage.BaseEnvironment);
        StageObjects.Add(BaseEnvironment);

        yield return null;

      


        BattleManager = Instantiate(stage.BattleManager);
        StageObjects.Add(BattleManager);

        if (stage.EventManager != null)
        {
            EventManager = Instantiate(stage.EventManager);
            StageObjects.Add(EventManager);
        }

        if (stage.Wave != null)
        {
            Wave = Instantiate(stage.Wave);
            StageObjects.Add(Wave);
        }

        
        BattleInfoManager = Instantiate(stage.BattleInfoManager);
        StageObjects.Add(BattleInfoManager);

     /*   switch (BattleInfoManager.GetComponent<BattleInfoManagerScript>().MatchInfoType)
        {
            case MatchType.PvE:
                InputController.Instance.PlayersNumber = 1;
                InputController.Instance.SetChars();
                break;
            case MatchType.PvP:
                InputController.Instance.PlayersNumber = 2;
                InputController.Instance.SetChars();
                break;
            case MatchType.PPvE:
                InputController.Instance.PlayersNumber = 2;
                InputController.Instance.SetChars();
                break;
            case MatchType.PPvPP:
                InputController.Instance.PlayersNumber = 4;
                InputController.Instance.SetChars();
                break;
            case MatchType.PPPPvE:
                InputController.Instance.PlayersNumber = 4;
                InputController.Instance.SetChars();
                break;
            default:
                break;
        }*/

        Battle_UI = Instantiate(stage.UI_Battle);
        StageObjects.Add(Battle_UI);

        yield return null;

        yield return BattleManagerScript.Instance.InstanciateAllChar();
        if (Wave != null) yield return WaveManagerScript.Instance.WaveCharCreator();

        yield return null;

        AudioManager = Instantiate(stage.AudioManager);
        StageObjects.Add(AudioManager);

        StartMatchEvent?.Invoke();

        if (AudioManager.GetComponent<AudioManagerMk2>() != null && stage.StageAudioProfile != null)
        {
            if (stage.StageAudioProfile.music != null)
                AudioManager.GetComponent<AudioManagerMk2>().PlaySound(AudioSourceType.Music, stage.StageAudioProfile.music, AudioBus.Music, loop: true);
            if (stage.StageAudioProfile.ambience != null)
                AudioManager.GetComponent<AudioManagerMk2>().PlaySound(AudioSourceType.Ambience, stage.StageAudioProfile.ambience, AudioBus.Music, loop: true, fadeInDuration: 1f);
        }

        UserInputManager.Instance.StartUserInputManager();
        BattleManagerScript.Instance.SetupBattleState();

        StageStartAnalytics();
    }

}


