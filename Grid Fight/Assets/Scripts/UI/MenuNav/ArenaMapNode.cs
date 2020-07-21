using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaMapNode : MonoBehaviour
{
    [HideInInspector] public StageLoadInformation stageProfile;
    public Image thumbnail;
    public Image lockIcon;
    public Animation anim;

    public void SetupStageInfo(StageLoadInformation stage, Vector2 dimentions)
    {
        stageProfile = stage;

        thumbnail.rectTransform.sizeDelta = dimentions;

        thumbnail.sprite = stage.stageProfile.Thumbnail;
        thumbnail.color = stage.lockState == StageUnlockType.locked ? new Color(0.3f, 0.3f, 0.3f, 1f) : Color.white;
        lockIcon.color = stage.lockState == StageUnlockType.locked ? Color.white : Color.clear;

        anim = GetComponent<Animation>();
    }

    public void SelectAction()
    {
        if(anim.isPlaying) anim.Stop();
        anim.clip = anim.GetClip("ArenaMapNode_Select");
        anim.Play();
    }

    public void DeselectAction()
    {
        if (anim.isPlaying) anim.Stop();
        anim.clip = anim.GetClip("ArenaMapNode_Deselect");
        anim.Play();
    }
}
