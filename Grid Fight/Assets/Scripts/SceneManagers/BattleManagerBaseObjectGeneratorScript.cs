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
        while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "BattleScene")
        {
            yield return null;
        }
        Instantiate(BaseEnvironment);
        yield return null;
        Instantiate(BattleManager);
        Instantiate(UI_Battle);
        yield return BattleManagerScript.Instance.InstanciateAllChar();
        Instantiate(Wave);
        yield return null;
        Instantiate(FlowChart);
        yield return null;
    }
}
