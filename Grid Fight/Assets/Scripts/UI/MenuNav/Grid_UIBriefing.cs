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
    public Image[] squadImages;

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
        for (int i = 0; i < squadImages.Length; i++)
        {
            squadImages[i].color = Color.white;
            squadImages[i].sprite = SceneLoadManager.Instance.squad[i].charImage;


            if (squadImages[i].sprite == null)
            {
                squadImages[i].color = new Color(1f,1f,1f,0f);
            }
        }
    }
}
