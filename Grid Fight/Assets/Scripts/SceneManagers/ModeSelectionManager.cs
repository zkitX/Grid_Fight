using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelectionManager : MonoBehaviour
{
    public void SetMode(int v)
    {
        
        BattleInfoManagerScript.Instance.MatchInfoType = (MatchType)v;
        InputController.Instance.Applet((MatchType)v == MatchType.PvE ? 1 : (MatchType)v == MatchType.PPvPP ? 4 : 2);
    }

    public void GoToBattle()
    {
        SceneManager.LoadScene(1);
    }
}
