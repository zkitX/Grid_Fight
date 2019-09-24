using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpineAnimationManager : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    public CharacterAnimationStateType BaseAnim;
    public CharacterAnimationStateType MixAnim;
    public CharacterBase CharOwner;

    public AnimationCurve UpMovementSpeed;
    public AnimationCurve DownMovementSpeed;
    public AnimationCurve LeftMovementSpeed;
    public AnimationCurve RightMovementSpeed;

    public Transform FiringPoint;

    private void SetupSpineAnim()
    {
        if(skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            spineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
            spineAnimationState.Complete += SpineAnimationState_Complete;
            spineAnimationState.Event += SpineAnimationState_Event;
        }
    }

    private void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if(e.Data.Name == "FireParticles")
        {

        }
    }

    private void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        //Debug.Log("Complete" + "   "  + skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name + "     " + trackEntry.TrackIndex);

        if(skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name == "<empty>")
        {
            return;
        }

        switch ((CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name))
        {
            case CharacterAnimationStateType.NoMesh:
                break;
            case CharacterAnimationStateType.Idle:
                break;
            case CharacterAnimationStateType.Atk:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Atk1:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Buff:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Debuff:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Gettinghit:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Defending:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Paralized:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Arriving:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashRight:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashLeft:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashDown:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashUp:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Selection:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.PowerUp:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Speaking:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Victory:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Defeat:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Death:
                spineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetAnim(BaseAnim, true);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SetMixAnim(MixAnim, 0.1f, false);
        }
    }

    public void SetAnim(CharacterAnimationStateType anim, bool loop)
    {
        SetupSpineAnim();
        spineAnimationState.SetAnimation(0, anim.ToString(), loop);
    }

    public void SetMixAnim(CharacterAnimationStateType anim, float duration, bool loop)
    {
        SetupSpineAnim();
        spineAnimationState.SetEmptyAnimation(1, 0);
        spineAnimationState.AddAnimation(1, anim.ToString(), loop, 0).MixDuration = duration;
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
    }

    public void SetAnimationSpeed(float speed)
    {
        spineAnimationState.Tracks.ForEach(r => r.TimeScale = speed);
    }

}
