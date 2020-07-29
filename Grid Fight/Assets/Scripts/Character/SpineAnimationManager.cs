using Spine.Unity;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class SpineAnimationManager : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton skeleton;
    public BaseCharacter CharOwner;

    public MovementCurveType CurveType = MovementCurveType.Speed_Time;
    [HideInInspector]public AnimationMovementCurveClass Speed_Time_Curves;
    [HideInInspector]public AnimationMovementCurveClass Space_Time_Curves;


    public List<Transform> FiringPints = new List<Transform>();
    public string CurrentAnim;
    public float AnimationTransition = 0.1f;

    public bool Loop = false;


    //initialize all the spine element
    public void SetupSpineAnim()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            skeletonAnimation.enabled = true;
            SpineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
        }
    }

    public void SetAnim(CharacterAnimationStateType anim)
    {

        if (CurrentAnim == CharacterAnimationStateType.Death.ToString() && anim.ToString() != CharacterAnimationStateType.Idle.ToString())
        {
            return;
        }

        if (anim == CharacterAnimationStateType.Arriving || anim.ToString().Contains("Growing"))
        {
            SetAnim(anim, false, 0);
        }
        else
        {
            SetAnim(anim, anim == CharacterAnimationStateType.Death || anim == CharacterAnimationStateType.Idle ? true : false, AnimationTransition);
        }
    }

    public void SetAnim(CharacterAnimationStateType anim, bool loop, float transition)
    {
        if (anim == CharacterAnimationStateType.Atk1_IdleToAtk)
        {
            //Debug.Log("Arriving start");
        }
        Loop = loop;
        //Debug.Log(anim.ToString());

        SpineAnimationState.SetAnimation(0, anim.ToString(), loop).MixDuration = transition;
        //StartCoroutine(ClearAnim(transition));
        CurrentAnim = anim.ToString();
    }

    public void SetAnim(string anim, bool loop, float transition)
    {
        if (anim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
        {
            //Debug.Log("Arriving start");
        }
        Loop = loop;
        //Debug.Log(anim.ToString());

        SpineAnimationState.SetAnimation(0, anim, loop).MixDuration = transition;
        CurrentAnim = anim;
    }

   
    public float GetAnimTime()
    {
        return skeletonAnimation.state.GetCurrent(1).TrackTime;
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        if (skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()) != null)
        {
            return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
        }
        return 1;
    }

    public float GetAnimLenght(string anim)
    {
        if (skeletonAnimation.Skeleton.Data.FindAnimation(anim) != null)
        {
            return skeletonAnimation.Skeleton.Data.FindAnimation(anim).Duration;
        }
        return 1;
    }

    public void SetAnimationSpeed(float speed)
    {
        if (SpineAnimationState != null)
        {
            SpineAnimationState.Tracks.ForEach(r => {
                if (r != null)
                {
                    r.TimeScale = speed;
                }
            });
        }
    }

    public float GetAnimationSpeed()
    {
        if (SpineAnimationState != null)
        {
            if (SpineAnimationState.Tracks.Where(r => r != null).FirstOrDefault() != null)
            {
                return SpineAnimationState.Tracks.Where(r => r != null).FirstOrDefault().TimeScale;
            }
        }
        return 1f;
    }

    public void SetSkeletonOrderInLayer(int order)
    {
        GetComponent<MeshRenderer>().sortingOrder = order;
    }








    public bool changeanim = false;
    public float transition = 0.5f;
    public CharacterAnimationStateType NextAnim = CharacterAnimationStateType.Arriving;
    private void Update()
    {
        if (changeanim)
        {
            changeanim = false;
            SetAnim(NextAnim.ToString(), false, transition);
        }
    }

}

[System.Serializable]
public class CurrentAnimClass
{
    public CharacterAnimationStateType CurrentAnimation;
    public int CurrentTrack;

    public CurrentAnimClass()
    {
        CurrentTrack = 0;
    }
}

[System.Serializable]
public class AnimationMovementCurveClass
{
    public AnimationCurve UpMovement;
    public AnimationCurve DownMovement;
    public AnimationCurve BackwardMovement;
    public AnimationCurve ForwardMovement;
}