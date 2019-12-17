using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossGirl_Flower_Script : MinionType_Script
{

    public Vector2Int BasePos;
    private float StasyTime = 30;
    public bool CanRebirth = true;
    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Growing);
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
        IsOnField = false;
        CanAttack = false;
        Call_CurrentCharIsDeadEvent();
    }

    private IEnumerator DeathStasy()
    {
        float timer = 0;
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
        if(CanRebirth)
        {
            base.Call_CurrentCharIsRebirthEvent();
            CharInfo.HealthStats.Health = CharInfo.HealthStats.Base;
            IsOnField = true;
            CanAttack = true;
        }
    }
}


