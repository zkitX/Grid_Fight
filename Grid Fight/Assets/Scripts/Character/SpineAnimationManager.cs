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
    public CurrentAnimClass CurrentAnim = new CurrentAnimClass();
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
        }
    }

    private void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "FireParticles")
        {
            CharOwner.CastAttackParticles();
            switch (CharOwner.BulletInfo.ClassType)
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

       

        //Debug.Log("Complete" + "   " + skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name + "     " + trackEntry.TrackIndex);


        if (trackEntry.TrackIndex != CurrentAnim.CurrentTrack)
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
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Atk1:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Buff:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Debuff:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Gettinghit:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Defending:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Paralized:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Arriving:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashRight:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashLeft:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashDown:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashUp:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Selection:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.PowerUp:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Speaking:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Victory:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Defeat:
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Death:
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
            SetMixAnim(MixAnim, AnimationTransition, false);
        }

    }

    public void SetAnim(CharacterAnimationStateType anim, bool loop)
    {
        SetMixAnim(anim, AnimationTransition, loop);
    }

    public void SetMixAnim(CharacterAnimationStateType anim, float duration, bool loop)
    {
        SetupSpineAnim();
        CurrentAnim.CurrentAnimation = anim;
        CurrentAnim.CurrentTrack++;
        SpineAnimationState.SetEmptyAnimation(CurrentAnim.CurrentTrack, 0);
        SpineAnimationState.AddAnimation(CurrentAnim.CurrentTrack, anim.ToString(), loop, 0).MixDuration = duration;
        if(CurrentAnim.CurrentTrack - 1 >= 0)
        {
            StartCoroutine(ClearTrack(duration, CurrentAnim.CurrentTrack - 1));
        }
        //Debug.Log("SetMixAnim   " + anim.ToString() + "   " + CurrentAnim.CurrentTrack);

       
    }

   
    public IEnumerator ClearTrack(float duration, int track)
    {
        float timer = 0;
        while (timer <= duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }

            timer += Time.fixedDeltaTime;
        }
        //Debug.Log("Clear   " + track);
        SpineAnimationState.ClearTrack(track);
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

}

[System.Serializable]
public class CurrentAnimClass
{
    public CharacterAnimationStateType CurrentAnimation;
    public int CurrentTrack;

    public CurrentAnimClass()
    {
        CurrentTrack = -1;
    }
}

