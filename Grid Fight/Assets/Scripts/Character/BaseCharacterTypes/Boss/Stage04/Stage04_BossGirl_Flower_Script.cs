using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage04_BossGirl_Flower_Script : MinionType_Script
{

    public Vector2Int BasePos;
    public float StasyTime = 50;
    public bool CanRebirth = true;
    public GameObject Smoke;
    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Growing);
        GridManagerScript.Instance.SetBattleTileState(UMS.Pos[0], BattleTileStateType.Occupied);
        StartCoroutine(base.MoveByTileSpeed(GridManagerScript.Instance.GetBattleTile(UMS.Pos[0]).transform.position, SpineAnim.CurveType == MovementCurveType.Space_Time ? SpineAnim.Space_Time_Curves.UpMovement : SpineAnim.Speed_Time_Curves.UpMovement, SpineAnim.GetAnimLenght(CharacterAnimationStateType.Growing)));
    }

    public override IEnumerator AI()
    {
        while (BattleManagerScript.Instance.PlayerControlledCharacters.Length == 0)
        {
            yield return null;
        }
        int times = 0;
        int MoveTime = 0;
        bool val = true;
        InputDirection dir = InputDirection.Down;
        bool goBack = false;
        while (val)
        {
            yield return null;
            if (IsOnField)
            {

                yield return BattleManagerScript.Instance.WaitUpdate(() =>BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

                if (MoveTime == 0)
                {
                    MoveTime = Random.Range(1, 2);
                }
                else
                {
                    times++;
                    if (times == MoveTime)
                    {
                        yield return new WaitForSecondsRealtime(1);
                        if (goBack)
                        {
                            if (CharInfo.Health > 0)
                            {
                                yield return MoveCharOnDir_Co(dir == InputDirection.Down ? InputDirection.Up : dir == InputDirection.Up ? InputDirection.Down : dir == InputDirection.Left ? InputDirection.Right : InputDirection.Left);
                                goBack = false;
                            }

                        }
                        else
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                dir = (InputDirection)Random.Range(0, 4);
                                BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos + (dir == InputDirection.Down ? new Vector2Int(1, 0) :
                                    dir == InputDirection.Up ? new Vector2Int(-1, 0) : dir == InputDirection.Left ? new Vector2Int(0, -1) : new Vector2Int(0, 1)));
                                if (bts.BattleTileState == BattleTileStateType.Empty && bts.WalkingSide == UMS.WalkingSide)
                                {
                                    break;
                                }
                            }
                            
                            goBack = true;
                            if (CharInfo.Health > 0)
                            {
                                yield return MoveCharOnDir_Co(dir);
                            }
                        }

                        times = 0;
                        MoveTime = 0;
                    }
                }
                yield return AttackSequence();
                yield return null;
            }
        }
    }

  

    public override IEnumerator MoveByTileSpeed(Vector3 nextPos, AnimationCurve curve, float animLenght)
    {
        return base.MoveByTileSpeed(nextPos, curve, animLenght);
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
        if(Smoke == null)
        {
            Smoke = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage04FlowersSmoke);
            Smoke.transform.parent = transform;
            Smoke.transform.localPosition = Vector3.zero;
        }
        Smoke.SetActive(true);
        SetAnimation(CharacterAnimationStateType.Death);
        yield return BattleManagerScript.Instance.WaitFor(StasyTime, () =>BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);

        Call_CurrentCharIsRebirthEvent();
    }

    protected override void Call_CurrentCharIsRebirthEvent()
    {
        if(CanRebirth)
        {
            Smoke.SetActive(false);
            SetAttackReady(true);
            SetAnimation(CharacterAnimationStateType.Idle);
            base.Call_CurrentCharIsRebirthEvent();
            CharInfo.Health = CharInfo.HealthStats.Base;
            EventManager.Instance.UpdateHealth(this);
            EventManager.Instance.UpdateStamina(this);
        }
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        if(animState.ToString().Contains("Dash"))
        {
            animState = CharacterAnimationStateType.Idle;
            loop = true;
        }
        base.SetAnimation(animState, loop, transition);
    }
}


