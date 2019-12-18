using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossMonster_Flower_Script : MinionType_Script
{
    public Vector2Int BasePos;
    private float StasyTime = 15;
    public bool CanRebirth = true;
    public MonsterFlowerType mfType;

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation((CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), CharacterAnimationStateType.Growing.ToString() + Random.Range(1, 3).ToString()));
        Skin newSkin = new Skin("new-skin"); // 1. Create a new empty skin
        newSkin.AddSkin(SpineAnim.skeleton.Data.FindSkin(mfType.ToString())); // 2. Add items
        SpineAnim.skeleton.SetSkin(mfType.ToString());
        SpineAnim.skeleton.SetSlotsToSetupPose();
        SpineAnim.SpineAnimationState.Apply(SpineAnim.skeleton);
        StartCoroutine(MoveByTile(GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).transform.position, CharacterAnimationStateType.Growing, SpineAnim.UpMovementSpeed));
    }

    public override void StartMoveCo()
    {
        base.StartMoveCo();
    }

    public override IEnumerator Move()
    {
        while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            yield return new WaitForFixedUpdate();
        }
        SetAttackReady();
        while (MoveCoOn)
        {
            float timer = 0;
            float MoveTime = Random.Range(MinMovementTimer, MaxMovementTimer);
            while (timer < 1)
            {
                yield return new WaitForFixedUpdate();
                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return new WaitForFixedUpdate();
                }

                timer += Time.fixedDeltaTime / MoveTime;
            }
            if (CharInfo.Health > 0)
            {
                MoveCharOnDirection((InputDirection)Random.Range(0, 4));
            }
            MoveTime = Random.Range(MinMovementTimer, MaxMovementTimer);
            while (timer < 1)
            {
                yield return new WaitForFixedUpdate();
                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return new WaitForFixedUpdate();
                }

                timer += Time.fixedDeltaTime / MoveTime;
            }
            if (CharInfo.Health > 0)
            {
                StartCoroutine(MoveByTile(GridManagerScript.Instance.GetBattleTile(BasePos).transform.position, CharacterAnimationStateType.Idle, SpineAnim.UpMovementSpeed));
            }
        }
    }

    protected override IEnumerator MoveByTile(Vector3 nextPos, CharacterAnimationStateType animState, AnimationCurve curve)
    {
        return base.MoveByTile(nextPos, CharacterAnimationStateType.Idle, curve);
    }

    public override void StopMoveCo()
    {
        base.StopMoveCo();
    }


    public override void SetCharDead()
    {
        if (SpineAnim.CurrentAnim != CharacterAnimationStateType.Death)
        {
            IsOnField = false;
            CanAttack = false;
            CharBoxCollider.enabled = false;
            Call_CurrentCharIsDeadEvent();
            StartCoroutine(DeathStasy());
        }
    }

    private IEnumerator DeathStasy()
    {
        float timer = 0;
        SetAnimation(CharacterAnimationStateType.Death);
        while (timer < StasyTime)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
            {
                yield return new WaitForFixedUpdate();
            }

            timer += Time.fixedDeltaTime;
        }

        Call_CurrentCharIsRebirthEvent();
    }

    protected override void Call_CurrentCharIsRebirthEvent()
    {
        if (CanRebirth)
        {
            CharBoxCollider.enabled = true;
            SetAnimation(CharacterAnimationStateType.Idle);
            base.Call_CurrentCharIsRebirthEvent();
            CharInfo.HealthStats.Health = CharInfo.HealthStats.Base;
            IsOnField = true;
            CanAttack = true;
        }
    }
}
