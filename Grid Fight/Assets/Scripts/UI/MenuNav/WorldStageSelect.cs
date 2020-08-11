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
        StageButton curBtn = stageBtns.Where(r => r.stage != null && r.stage.ID == Grid_UIBriefing.Instance.curStage.ID).FirstOrDefault();
        if (curBtn == null) return null;

        return curBtn.transform;
    }

    public void ShowUnfocusedStageButtons(bool state)
    {
        StageButton[] unfocused = stageBtns.Where(r => r.stage == null || r.stage.ID != Grid_UIBriefing.Instance.curStage.ID).ToArray();
        foreach (StageButton btn in unfocused)
        {
            btn.ShowButton(state);
        }
    }

    public void SelectLastFocusedButton()
    {
        StageButton curBtn = stageBtns.Where(r => r.stage != null && r.stage.ID == Grid_UIBriefing.Instance.curStage.ID).FirstOrDefault();
        if (curBtn == null) return;

        Grid_UINavigator.Instance.SelectButton(curBtn.GetComponent<Grid_UIButton>(), true);
    }

    public void RefreshStageButtons()
    {
        foreach(StageButton btn in stageBtns)
        {
            btn.ProcessLockState();
        }
    }

    public void ShowStageButtonsNames(bool state)
    {
        foreach (StageButton btn in stageBtns)
        {
            btn.ShowStageText(state);
        }
    }
}
