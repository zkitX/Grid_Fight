using Spine.Unity;
using System.Collections;
using UnityEngine;

public class SpineAnimationManager : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton skeleton;
    public CharacterBase CharOwner;

    public AnimationCurve UpMovementSpeed;
    public AnimationCurve DownMovementSpeed;
    public AnimationCurve LeftMovementSpeed;
    public AnimationCurve RightMovementSpeed;

    public Transform FiringPoint;
    public Transform SpecialFiringPoint;
    public CharacterAnimationStateType CurrentAnim;
    public float AnimationTransition = 0.1f;
    private float BaseSpeed;

//initialize all the spine element
    private void SetupSpineAnim()
    {
        if(skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
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
        if (e.Data.Name.Contains("FireParticle"))
        {
            
            switch (CharOwner.CharInfo.ClassType)
            {
                case CharacterClassType.Valley:
                    //CharOwner.CreateSingleBullet(CharOwner.CharInfo.BulletDistanceInTile[0], CharOwner.NextAttackLevel);
                    CharOwner.CreateSingleBullet();
                    if (!CharOwner.VFXTestMode)
                    {
                        CharOwner.NextAttackLevel = CharacterLevelType.Novice;

                    }
                    break;
                case CharacterClassType.Mountain:
                    //CharOwner.CreateSingleBullet(CharOwner.CharInfo.BulletDistanceInTile[0], CharOwner.NextAttackLevel);
                    CharOwner.CreateSingleBullet();
                    if (!CharOwner.VFXTestMode)
                    {
                        CharOwner.NextAttackLevel = CharacterLevelType.Novice;

                    }
                    break;
                case CharacterClassType.Forest:
                    CharOwner.CreateMachingunBullets();
                    break;
                case CharacterClassType.Desert:
                    //CharOwner.CreateSingleBullet(CharOwner.CharInfo.BulletDistanceInTile[0], CharOwner.NextAttackLevel);
                    CharOwner.CreateSingleBullet();
                    if (!CharOwner.VFXTestMode)
                    {
                        CharOwner.NextAttackLevel = CharacterLevelType.Novice;

                    }
                    break;
            }
            
        }
    }


    //Method fired when an animation is complete
    private void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {

        if (skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name == "<empty>")
        {
            return;
        }

        CharacterAnimationStateType completedAnim = (CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name);

        if (completedAnim == CharacterAnimationStateType.Arriving)
        {
            CharOwner.IsOnField = true;
            
        }
        if(completedAnim != CharacterAnimationStateType.Idle)
        {
            SpineAnimationState.AddEmptyAnimation(1,AnimationTransition,0);
            CurrentAnim = CharacterAnimationStateType.Idle;
        }
       
        
    }


    public void SetAnim(CharacterAnimationStateType anim, bool loop)
    {
        SetupSpineAnim();
     
        if(CurrentAnim == CharacterAnimationStateType.Atk && anim == CharacterAnimationStateType.Atk)
        {
            return;
        }
        SpineAnimationState.AddAnimation(1, anim.ToString(), loop, 0).MixDuration = AnimationTransition;
        CurrentAnim = anim;
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
    }

    public void SetAnimationSpeed(float speed)
    {
        BaseSpeed = speed;
        SpineAnimationState.Tracks.ForEach(r => {
            if(r != null)
            {
                r.TimeScale = speed;
            }
        });
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.V))
        {
           // SetMixAnim(CharacterAnimationStateType.Gettinghit, false);
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

