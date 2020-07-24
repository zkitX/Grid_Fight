using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Grid_UIBriefing : MonoBehaviour
{
    public StageProfile curStage = null;
    public static Grid_UIBriefing Instance;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Grid_UISpineCharDisplay[] squadSpines;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupBriefing(StageProfile stageInfo)
    {
        curStage = stageInfo;
        title.text = stageInfo.Name;
        description.text = stageInfo.Description;
        SceneLoadManager.Instance.stagePrimedToLoad = stageInfo;
        UpdateSquadImages();
    }

    public void UpdateSquadImages()
    {
        for (int i = 0; i < squadSpines.Length; i++)
        {
            squadSpines[i].DisplayChar(SceneLoadManager.Instance.squad[i] == null ? CharacterNameType.None : SceneLoadManager.Instance.squad[i].characterID, false);
        }
    }
}
