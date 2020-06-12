﻿using Spine;
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
        StartCoroutine(base.MoveByTileSpeed(GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).transform.position, SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.UpMovement : SpineAnim.Speed_Time_Curves.UpMovement, SpineAnim.GetAnimLenght(animType)));
        Skin newSkin = new Skin("new-skin"); // 1. Create a new empty skin
        newSkin.AddSkin(SpineAnim.skeleton.Data.FindSkin(mfType.ToString())); // 2. Add items
        SpineAnim.skeleton.SetSkin(mfType.ToString());
        SpineAnim.skeleton.SetSlotsToSetupPose();
        SpineAnim.SpineAnimationState.Apply(SpineAnim.skeleton);
    }

    public override void StartMoveCo()
    {
       // base.StartMoveCo();
    }

    public override IEnumerator Move()
    {

        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        while (MoveCoOn)
        {
            InputDirection dir = (InputDirection)Random.Range(0, 4);
            float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);
            yield return BattleManagerScript.Instance.WaitFor(1, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

            if (CharInfo.Health > 0)
            {
                MoveCharOnDirection(dir);
            }
            MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);

            yield return BattleManagerScript.Instance.WaitFor(1, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

            if (CharInfo.Health > 0)
            {
                MoveCharOnDirection(dir == InputDirection.Down ? InputDirection.Up : dir == InputDirection.Up ? InputDirection.Down : dir == InputDirection.Left ? InputDirection.Right : InputDirection.Left);
            }
        }
    }

    public override IEnumerator MoveByTileSpeed(Vector3 nextPos, AnimationCurve curve, float animLength)
    {
        SetAnimation(CharacterAnimationStateType.Idle);
        return base.MoveByTileSpeed(nextPos, curve, animLength);
    }

    public override void StopMoveCo()
    {
        base.StopMoveCo();
    }


    public override void SetCharDead(bool hasToDisappear = true)
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
            Smoke = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage04FlowersSmoke);
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
