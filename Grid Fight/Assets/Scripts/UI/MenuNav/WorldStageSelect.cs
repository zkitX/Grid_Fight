using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class WorldStageSelect : MonoBehaviour
{
    public static WorldStageSelect Instance;

    protected StageButton[] stageBtns;

    private void Awake()
    {
        Instance = this;
        stageBtns = GetComponentsInChildren<StageButton>();
    }

    public Transform GetCurrentFocusStage()
    {
        StageButton curBtn = stageBtns.Where(r => r.stage.ID == Grid_UIBriefing.Instance.curStage.ID).FirstOrDefault();
        if (curBtn == null) return null;
        return curBtn.transform;
    }

    public void RefreshStageButtons()
    {
        foreach(StageButton btn in stageBtns)
        {
            btn.ProcessLockState();
        }
    }
}
