using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossMonster_Flower_Script : MinionType_Script
{
    public Vector2Int BasePos;


    [SerializeField]
    private MonsterFlowerType MonsterFlower;


    public override void SetUpEnteringOnBattle()
    {
        SetAnimation((CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), CharacterAnimationStateType.Growing.ToString() + Random.Range(1, 3).ToString()));
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
}
