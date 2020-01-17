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


//initialize all the spine element
    private void SetupSpineAnim()
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
            if (CurrentAnim == CharacterAnimationStateType.Atk)
            {
                CharOwner.currentAttackPhase = AttackPhasesType.Cast;
            }
            else
            {
                CharOwner.currentSpecialAttackPhase = AttackPhasesType.Cast;
            }
            CharOwner.FireCastParticles();
        }
        if (e.Data.Name.Contains("FireBulletParticle"))
        {
            if(CurrentAnim == CharacterAnimationStateType.Atk)
            {
                CharOwner.currentAttackPhase = AttackPhasesType.Bullet;
            }
            else
            {
                CharOwner.currentSpecialAttackPhase = AttackPhasesType.Bullet;
            }
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
            CharOwner.SetAttackReady(true);
            CharOwner.StartAttakCo();
        }
        if(CurrentAnim == CharacterAnimationStateType.Death)
        {
            return;
        }

        if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk && CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk)
        {
            ((CharacterType_Script)CharOwner).SpecialAttackLoop();
            return;
        }
      
        if (completedAnim == CharacterAnimationStateType.Atk1_Loop &&
                CurrentAnim == CharacterAnimationStateType.Atk1_Loop)
        {
            if(((CharacterType_Script)CharOwner).Atk1Queueing)
            {
                ((CharacterType_Script)CharOwner).SpecialAttackLoop();
            }
            else
            {
                SetAnim(CharacterAnimationStateType.Atk1_AtkToIdle);
            }
            return;
        }

        if (completedAnim == CharacterAnimationStateType.Atk)
        {
            CharOwner.currentAttackPhase = AttackPhasesType.End;
        }
        if (completedAnim == CharacterAnimationStateType.Atk1_AtkToIdle)
        {
            CharOwner.currentSpecialAttackPhase = AttackPhasesType.End;
        }


        if (completedAnim != CharacterAnimationStateType.Idle)
        {
            SetAnimationSpeed(CharOwner.CharInfo.BaseSpeed);
            SpineAnimationState.AddEmptyAnimation(1,AnimationTransition,0);
            //Debug.Log("Idle --- ");
            CurrentAnim = CharacterAnimationStateType.Idle;
        }
    }


    public void SpeialAtkTest()
    {
        Debug.Log("Test");
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
            SpineAnimationState.SetAnimation(1, anim.ToString(), false);
        }
        else
        {
            SpineAnimationState.SetAnimation(1, anim.ToString(), anim == CharacterAnimationStateType.Death || anim.ToString().Contains("Idle") ? true : false).MixDuration = AnimationTransition;
        }
        //Debug.Log(CurrentAnim.ToString() + "  --- ");
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

