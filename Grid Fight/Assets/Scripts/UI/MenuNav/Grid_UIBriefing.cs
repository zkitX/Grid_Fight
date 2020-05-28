using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Grid_UIBriefing : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;


    public void SetupBriefing(StageProfile stageInfo)
    {
        title.text = stageInfo.Name;
        description.text = stageInfo.Description;
        SceneLoadManager.Instance.stagePrimedToLoad = stageInfo;
    }
}
