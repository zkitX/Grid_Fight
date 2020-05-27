using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;

public class Stage01_Boss_Script : MinionType_Script
{
    public enum Stage01_Boss_MaskType
    {
        WarDrums,
        LifeDrums,
        MoonDrums,
        CrystalTomb
    }


    public Stage01_Boss_MaskType CurrentPhase;
    public List<BattleTileScript> AttackedTilesList = new List<BattleTileScript>();

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

                AttackedTilesList.Clear();
                List<BaseCharacter> enemys = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToList();
                if (enemys.Count > 0)
                {
                    BaseCharacter targetChar = enemys.Where(r => r.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x).FirstOrDefault();
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
                            yield return new WaitForSeconds(1);
                        }
                        else if (randomizer > (100 - UpDownPerc))
                        {
                            yield return MoveCharOnDir_Co(InputDirection.Right);
                            yield return new WaitForSeconds(1);
                        }
                        else
                        {
                            targetChar = GetTargetChar(enemys);
                            if (targetChar.UMS.CurrentTilePos.x < UMS.CurrentTilePos.x)
                            {
                                yield return MoveCharOnDir_Co(InputDirection.Up);
                                yield return new WaitForSeconds(1);
                            }
                            else
                            {
                                yield return MoveCharOnDir_Co(InputDirection.Down);
                                yield return new WaitForSeconds(1);
                            }
                        }
                    }
                }
                yield return null;
            }
        }
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if(animState.Contains("Dash"))
        {
            return;
        }

        if(animState.Contains("Atk") || animState.Contains("Charging") || animState.Contains("Loop"))
        {
            switch (nextAttack.AttackInput)
            {
                case AttackInputType.Weak:
                    CurrentPhase = Stage01_Boss_MaskType.WarDrums;
                    break;
                case AttackInputType.Strong:
                    CurrentPhase = Stage01_Boss_MaskType.CrystalTomb;
                    break;
                case AttackInputType.Skill1:
                    CurrentPhase = Stage01_Boss_MaskType.LifeDrums;
                    break;
                case AttackInputType.Skill2:
                    CurrentPhase = Stage01_Boss_MaskType.MoonDrums;
                    break;
            }


        }

        base.SetAnimation(CurrentPhase.ToString() + "_" + animState, loop, transition, _pauseOnLastFrame);
    }

    public override void SpineAnimationState_Complete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "<empty>" || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString()
       || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString())
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;

        if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            return;
        }

        if (completedAnim.Contains("_Loop") && SpineAnim.CurrentAnim.Contains("_Loop"))
        {

            //If they can still attack, keep them in the charging loop
            if (shotsLeftInAttack > 0)
            {
                SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            }
            //otherwise revert them to the idle postion
            else
            {
                SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
                currentAttackPhase = AttackPhasesType.End;
            }
            return;
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            if (shotsLeftInAttack == 0)
            {
                Attacking = false;
            }
        }

        if (completedAnim.Contains(CharacterAnimationStateType.Arriving.ToString()))
        {
            CharArrivedOnBattleField();
        }


        if (completedAnim != CharacterAnimationStateType.Idle.ToString() && !SpineAnim.Loop)
        {
            SetAnimation(CharacterAnimationStateType.Idle.ToString(), true);
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
        }
    }

    public override void AttackedTiles(BattleTileScript bts)
    {
        AttackedTilesList.Add(bts);

    }

    public override void CastAttackParticles()
    {
        GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Cast, CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
          SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position, UMS.Side, nextAttack.AttackInput);

        cast.GetComponent<DisableParticleScript>().SetSimulationSpeed(CharInfo.BaseSpeed);
        if(nextAttack.AttackInput == AttackInputType.Skill1)
        {
            foreach (VFXOffsetToTargetVOL item in cast.GetComponentsInChildren<VFXOffsetToTargetVOL>())
            {
                item.Target = AttackedTilesList.First().transform;
            }
        }
    }

}
