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
        if (e.Data.Name.Contains("FireArrivingParticle"))
        {
            CharOwner.ArrivingEvent();
        }
        if (e.Data.Name.Contains("FireCastParticle"))
        {
            CharOwner.currentAttackPhase = AttackPhasesType.Cast;
            CharOwner.FireCastParticles();
        }
        if (e.Data.Name.Contains("FireBulletParticle"))
        {
            CharOwner.currentAttackPhase = AttackPhasesType.Bullet;
            CharOwner.CharInfo.Stamina -= CharOwner.CharInfo.StaminaStats.Stamina_Cost_Atk;
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
            Debug.Log("Arriving complete");
            CharOwner.SetAttackReady(true);
            CharOwner.StartAttakCo();
        }
        if (CurrentAnim == CharacterAnimationStateType.Reverse_Arriving)
        {
            CharOwner.SetAttackReady(false);
        }

        if (CurrentAnim == CharacterAnimationStateType.Death)
        {
            return;
        }


        //Switch between character and minion unique attacking animation sequences
        if (CharOwner.UMS.CurrentAttackType == AttackType.Particles)
        {
            if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk && CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk)
            {
                ((CharacterType_Script)CharOwner).SpecialAttackLoop();
                ((CharacterType_Script)CharOwner).AtkCharging = true;
                return;
            }

            if (completedAnim == CharacterAnimationStateType.Atk2_IdleToAtk && CurrentAnim == CharacterAnimationStateType.Atk2_IdleToAtk)
            {
                ((CharacterType_Script)CharOwner).SecondSpecialAttackChargingLoop();
                return;
            }

            if (completedAnim == CharacterAnimationStateType.Atk1_Loop &&
               CurrentAnim == CharacterAnimationStateType.Atk1_Loop)
            {
                if (((CharacterType_Script)CharOwner).Atk1Queueing)
                {
                    ((CharacterType_Script)CharOwner).SpecialAttackLoop();
                }
                else
                {
                    /* if (((CharacterType_Script)CharOwner).AtkCharging)
                     {

                         ((CharacterType_Script)CharOwner).SecondSpecialAttackStarting();
                     }
                     else
                     {
                         SetAnim(CharacterAnimationStateType.Atk1_AtkToIdle);
                     }*/

                    SetAnim(CharacterAnimationStateType.Atk1_AtkToIdle);

                }
                return;
            }
         
            if (completedAnim == CharacterAnimationStateType.Atk1_AtkToIdle || completedAnim == CharacterAnimationStateType.Atk2_AtkToIdle
                || completedAnim == CharacterAnimationStateType.Atk || completedAnim == CharacterAnimationStateType.Atk1)
            {
                if(((CharacterType_Script)CharOwner).AtkCharging)
                {
                    SetAnim(CharacterAnimationStateType.Atk2_IdleToAtk);
                    return;
                }
                else
                {
                    ((CharacterType_Script)CharOwner).GetAttack(CharacterAnimationStateType.Atk);
                    CharOwner.currentAttackPhase = AttackPhasesType.End;
                }
               
            }
        }
        else if (CharOwner.UMS.CurrentAttackType == AttackType.Tile)
        {
            if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk && CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk)
            {
                SetAnim(CharacterAnimationStateType.Atk1_Charging, true, 0f);
            }

            if(completedAnim == CharacterAnimationStateType.Atk1_Loop && CurrentAnim == CharacterAnimationStateType.Atk1_Loop)
            {
                //If they can still attack, keep them in the charging loop
                if(((MinionType_Script)CharOwner).shotsLeftInAttack > 0)
                {
                    SetAnim(CharacterAnimationStateType.Atk1_Charging);
                }
                //otherwise revert them to the idle postion
                else
                {
                    SetAnim(CharacterAnimationStateType.Atk1_AtkToIdle);
                }
            }

            if (completedAnim == CharacterAnimationStateType.Atk1_AtkToIdle || completedAnim == CharacterAnimationStateType.Atk2_AtkToIdle
                || completedAnim == CharacterAnimationStateType.Atk || completedAnim == CharacterAnimationStateType.Atk1)
            {
                CharOwner.currentAttackPhase = AttackPhasesType.End;
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


    public void SpeialAtkTest()
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
        if(anim == CharacterAnimationStateType.Arriving)
        {
           // Debug.Log("Arriving");
        }
        SetupSpineAnim();
        Loop = loop;
        SpineAnimationState.SetAnimation(1, anim.ToString(), loop).MixDuration = transition;
        CurrentAnim = anim;
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
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

