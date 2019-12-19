using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionType_Script : BaseCharacter
{
    public Vector2 MovementTimer = new Vector2(5,8);
    protected IEnumerator MoveCo;
    protected bool MoveCoOn = true;

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
    }

    public override void StartMoveCo()
    {
        MoveCoOn = true;
        MoveCo = Move();
        StartCoroutine(MoveCo);
    }

    public virtual IEnumerator Move()
    {
        while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
        {
            yield return new WaitForFixedUpdate();
        }
        while (MoveCoOn)
        {
            float timer = 0;
            float MoveTime = Random.Range(MovementTimer.x, MovementTimer.y);
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
        }
    }

    public override void StopMoveCo()
    {
        MoveCoOn = false;
        if (MoveCo != null)
        {
            StopCoroutine(MoveCo);
        }
    }
}
