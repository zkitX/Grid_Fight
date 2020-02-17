using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerBaseObjectGeneratorScript : MonoBehaviour
{

    public GameObject Rewired;
    public GameObject BattleInfoManager;


    public GameObject BattleManager;
    public GameObject BaseEnvironment;

    public GameObject UI_Battle;
    public GameObject FlowChart;
    public GameObject Wave;
    public GameObject bosstest;

    [Space(20)]
    [Header("Defaults")]
    [SerializeField] protected GameObject defaultWave;
    [SerializeField] protected GameObject defaultFlowChart;
    [SerializeField] protected GameObject defaultBattleInfo;
    [SerializeField] protected StageEventTriggersProfile defaultEventProfile;

    public bool usingFungus = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetupScene());
    }

    private IEnumerator SetupScene()
    {
        yield return InputController.Instance.Applet(2);
        StartCoroutine(LevelLoader());
    }

    private IEnumerator LevelLoader()
    {
        while (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("BattleScene"))
        {
            yield return null;
        }
        Instantiate(BaseEnvironment);
        yield return null;

        if(!usingFungus) BattleInfoManager = defaultBattleInfo;
        Instantiate(BattleInfoManager);

        BattleManagerScript bms = Instantiate(BattleManager).GetComponent<BattleManagerScript>();
        if(!usingFungus)bms.gameObject.GetComponent<EventManager>().stageEventTriggersProfile = defaultEventProfile;
        bms.usingFungus = usingFungus;

        Instantiate(UI_Battle);
        yield return BattleManagerScript.Instance.InstanciateAllChar();
        if(!usingFungus) Wave = defaultWave;
        Instantiate(Wave);
        yield return WaveManagerScript.Instance.WaveCharCreator();
        if (!usingFungus) FlowChart = defaultFlowChart;
        Instantiate(FlowChart);
        yield return null;

        UserInputManager.Instance.StartUserInputManager();
        bms.SetupBattleState();
    }
}
