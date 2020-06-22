using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMinionType_Script : MinionType_Script
{
    public override IEnumerator AI()
    {
        bool val = true;
        while (val)
        {
            yield return null;
            if (IsOnField)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
                }


                List<BaseCharacter> enemys = WaveManagerScript.Instance.WaveCharcters.Where(r => r.IsOnField).ToList();
                if (enemys.Count > 0)
                {
                    BaseCharacter targetChar = enemys.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x).FirstOrDefault();
                    /*BaseCharacter targetChar = null;
                    List<BaseCharacter> possibleTargets = enemys.Where(r => Mathf.Abs(r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x) <= 1).ToList();
                    if (possibleTargets.Count > 0)
                    {
                        targetChar = possibleTargets[Random.Range(0, possibleTargets.Count)];
                    }*/
                    if (targetChar != null)
                    {
                        nextAttackPos = targetChar.UMS.CurrentTilePos;
                        yield return AttackSequence();
                    }
                    else
                    {

                        int randomizer = Random.Range(0, 100);
                        if (randomizer < UpDownPerc)
                        {
                            yield return MoveCharOnDir_Co(InputDirection.Left);
                        }
                        else if (randomizer > (100 - UpDownPerc))
                        {
                            yield return MoveCharOnDir_Co(InputDirection.Right);
                        }
                        else
                        {
                            targetChar = GetTargetChar(enemys);
                            if (targetChar.UMS.CurrentTilePos.x < UMS.CurrentTilePos.x)
                            {
                                yield return MoveCharOnDir_Co(InputDirection.Up);
                            }
                            else
                            {
                                yield return MoveCharOnDir_Co(InputDirection.Down);
                            }
                        }
                    }
                }
                yield return null;
            }
        }
    }


    public override void SetCharDead()
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);

        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        Attacking = false;
        BuffsDebuffsList.ForEach(r =>
        {
            r.Duration = 0;
            r.CurrentBuffDebuff.Duration = 0;
            r.CurrentBuffDebuff.Stop_Co = true;
        }
        );


        for (int i = 0; i < HittedByList.Count; i++)
        {
            StatisticInfoClass sic = StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == HittedByList[i].CharacterId).FirstOrDefault();
            if (sic != null)
            {
                sic.DamageExp += (HittedByList[i].Damage / totDamage) * CharInfo.ExperienceValue;
            }
        }
        totDamage = 0;

        for (int i = 0; i < UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
            UMS.Pos[i] = Vector2Int.zero;
        }
        base.SetCharDead();
        SpineAnim.SpineAnimationState.Event -= SpineAnimationState_Event;
        SpineAnim.SpineAnimationState.Complete -= SpineAnimationState_Complete;
        CharInfo.BaseSpeedChangedEvent -= _CharInfo_BaseSpeedChangedEvent;
        CharInfo.DeathEvent -= _CharInfo_DeathEvent;
        Invoke("DisableChar", 0.5f);
    }
}
