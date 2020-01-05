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
        StartCoroutine(base.MoveByTile(GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).transform.position, (CharacterAnimationStateType)System.Enum.Parse(typeof(CharacterAnimationStateType), CharacterAnimationStateType.Growing.ToString() + Random.Range(1, 3).ToString()), SpineAnim.UpMovementSpeed));
        Skin newSkin = new Skin("new-skin"); // 1. Create a new empty skin
        newSkin.AddSkin(SpineAnim.skeleton.Data.FindSkin(mfType.ToString())); // 2. Add items
        SpineAnim.skeleton.SetSkin(mfType.ToString());
        SpineAnim.skeleton.SetSlotsToSetupPose();
        SpineAnim.SpineAnimationState.Apply(SpineAnim.skeleton);
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
        while (MoveCoOn)
        {
            float timer = 0;
            InputDirection dir = (InputDirection)Random.Range(0, 4);
            float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);
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
                MoveCharOnDirection(dir);
            }
            timer = 0;
            MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y);
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
                MoveCharOnDirection(dir == InputDirection.Down ? InputDirection.Up : dir == InputDirection.Up ? InputDirection.Down : dir == InputDirection.Left ? InputDirection.Right : InputDirection.Left);
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
            SetAttackReady(false);
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

        SetAttackReady(true);
        SetAnimation(CharacterAnimationStateType.Idle);
        CharInfo.Health = CharInfo.HealthStats.Base;
    }
}
