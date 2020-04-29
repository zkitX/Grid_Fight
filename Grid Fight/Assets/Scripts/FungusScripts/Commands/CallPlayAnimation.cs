using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Play Animation",
                "Trigger the animation sequence of a specified animation within the denoted character")]
[AddComponentMenu("")]
public class CallPlayAnimation : Command
{
    public CharacterNameType characterID;
    public AnimationDetailsClass[] animDetails;
    public bool holdForCompletedAnimation = true;

    IEnumerator animate()
    {
        BaseCharacter character = BattleManagerScript.Instance.GetActiveCharNamed(characterID);

        foreach (AnimationDetailsClass animDetail in animDetails)
        {
            yield return character.PuppetAnimation(animDetail.animClipToPlay, animDetail.plays, animDetail.pauseOnLastFrame, animDetail.animationSpeed);
        }

        if (holdForCompletedAnimation) Continue();
    }

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(animate());
        if (!holdForCompletedAnimation) Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    public override void OnValidate()
    {
        foreach (AnimationDetailsClass animDetail in animDetails)
        {
            if (animDetail.animFromPreset) animDetail.animClipToPlay = animDetail.animClipPreset.ToString();
            if (animDetail.plays < 1) animDetail.plays = 1;
            animDetail.Name = "Play " + animDetail.animClipToPlay + " " + animDetail.plays + (animDetail.plays == 1 ? " time" : " times");
        }
        base.OnValidate();
    }
    #endregion
}

[System.Serializable]
public class AnimationDetailsClass
{
    [HideInInspector] public string Name = "";
    public bool animFromPreset = true;
    [ConditionalField("animFromPreset", true)] public string animClipToPlay = "";
    [ConditionalField("animFromPreset")] public CharacterAnimationStateType animClipPreset = CharacterAnimationStateType.Death;
    public int plays = 1;
    [Range(0.001f, 10f)] public float animationSpeed = 1f;
    public bool pauseOnLastFrame = false;

    public AnimationDetailsClass()
    {
        animFromPreset = true;
        plays = 0;
        pauseOnLastFrame = false;
    }
}
