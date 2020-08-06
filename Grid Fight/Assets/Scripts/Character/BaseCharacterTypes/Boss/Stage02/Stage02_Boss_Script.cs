using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;

public class Stage02_Boss_Script : MinionType_Script
{
    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (Attacking && animState.Contains("Defending")) return;

        if(animState == "S_Buff_IdleToAtk")
        {
            animState = "Paste";
        }
        if(animState == "S_Buff_AtkToIdle")
        {
            return;
        }

        if (animState.Contains("Atk1"))
        {
            animState = animState.Replace("Atk1", "Undo");
        }
        if (animState.Contains("Atk2"))
        {
            animState = animState.Replace("Atk2", "Copy");
        }
        if (animState.Contains("Atk3"))
        {
            animState = animState.Replace("Atk3", "Redo");
        }

        base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }

    public override void SpineAnimationState_Complete(TrackEntry trackEntry)
    {
        string completedAnim = trackEntry.Animation.Name;
        if (completedAnim.Contains("Paste"))
        {
            currentAttackPhase = AttackPhasesType.End;
            Attacking = false;
        }
        base.SpineAnimationState_Complete(trackEntry);
    }

    public override string GetAttackAnimName()
    {
        return "Undo_Loop";
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (attacker == this && damage == 0f) return true;
        return base.SetDamage(attacker, damage, elemental, isCritical, isAttackBlocking);
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical)
    {
        if (attacker == this && damage == 0f) return true;
        return base.SetDamage(attacker, damage, elemental, isCritical);
    }

    public override void fireAttackAnimation(Vector3 pos)
    {
          if (!SpineAnim.CurrentAnim.Contains("Loop"))
          {
              if(nextAttack.PrefixAnim != AttackAnimPrefixType.Atk1)
              {
                  if(!strongAnimDone)
                  {
                      strongAnimDone = true;
                      SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
                  }
              }
              else
              {
                  SetAnimation(nextAttack.PrefixAnim + "_Loop");
              }
          }

          if (chargeParticles != null && shotsLeftInAttack <= 0)
          {
              chargeParticles.SetActive(false);

              chargeParticles = null;
          }
    }
}
