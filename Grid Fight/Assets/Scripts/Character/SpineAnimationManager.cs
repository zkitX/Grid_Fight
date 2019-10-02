using Spine.Unity;
using UnityEngine;

public class SpineAnimationManager : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    public Spine.AnimationState SpineAnimationState;
    public SkeletonAnimation ShadowSkeletonAnimation;
    public Spine.AnimationState ShadowSpineAnimationState;

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
            SpineAnimationState = skeletonAnimation.AnimationState;
            ShadowSpineAnimationState = ShadowSkeletonAnimation.AnimationState;
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
            CharOwner.CreateBullet();
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
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Atk1:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Buff:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Debuff:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Gettinghit:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Defending:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Paralized:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Arriving:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashRight:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashLeft:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashDown:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.DashUp:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Selection:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.PowerUp:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Speaking:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Victory:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Defeat:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
                SetAnim(CharacterAnimationStateType.Idle, true);
                break;
            case CharacterAnimationStateType.Death:
                SpineAnimationState.ClearTrack(trackEntry.TrackIndex);
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
        SpineAnimationState.SetAnimation(0, anim.ToString(), loop);
        ShadowSpineAnimationState.SetAnimation(0, anim.ToString(), loop);
    }

    public void SetMixAnim(CharacterAnimationStateType anim, float duration, bool loop)
    {
        SetupSpineAnim();
        SpineAnimationState.SetEmptyAnimation(1, 0);
        SpineAnimationState.AddAnimation(1, anim.ToString(), loop, 0).MixDuration = duration;
        ShadowSpineAnimationState.SetEmptyAnimation(1, 0);
        ShadowSpineAnimationState.AddAnimation(1, anim.ToString(), loop, 0).MixDuration = duration;
    }

    public float GetAnimLenght(CharacterAnimationStateType anim)
    {
        return skeletonAnimation.Skeleton.Data.FindAnimation(anim.ToString()).Duration;
    }

    public void SetAnimationSpeed(float speed)
    {
        SpineAnimationState.Tracks.ForEach(r => r.TimeScale = speed);
        ShadowSpineAnimationState.Tracks.ForEach(r => r.TimeScale = speed);
    }

}




