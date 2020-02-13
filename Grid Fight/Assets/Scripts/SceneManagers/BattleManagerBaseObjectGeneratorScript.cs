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
    [SerializeField] protected GameObject defaultFlowChart;
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
        BattleManagerScript bms = Instantiate(BattleManager).GetComponent<BattleManagerScript>();
        bms.gameObject.GetComponent<EventManager>().stageEventTriggersProfile = defaultEventProfile;
        bms.usingFungus = usingFungus;

        Instantiate(UI_Battle);
        yield return BattleManagerScript.Instance.InstanciateAllChar();
        Instantiate(Wave);
        yield return WaveManagerScript.Instance.WaveCharCreator();
        if (!usingFungus) FlowChart = defaultFlowChart;
        Instantiate(FlowChart);
        yield return null;

        UserInputManager.Instance.StartUserInputManager();
    }
}
