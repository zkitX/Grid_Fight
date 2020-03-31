using Spine.Unity;
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

    //Method fired when an animation is complete
    private void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
       //Debug.Log(skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name + "   " + CurrentAnim.ToString());
        if (skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name == "<empty>")
        {
            return;
        }



        string completedAnim = skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name;


        if (CurrentAnim == CharacterAnimationStateType.Idle.ToString())
        {
            return;
        }

        if(completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString())
        {
            CharOwner.transform.position = new Vector3(100,100,100);
            return;
        }

        if (completedAnim == CharacterAnimationStateType.Arriving.ToString() || completedAnim.Contains("Growing"))
        {
            CharOwner.IsSwapping = false;
            CharOwner.SwapWhenPossible = false;
            CharOwner.CharArrivedOnBattleField();
        }
        if (CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            CharOwner.IsSwapping = false;
            CharOwner.SwapWhenPossible = false;
            CharOwner.SetAttackReady(false);
        }

        if (CurrentAnim == CharacterAnimationStateType.Death.ToString())
        {
            return;
        }

        //DEATH
        if (CurrentAnim == CharacterAnimationStateType.Death_Prep.ToString() && completedAnim == CharacterAnimationStateType.Death_Prep.ToString())
        {
            CharOwner.currentDeathProcessPhase = DeathProcessStage.LoopDying;
            return;
        }
        if (CurrentAnim == CharacterAnimationStateType.Death_Exit.ToString() && completedAnim == CharacterAnimationStateType.Death_Exit.ToString())
        {
            CharOwner.currentDeathProcessPhase = DeathProcessStage.None;
            CharOwner.gameObject.SetActive(false);
            return;
        }
        /*if (CurrentAnim == CharacterAnimationStateType.Death_Loop && 
            completedAnim == CharacterAnimationStateType.Death_Loop && 
            CharOwner.currentDeathProcessPhase == DeathProcessStage.End)
        {
            SetAnim(CharacterAnimationStateType.Death_Loop, true, 0);
            return;
        }*/
        //END DEATH

        //Switch between character and minion unique attacking animation sequences
        if (CharOwner.UMS.CurrentAttackType == AttackType.Particles)
        {
            #region QuickAttack

            if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString() && CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
            {
                ((CharacterType_Script)CharOwner).QuickAttack();
                return;
            }
            if (completedAnim == CharacterAnimationStateType.Atk1_Loop.ToString() &&
             CurrentAnim == CharacterAnimationStateType.Atk1_Loop.ToString())
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
            if (completedAnim == CharacterAnimationStateType.Atk2_IdleToAtk.ToString() && CurrentAnim == CharacterAnimationStateType.Atk2_IdleToAtk.ToString())
            {
                ((CharacterType_Script)CharOwner).ChargingLoop();
                return;
            }
            #endregion
            if (completedAnim == CharacterAnimationStateType.Atk1_AtkToIdle.ToString() || completedAnim == CharacterAnimationStateType.Atk2_AtkToIdle.ToString()
                || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
            {
                CharOwner.currentAttackPhase = AttackPhasesType.End;
            }
        }
        else if (CharOwner.UMS.CurrentAttackType == AttackType.Tile)
        {
            if (completedAnim.Contains("IdleToAtk") && CurrentAnim.Contains("IdleToAtk"))
            {
                Char atkNum = 'a';
                foreach (Char letter in completedAnim.ToCharArray())
                {
                    if (Char.IsDigit(letter))
                    {
                        atkNum = letter;
                        break;
                    }
                }
                if (atkNum == 'a')
                {
                    atkNum = '\0';
                    Debug.LogError("THE ATTACK ANIM PASSED DOES NOT CONTAIN A NUMERIC ID, defaulting to basic");
                }
                CharOwner.SetAnimation("Atk" + atkNum.ToString() + "_Charging", true, 0);
            }

            if(completedAnim.Contains("_Loop") && CurrentAnim.Contains("_Loop"))
            {
                Char atkNum = 'a';
                foreach(Char letter in completedAnim.ToCharArray())
                {
                    if(Char.IsDigit(letter))
                    {
                        atkNum = letter;
                        break;
                    }
                }
                if(atkNum == 'a')
                {
                    atkNum = '\0';
                    Debug.LogError("THE ATTACK ANIM PASSED DOES NOT CONTAIN A NUMERIC ID, defaulting to basic");
                }

                //If they can still attack, keep them in the charging loop
                if(CharOwner.shotsLeftInAttack > 0)
                {
                    CharOwner.SetAnimation("Atk" + atkNum.ToString() + "_Charging", true, 0);
                }
                //otherwise revert them to the idle postion
                else
                {
                    CharOwner.SetAnimation("Atk" + atkNum.ToString() + "_AtkToIdle");
                    CharOwner.currentAttackPhase = AttackPhasesType.End;
                }
                return;
            }

            if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
            {
                CharOwner.currentAttackPhase = AttackPhasesType.End;
                CharOwner.shotsLeftInAttack = 0;
            }
        }

        if (completedAnim != CharacterAnimationStateType.Idle.ToString() && !Loop)
        {
            SetAnimationSpeed(CharOwner.CharInfo.BaseSpeed);
            //Debug.Log("IDLE     " + completedAnim.ToString());
            SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            //SpineAnimationState.AddEmptyAnimation(1,AnimationTransition,0);
            CurrentAnim = CharacterAnimationStateType.Idle.ToString();
        }
    }


    public void SetAnim(CharacterAnimationStateType anim)
    {
        SetupSpineAnim();

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
         SetupSpineAnim();
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
        SetupSpineAnim();
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

