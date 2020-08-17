using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossMonster_Flower_Script : MinionType_Script
{
    public Vector2Int BasePos;
    public float StasyTime = 15;
    public bool CanRebirth = true;
    public MonsterFlowerType mfType;
    public GameObject Smoke;

    public override void SetUpEnteringOnBattle()
    {
        CharacterAnimationStateType animType = (CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), CharacterAnimationStateType.Growing.ToString() + Random.Range(1, 3).ToString());
        SetAnimation(animType);
        StartCoroutine(base.MoveByTileSpace(GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).transform.position, SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.UpMovement : SpineAnim.Speed_Time_Curves.UpMovement, 0));
        Skin newSkin = new Skin("new-skin"); // 1. Create a new empty skin
        newSkin.AddSkin(SpineAnim.skeleton.Data.FindSkin(mfType.ToString())); // 2. Add items
        SpineAnim.skeleton.SetSkin(mfType.ToString());
        SpineAnim.skeleton.SetSlotsToSetupPose();
        SpineAnim.SpineAnimationState.Apply(SpineAnim.skeleton);
    }

    public override void SetCharDead()
    {
        if (SpineAnim.CurrentAnim != CharacterAnimationStateType.Death.ToString())
        {
            CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
            SetAttackReady(false);
            Call_CurrentCharIsDeadEvent();
            StartCoroutine(DeathStasy());
        }
    }

    private IEnumerator DeathStasy()
    {
        if (Smoke == null)
        {
            Smoke = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter05_AscensoMountain_FlowersSmoke);
            Smoke.transform.parent = transform;
            Smoke.transform.localPosition = Vector3.zero;
        }
        Smoke.SetActive(true);
        SetAnimation(CharacterAnimationStateType.Death);
        yield return BattleManagerScript.Instance.WaitFor(StasyTime, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        Smoke.SetActive(false);
        SetAttackReady(true);
        SetAnimation(CharacterAnimationStateType.Idle);
        CharInfo.Health = CharInfo.HealthStats.Base;
        EventManager.Instance.UpdateHealth(this);
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        if (animState.ToString().Contains("Dash"))
        {
            animState = CharacterAnimationStateType.Idle;
            loop = true;
        }
        base.SetAnimation(animState, loop, transition);
    }
}
