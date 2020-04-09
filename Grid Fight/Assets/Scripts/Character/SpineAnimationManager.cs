﻿using Spine.Unity;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;

public class SpineAnimationManager : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton skeleton;
    public BaseCharacter CharOwner;

    public AnimationCurve UpMovementSpeed;
    public AnimationCurve DownMovementSpeed;
    public AnimationCurve LeftMovementSpeed;
    public AnimationCurve RightMovementSpeed;

    public Transform FiringPoint;
    public Transform SpecialFiringPoint;
    public string CurrentAnim;
    public float AnimationTransition = 0.1f;

    public bool Loop = false;


    //initialize all the spine element
    public void SetupSpineAnim()
    {
        if(skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            skeletonAnimation.enabled = true;
            SpineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
            //SpineAnimationState.Complete += SpineAnimationState_Complete;
            if (SpineAnimationState != null)
            {
                SpineAnimationState.Event += SpineAnimationState_Event;
            }
            //SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            //SpineAnimationState.SetEmptyAnimation(1, 0);
        }
    }

    //Used to get spine event
    private void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name.Contains("StopDefending"))
        {
            SetAnimationSpeed(0);
        }
        else if (e.Data.Name.Contains("FireArrivingParticle"))
        {
            CharOwner.ArrivingEvent();
        }
        else if (e.Data.Name.Contains("FireCastParticle"))
        {
            CharOwner.currentAttackPhase = CurrentAnim.Contains("Atk1") ? AttackPhasesType.Cast_Rapid : AttackPhasesType.Cast_Powerful;
            CharOwner.FireCastParticles(CurrentAnim.Contains("Atk1") ? CharacterLevelType.Novice : CharacterLevelType.Godness);
        }
        else if (e.Data.Name.Contains("FireBulletParticle"))
        {
            CharOwner.currentAttackPhase = CurrentAnim.Contains("Atk1") ? AttackPhasesType.Bullet_Rapid : AttackPhasesType.Bullet_Powerful;
            CharOwner.CreateParticleAttack();
           
            if (!CharOwner.VFXTestMode)
            {
                CharOwner.NextAttackLevel = CharacterLevelType.Novice;

            }
        }
        else if(e.Data.Name.Contains("FireTileAttack"))
        {
            CharOwner.currentAttackPhase = CurrentAnim.Contains("Atk1") ? AttackPhasesType.Bullet_Rapid : AttackPhasesType.Bullet_Powerful;
            //CharOwner.CreateTileAttack();

            if (!CharOwner.VFXTestMode)
            {
                CharOwner.NextAttackLevel = CharacterLevelType.Novice;

            }
        }
    }

    public void SetAnim(CharacterAnimationStateType anim)
    {

        if(CurrentAnim == CharacterAnimationStateType.Death.ToString() && anim.ToString() != CharacterAnimationStateType.Idle.ToString())
        {
            return;
        }

        if(anim == CharacterAnimationStateType.Arriving || anim.ToString().Contains("Growing"))
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
         if(anim == CharacterAnimationStateType.Atk1_IdleToAtk)
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
        //StartCoroutine(ClearAnim(transition));
        CurrentAnim = anim;
    }

    private IEnumerator ClearAnim(float transition)
     {
         float timer = 0;
         while (timer <= transition)
         {
             yield return new WaitForFixedUpdate();
             while (!CharOwner.VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause))
             {
                 yield return new WaitForEndOfFrame();
             }
             timer += Time.fixedDeltaTime;
         }
         Debug.Log("Clear");
         SpineAnimationState.SetEmptyAnimation(0, 0);
     }


   /* public void SetAnim(CharacterAnimationStateType anim, bool loop, float transition)
    {

        Debug.Log(anim.ToString());
        SetupSpineAnim();
        Loop = loop;
        SpineAnimationState.SetAnimation(1, anim.ToString(), loop).MixDuration = transition;
        StartCoroutine(test(transition));
        CurrentAnim = anim;
    }

    private IEnumerator test(float asdaf)
    {
        float timer = 0;
        while (timer <= asdaf)
        {
            yield return new WaitForFixedUpdate();
            while (!CharOwner.VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }
        SpineAnimationState.SetEmptyAnimation(0, 0);
    }*/



    public float GetAnimTime()
    {
        return skeletonAnimation.state.GetCurrent(1).TrackTime;
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        if(skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()) != null)
        {
            return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
        }
        return 1;
    }
    public bool changeanim = false;
    public float transition = 0.5f;
    private void Update()
    {
        if(changeanim)
        {
            changeanim = false;
            SetAnim(CurrentAnim, false, transition);
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        if(SpineAnimationState != null)
        {
            SpineAnimationState.Tracks.ForEach(r => {
                if (r != null)
                {
                    r.TimeScale = speed;
                }
            });
        }
    }

    public void SetSkeletonOrderInLayer(int order)
    {
        GetComponent<MeshRenderer>().sortingOrder = order;
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

