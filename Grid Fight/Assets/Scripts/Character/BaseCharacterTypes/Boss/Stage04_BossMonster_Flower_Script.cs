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
        StartCoroutine(base.MoveByTile(GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).transform.position, SpineAnim.UpMovementSpeed, SpineAnim.GetAnimLenght(animType)));
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

    protected override IEnumerator MoveByTile(Vector3 nextPos, AnimationCurve curve, float animLength)
    {

        
        SetAnimation(CharacterAnimationStateType.Idle);
        return base.MoveByTile(nextPos, curve, animLength);
    }

    public override void StopMoveCo()
    {
        base.StopMoveCo();
    }


    public override void SetCharDead()
    {
        if (SpineAnim.CurrentAnim != CharacterAnimationStateType.Death)
        {
            CameraManagerScript.Instance.CameraShake();
            SetAttackReady(false);
            Call_CurrentCharIsDeadEvent();
            StartCoroutine(DeathStasy());
        }
    }

    private IEnumerator DeathStasy()
    {
        float timer = 0;
        if (Smoke == null)
        {
            Smoke = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage04FlowersSmoke);
            Smoke.transform.parent = transform;
            Smoke.transform.localPosition = Vector3.zero;
        }
        Smoke.SetActive(true);
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
        Smoke.SetActive(false);
        SetAttackReady(true);
        SetAnimation(CharacterAnimationStateType.Idle);
        CharInfo.Health = CharInfo.HealthStats.Base;
    }
}
