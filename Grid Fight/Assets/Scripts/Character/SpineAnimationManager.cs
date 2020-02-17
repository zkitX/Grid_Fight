using Spine.Unity;
using System.Collections;
using UnityEngine;

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
    public CharacterAnimationStateType CurrentAnim;
    public float AnimationTransition = 0.1f;

    private bool Loop = false;


    //initialize all the spine element
    public void SetupSpineAnim()
    {
        if(skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            skeletonAnimation.enabled = true;
            SpineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
            SpineAnimationState.Complete += SpineAnimationState_Complete;
            SpineAnimationState.Event += SpineAnimationState_Event;
            SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            SpineAnimationState.SetEmptyAnimation(1, 0);
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
            CharOwner.currentAttackPhase = CurrentAnim.ToString().Contains("Atk1") ? AttackPhasesType.Cast_Rapid : AttackPhasesType.Cast_Powerful;
            CharOwner.FireCastParticles();
        }
        else if (e.Data.Name.Contains("FireBulletParticle"))
        {
            CharOwner.currentAttackPhase = CurrentAnim.ToString().Contains("Atk1") ? AttackPhasesType.Bullet_Rapid : AttackPhasesType.Bullet_Powerful;
            CharOwner.CreateAttack();
           
            if (!CharOwner.VFXTestMode)
            {
                CharOwner.NextAttackLevel = CharacterLevelType.Novice;

            }
        }
    }

    //Method fired when an animation is complete
    private void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
       //Debug.Log(skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name + "   " + CurrentAnim.ToString());
        if (skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name == "<empty>")
        {
            return;
        }
        CharacterAnimationStateType completedAnim = (CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name);

        if (completedAnim == CharacterAnimationStateType.Arriving || completedAnim.ToString().Contains("Growing"))
        {
            CharOwner.IsSwapping = false;
            CharOwner.CharArrivedOnBattleField();
        }
        if (CurrentAnim == CharacterAnimationStateType.Reverse_Arriving)
        {
            CharOwner.IsSwapping = false;
            CharOwner.SetAttackReady(false);
        }

        if (CurrentAnim == CharacterAnimationStateType.Death)
        {
            return;
        }


        //Switch between character and minion unique attacking animation sequences
        if (CharOwner.UMS.CurrentAttackType == AttackType.Particles)
        {
            #region QuickAttack

            if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk && CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk)
            {
                ((CharacterType_Script)CharOwner).QuickAttack();
                return;
            }
            if (completedAnim == CharacterAnimationStateType.Atk1_Loop &&
             CurrentAnim == CharacterAnimationStateType.Atk1_Loop)
            {
                if (((CharacterType_Script)CharOwner).Atk1Queueing)
                {
                    ((CharacterType_Script)CharOwner).QuickAttack();
                }
                else
                {
                    SetAnim(CharacterAnimationStateType.Atk1_AtkToIdle);
                    SetAnimationSpeed(GetAnimLenght(CharacterAnimationStateType.Atk2_IdleToAtk) / CharOwner.CharInfo.SpeedStats.IdleToAtkDuration);
                }
                return;
            }

            #endregion
            #region ChargingAttack
            if (completedAnim == CharacterAnimationStateType.Atk2_IdleToAtk && CurrentAnim == CharacterAnimationStateType.Atk2_IdleToAtk)
            {
                ((CharacterType_Script)CharOwner).ChargingLoop();
                return;
            }
            #endregion
            if (completedAnim == CharacterAnimationStateType.Atk1_AtkToIdle || completedAnim == CharacterAnimationStateType.Atk2_AtkToIdle
                || completedAnim == CharacterAnimationStateType.Atk || completedAnim == CharacterAnimationStateType.Atk1)
            {
                ((CharacterType_Script)CharOwner).GetAttack(CharacterAnimationStateType.Atk);
                CharOwner.currentAttackPhase = AttackPhasesType.End;
            }
        }
        else if (CharOwner.UMS.CurrentAttackType == AttackType.Tile)
        {
            if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk && CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk)
            {
                SetAnim(CharacterAnimationStateType.Atk1_Charging, true, 0);
            }

            if(completedAnim == CharacterAnimationStateType.Atk1_Loop && CurrentAnim == CharacterAnimationStateType.Atk1_Loop)
            {
                //If they can still attack, keep them in the charging loop
                if(((MinionType_Script)CharOwner).shotsLeftInAttack > 0)
                {
                    SetAnim(CharacterAnimationStateType.Atk1_Charging, true, 0);
                }
                //otherwise revert them to the idle postion
                else
                {
                    SetAnim(CharacterAnimationStateType.Atk1_AtkToIdle);
                    CharOwner.currentAttackPhase = AttackPhasesType.End;
                  
                }
                return;
            }

            if (completedAnim == CharacterAnimationStateType.Atk1_AtkToIdle || completedAnim == CharacterAnimationStateType.Atk2_AtkToIdle
                || completedAnim == CharacterAnimationStateType.Atk || completedAnim == CharacterAnimationStateType.Atk1)
            {
                CharOwner.currentAttackPhase = AttackPhasesType.End;
                ((MinionType_Script)CharOwner).Attacking = false;
            }
        }

        if (completedAnim != CharacterAnimationStateType.Idle && !Loop)
        {
            SetAnimationSpeed(CharOwner.CharInfo.BaseSpeed);
            SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            SpineAnimationState.AddEmptyAnimation(1,AnimationTransition,0);
            CurrentAnim = CharacterAnimationStateType.Idle;
        }
    }

    public void SpecialAtkTest()
    {
        float timer = skeletonAnimation.state.GetCurrent(1).AnimationTime;
        SpineAnimationState.SetAnimation(1, CharacterAnimationStateType.Atk1_IdleToAtk.ToString(), false);
        skeletonAnimation.state.GetCurrent(1).AnimationStart = timer;
        CurrentAnim = CharacterAnimationStateType.Atk1_IdleToAtk;
    }

    public void SetAnim(CharacterAnimationStateType anim)
    {
        SetupSpineAnim();

        if(CurrentAnim == CharacterAnimationStateType.Death && anim != CharacterAnimationStateType.Idle)
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
        SetupSpineAnim();
        if(anim == CharacterAnimationStateType.Arriving)
        {
            //Debug.Log("Arriving start");
        }
        Loop = loop;
        SpineAnimationState.SetAnimation(1, anim.ToString(), loop).MixDuration = transition;
        if(transition > 0)
        {
            StartCoroutine(ClearAnim(transition));
        }
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
        SpineAnimationState.SetEmptyAnimation(0, 0);
    }

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

