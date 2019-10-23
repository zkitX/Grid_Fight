using Spine.Unity;
using System.Collections;
using UnityEngine;

public class SpineAnimationManager : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton skeleton;
    public CharacterAnimationStateType BaseAnim;
    public CharacterAnimationStateType MixAnim;
    public CharacterBase CharOwner;

    public AnimationCurve UpMovementSpeed;
    public AnimationCurve DownMovementSpeed;
    public AnimationCurve LeftMovementSpeed;
    public AnimationCurve RightMovementSpeed;

    public Transform FiringPoint;
    public CharacterAnimationStateType CurrentAnim;
    public float AnimationTransition = 0.1f;
    private void SetupSpineAnim()
    {
        if(skeletonAnimation == null)
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            SpineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
            SpineAnimationState.Complete += SpineAnimationState_Complete;
            SpineAnimationState.Event += SpineAnimationState_Event;
            SpineAnimationState.SetAnimation(0, "Idle", true);
            SpineAnimationState.SetEmptyAnimation(1, 0);
            CurrentAnim = CharacterAnimationStateType.Idle;
        }
    }

    private void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "FireParticles")
        {
            CharOwner.CastAttackParticles();
            switch (CharOwner.CharInfo.ClassType)
            {
                case CharacterClassType.Valley:
                    CharOwner.CreateSingleBullet();
                    break;
                case CharacterClassType.Mountain:
                    CharOwner.CreateSingleBullet();
                    break;
                case CharacterClassType.Forest:
                    CharOwner.CreateMachingunBullets();
                    break;
                case CharacterClassType.Desert:
                    CharOwner.CreateSingleBullet();
                    break;
            }
            
        }
    }

    private void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        if (skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name == "<empty>")
        {
            return;
        }

        SpineAnimationState.SetAnimation(0, "Idle", true);
    }

    public void SetAnim(CharacterAnimationStateType anim, bool loop)
    {
         SetupSpineAnim();
         SpineAnimationState.SetAnimation(0, anim.ToString(), loop);
         CurrentAnim = anim;
    }


    public void SetMixAnim(CharacterAnimationStateType anim, bool loop)
    {
        CurrentAnim = anim;
        SetupSpineAnim();
        SpineAnimationState.SetEmptyAnimation(1, 0);
        SpineAnimationState.AddAnimation(1, anim.ToString(), false, 0).MixDuration = AnimationTransition;
        StartCoroutine(ClearTrack(GetAnimLenght(anim), anim, 1));
    }

    public IEnumerator ClearTrack(float duration, CharacterAnimationStateType anim, int track)
    {
        float timer = 0;
        bool test;
        while (timer <= duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }
        if(anim == CurrentAnim)
        {
            SpineAnimationState.AddEmptyAnimation(track, 0.2f, 0);
            CurrentAnim = CharacterAnimationStateType.Idle;
            SpineAnimationState.SetAnimation(0, "Idle", true);
        }
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
    }

    public void SetAnimationSpeed(float speed)
    {
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
            SetMixAnim(CharacterAnimationStateType.Gettinghit, false);
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

