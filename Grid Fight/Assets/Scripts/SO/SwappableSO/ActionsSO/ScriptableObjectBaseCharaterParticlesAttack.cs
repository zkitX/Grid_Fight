using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/ParticlesAttack")]
public class ScriptableObjectBaseCharaterParticlesAttack : ScriptableObjectBaseCharacterBaseAttack
{
    public override IEnumerator Attack()
    {
        yield return base.Attack();
        if (strongChargePs != null)
        {
            strongChargePs.transform.parent = null;
            strongChargePs.SetActive(false);
            strongChargePs = null;
        }
        isStrongStop = false;
        isStrongLoading = false;
        isStrongChargingParticlesOn = false;
        CharOwner.ResetAudioManager();
    }



    public override IEnumerator StartIdleToAtk()
    {
        yield break;
    }
    public override IEnumerator IdleToAtk()
    {
        yield break;
    }
    public override IEnumerator StartCharging()
    {
        yield break;
    }
    public override IEnumerator Charging()
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
        strongAttackTimer += BattleManagerScript.Instance.DeltaTime;
    }
  
    public override IEnumerator StartLoop()
    {
        if (strongAttackTimer < CharOwner.nextAttack.ChargingTime && CharOwner.CharInfo.Health <= 0f)
        {
            shotsLeftInAttack = 0;
        }
        yield return null;
    }
    public override IEnumerator Loop()
    {
        shotsLeftInAttack = isStrongLoading ? 1 : 0;
        yield break;
    }
    public override IEnumerator StartAtkToIdle()
    {
        yield break;
    }
    public override IEnumerator AtkToIdle()
    {
        yield break;
    }








    public override IEnumerator StartStrongAttack_Co()
    {
        if (CharOwner.CanAttack && !isStrongLoading)
        {
            ScriptableObjectAttackBase nxtAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == AttackInputType.Strong).First();
            if (!CharOwner.GetCanUseStamina(nxtAtk.StaminaCost))
            {
                yield break;
            }

            isStrongLoading = true;
            strongAttackTimer = 0;
            currentAttackPhase = AttackPhasesType.Start;
            CharOwner.SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk", false, 0);
            if (strongChargeAudio != null)
            {
                strongChargeAudio.ResetSource();
            }
            if (strongChargeAudioStrong != null)
            {
                strongChargeAudioStrong.ResetSource();
            }
            strongChargeAudio = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingLoop, AudioBus.MidPrio, CharOwner.transform, true, 1f);
            while (isStrongLoading)
            {
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
                strongAttackTimer += BattleManagerScript.Instance.DeltaTime;
                if (strongChargeAudioStrong == null && strongAttackTimer >= 1.5f)
                {
                    strongChargeAudioStrong = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingLoopStrong, AudioBus.MidPrio, CharOwner.transform, true, 1f);
                }

                if (CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString())
                {
                    CharOwner.SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk");
                }
                if (!isStrongChargingParticlesOn || strongChargePs == null)
                {
                    isStrongChargingParticlesOn = true;
                    //Check
                    strongChargePs = ParticleManagerScript.Instance.FireParticlesInTransform(nxtAtk.Particles.CastLoopPS, CharOwner.CharInfo.CharacterID, AttackParticlePhaseTypes.Charging, CharOwner.SpineAnim.transform, CharOwner.UMS.Side, nxtAtk.AttackInput);
                }
                else
                {
                    ParticleManagerScript.Instance.SetParticlesLayer(strongChargePs, CharOwner.CharOredrInLayer);
                }

                if (!CharOwner.IsOnField)
                {
                    yield break;
                }
            }
            if (strongAttackTimer > 1f && CharOwner.CharInfo.Health > 0f)
            {
                currentAttackPhase = AttackPhasesType.Charging;
                if (CharOwner.IsOnField)
                {
                    while (CharOwner.isMoving)
                    {
                        yield return null;
                    }
                    StrongAttack(nxtAtk);
                }
            }
            else
            {
                CharOwner.SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
            }
        }
    }

    public override void StartWeakAttack(bool attackRegardless)
    {
        if (!CharOwner.CharActionlist.Contains(CharacterActionType.WeakAttack))
        {
            return;
        }
        if (CharOwner.CanAttack || attackRegardless)
        {


            ScriptableObjectAttackBase nxtAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == AttackInputType.Weak).First();
            if (!CharOwner.GetCanUseStamina(nxtAtk.StaminaCost))
            {
                return;
            }
            Attacking = true;
            WeakAttackOffset = shotsLeftInAttack > 0 ? WeakAttackOffset : Time.time;
            //lastAttack = false;
            CharOwner.FireActionEvent(CharacterActionType.WeakAttack);
            if (CharOwner.SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_Loop.ToString() && CharOwner.SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
            {
                //Debug.Log(SpineAnim.CurrentAnim);
                CharOwner.SetAnimation(CharacterAnimationStateType.Atk1_IdleToAtk);
                CharOwner.SpineAnim.SetAnimationSpeed(CharOwner.SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk2_IdleToAtk) / CharOwner.CharInfo.SpeedStats.IdleToAtkDuration);
            }
            else if (CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop.ToString())
            {
                shotsLeftInAttack = 1;
            }
        }
    }

    public override void StrongAttack(ScriptableObjectAttackBase atkType)
    {
        CharOwner.nextAttack = atkType;

        if (strongChargeAudio != null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingRelease, AudioBus.LowPrio, CharOwner.transform);
        }
        CharOwner.FireActionEvent(CharacterActionType.StrongAttack);
        CharOwner.SetAnimation(CharOwner.nextAttack.PrefixAnim + "_AtkToIdle");

        ParticleManagerScript.Instance.FireParticlesInPosition(CharOwner.nextAttack.Particles.CastActivationPS, CharOwner.CharInfo.CharacterID, AttackParticlePhaseTypes.CastActivation, CharOwner.transform.position, CharOwner.UMS.Side, CharOwner.nextAttack.AttackInput);
    }

    public override void WeakAttack()
    {
        if (!CharOwner.GetCanUseStamina(CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == AttackInputType.Weak).First().StaminaCost))
        {
            CharOwner.SetAnimation(CharOwner.nextAttack.PrefixAnim + "_AtkToIdle");
            shotsLeftInAttack = 0;
            return;
        }
        CharOwner.nextAttack = CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackAnim == AttackAnimType.Weak_Atk).First();
        currentAttackPhase = AttackPhasesType.Start;
        if (CharOwner.SpineAnim.CurrentAnim.Contains("Loop"))
        {
            CharOwner.SpineAnim.skeletonAnimation.state.GetCurrent(0).TrackTime = 0;
        }
        else
        {
            CharOwner.SetAnimation(CharacterAnimationStateType.Atk1_Loop);
        }
    }

    public override bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {

        if (!animState.ToString().Contains("Atk") && !animState.ToString().Contains("S_DeBuff") && !animState.ToString().Contains("S_Buff"))
        {
            currentAttackPhase = AttackPhasesType.End;
            shotsLeftInAttack = 0;
        }

        if ((string.Equals(animState, CharacterAnimationStateType.GettingHit.ToString()) ||
           string.Equals(animState, CharacterAnimationStateType.Buff.ToString()) ||
           string.Equals(animState, CharacterAnimationStateType.Debuff.ToString())) && (currentAttackPhase != AttackPhasesType.End || Attacking))
        {
            return true;
        }

        return base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }

    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if (completedAnim.Contains("IdleToAtk") && CharOwner.SpineAnim.CurrentAnim.ToString().Contains("IdleToAtk"))
        {
            currentAttackPhase = AttackPhasesType.Charging;
            return true;
        }

        if (completedAnim == CharacterAnimationStateType.Atk1_Loop.ToString() &&
          CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            return true;
        }

        if (completedAnim.Contains("AtkToIdle"))
        {
            currentAttackPhase = AttackPhasesType.End;
            Attacking = false;
        }

        return base.SpineAnimationState_Complete(completedAnim);
    }

    public override void CreateAttack(Vector2Int nextAttackPos)
    {
        if (CharOwner.nextAttack == null) return;

        if (CharOwner.nextAttack.CurrentAttackType == AttackType.Totem)
        {
            base.CreateAttack(nextAttackPos);
            return;
        }

        CreateParticleAttack();
    }

    public void CreateParticleAttack()
    {
        foreach (BulletBehaviourInfoClass item in CharOwner.nextAttack.ParticlesAtk.BulletTrajectories)
        {
            CreateParticleBullet(item);
        }
    }

    public override void CreateParticleBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
        bullet = BulletManagerScript.Instance.GetBullet().GetComponent<BulletScript>();
        bullet.bts = GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(new Vector2Int(CharOwner.UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x, CharOwner.UMS.CurrentTilePos.y - Mathf.Clamp(bulletBehaviourInfo.BulletDistanceInTile.y, 0, 11)
        + ((CharOwner.UMS.Facing == FacingType.Right ? 1 : -1) * bulletBehaviourInfo.BulletDistanceInTile.y)), CharOwner.UMS.Facing);
        if (bullet.bts == null)
        {
            bullet.gameObject.SetActive(false);
            return;
        }


        bullet.isColliding = true;
        bullet.DestinationTile = bullet.bts.Pos;
        bullet.BulletBehaviourInfo = bulletBehaviourInfo;
        ScriptableObjectAttackEffect[] abAtkBase = new ScriptableObjectAttackEffect[bulletBehaviourInfo.Effects.Count];
        bulletBehaviourInfo.Effects.CopyTo(abAtkBase);
        bullet.BulletEffects = abAtkBase.ToList();

        CompleteBulletSetup();
    }





    public override void Reset()
    {
        isStrongLoading = false;
        isStrongStop = false;
        strongAttackTimer = 0f;
        base.Reset();
    }


}


