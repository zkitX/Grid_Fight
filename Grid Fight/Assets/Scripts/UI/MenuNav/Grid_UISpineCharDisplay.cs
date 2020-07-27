using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System.Linq;

public class Grid_UISpineCharDisplay : MonoBehaviour
{
    SkeletonGraphic selectionDisplay = null;

    private void Awake()
    {
        selectionDisplay = GetComponentInChildren<SkeletonGraphic>();
    }

    public void DisplayChar(CharacterNameType charID, bool hidden = false)
    {
        DisplaySkeleton(charID != CharacterNameType.None ? SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == charID).FirstOrDefault().charSpine : null, hidden);
    }

    protected void DisplaySkeleton(SkeletonDataAsset characterSpine, bool hidden = false)
    {
        Color displayColor = !hidden ? new Color(1f, 1f, 1f, 1f) : new Color(0f, 0f, 0f, 1f);
        if (characterSpine == null)
        {
            displayColor = new Color(1f, 1f, 1f, 0f);
        }

        if (isActiveAndEnabled && selectionDisplay?.skeletonDataAsset != characterSpine)
        {
            if (SelectedDisplayer != null) StopCoroutine(SelectedDisplayer);
            SelectedDisplayer = ReloadSpineSkeletonData(characterSpine, displayColor);
            StartCoroutine(SelectedDisplayer);
        }
    }

    IEnumerator SelectedDisplayer = null;
    IEnumerator ReloadSpineSkeletonData(SkeletonDataAsset characterSpine, Color displayColor)
    {
        selectionDisplay.color = new Color(1f, 1f, 1f, 0f);

        selectionDisplay.skeletonDataAsset = characterSpine;

        selectionDisplay.Initialize(true);

        yield return new WaitForSecondsRealtime(0.1f);

        selectionDisplay.color = displayColor;
    }
}
