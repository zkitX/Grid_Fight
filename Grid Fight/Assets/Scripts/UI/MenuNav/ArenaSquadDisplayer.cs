using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using Spine;

public class ArenaSquadDisplayer : MonoBehaviour
{
    public int squadIndex = 1;

    public SkeletonGraphic[] squadChar = new SkeletonGraphic[4];

    public Image[] colorImages = new Image[0];
    public TextMeshProUGUI[] colorTexts = new TextMeshProUGUI[0];

    public void RefreshColors()
    {
        Color color = SceneLoadManager.Instance.teamsColor[squadIndex - 1];
        foreach (Image img in colorImages)
        {
            img.color = color;
        }
        foreach (TextMeshProUGUI text in colorTexts)
        {
            text.color = color;
        }
    }

    public void RefreshDisplay()
    {
        if (SelectedDisplayer != null) StopCoroutine(SelectedDisplayer);
        SelectedDisplayer = ReloadSpineSkeletonData();
        StartCoroutine(SelectedDisplayer);

        RefreshColors();
    }

    IEnumerator SelectedDisplayer = null;
    IEnumerator ReloadSpineSkeletonData()
    {
        Dictionary<int, CharacterLoadInformation> loadout = squadIndex == 1 ? SceneLoadManager.Instance.arenaLoadoutInfo.SquadT1 : squadIndex == 2 ? SceneLoadManager.Instance.arenaLoadoutInfo.SquadT2 : null;
        if (loadout == null) Debug.LogError("Squad Index does not exist");

        for (int i = 0; i < loadout.Count; i++)
        {
            squadChar[i].color = Color.clear;

            squadChar[i].skeletonDataAsset = loadout[i].characterID != CharacterNameType.None ? loadout[i].charSpine : null;

            squadChar[i].Initialize(true);
        }

        yield return new WaitForSecondsRealtime(0.1f);

        for (int i = 0; i < squadChar.Length; i++)
        {
            squadChar[i].color = loadout[i].characterID != CharacterNameType.None ? Color.white : Color.clear;
        }
    }
}
