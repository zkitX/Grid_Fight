using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagerBaseObjectGeneratorScript : MonoBehaviour
{

    public GameObject Rewired;
    public GameObject BAttleInfoManager;


    public GameObject BattleManager;
    public GameObject BaseEnvironment;

    public GameObject UI_Battle;
    public GameObject FlowChart;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LevelLoader(LoaderManagerScript.Instance != null ? 1 : 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LevelLoader(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

       // Instantiate(Rewired);
       // Instantiate(BAttleInfoManager);

        yield return new WaitForSecondsRealtime(duration);

        Instantiate(BaseEnvironment);

        yield return new WaitForSecondsRealtime(duration);

        GameObject battleManager = Instantiate(BattleManager);
        StartCoroutine(battleManager.GetComponent<BattleManagerScript>().InstanciateAllChar(duration == 0 ? 0 : 0.2f));
        yield return new WaitForSecondsRealtime(duration);

        Instantiate(UI_Battle);

        yield return new WaitForSecondsRealtime(duration);

        
        if(LoaderManagerScript.Instance != null)
        {
            LoaderManagerScript.Instance.MainCanvasGroup.alpha = 0;
        }
        yield return new WaitForSeconds(0.2f);
        Instantiate(FlowChart);
        yield return new WaitForSeconds(0.2f);
        BattleManagerScript.Instance.CurrentBattleState = BattleState.Intro;
    }
}
