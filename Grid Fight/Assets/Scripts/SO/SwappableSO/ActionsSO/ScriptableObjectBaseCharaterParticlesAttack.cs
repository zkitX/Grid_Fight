using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/ParticlesAttack")]
public class ScriptableObjectBaseCharaterParticlesAttack : ScriptableObjectBaseCharacterBaseAttack
{
    public override IEnumerator StartStrongAttack()
    {
        yield return StartStrongAttack_Co();
        if (strongChargePs != null)
        {
            strongChargePs.transform.parent = null;
            strongChargePs.SetActive(false);
            strongChargePs = null;
        }
        isStrongStop = false;
        isSrongLoading = false;
        isStrongChargingParticlesOn = false;
        CharOwner.ResetAudioManager();
    }


    public override IEnumerator StartStrongAttack_Co()
    {
        if (CharOwner.CanAttack && !isSrongLoading)
        {
            ScriptableObjectAttackBase nxtAtk = CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == AttackInputType.Strong).First();
            if (!CharOwner.GetCanUseStamina(nxtAtk.StaminaCost))
            {
                yield break;
            }

            isSrongLoading = true;
            strongAttackTimer = 0;
            currentAttackPhase = AttackPhasesType.Start;
            CharOwner.SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk", false, 0);
            if (strongChargeAudio != null)
            {
                strongChargeAudio.ResetSource();
            }
            if (strongChargeudioStrong != null)
            {
                strongChargeudioStrong.ResetSource();
            }
            strongChargeAudio = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingLoop, AudioBus.MidPrio, CharOwner.transform, true, 1f);
            while (isSrongLoading)
            {
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
                strongAttackTimer += BattleManagerScript.Instance.DeltaTime;
                if (strongChargeudioStrong == null && strongAttackTimer >= 1.5f)
                {
                    strongChargeudioStrong = AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingLoopStrong, AudioBus.MidPrio, CharOwner.transform, true, 1f);
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
                currentAttackPhase = AttackPhasesType.Loading;
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
        nextAttack = atkType;

        if (strongChargeAudio != null)
        {
            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.SpecialAttackChargingRelease, AudioBus.LowPrio, transform);
        }
        CharOwner.FireActionEvent(CharacterActionType.StrongAttack);
        CharOwner.SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");

        ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.CastActivationPS, CharInfo.CharacterID, AttackParticlePhaseTypes.CastActivation, transform.position, UMS.Side, nextAttack.AttackInput);
    }


    public override void WeakAttack()
    {
        if (!CharOwner.GetCanUseStamina(CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackInput == AttackInputType.Weak).First().StaminaCost))
        {
            CharOwner.SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
            shotsLeftInAttack = 0;
            return;
        }
        nextAttack = CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r.AttackAnim == AttackAnimType.Weak_Atk).First();
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


    public override void ChargingLoop()
    {
        CharOwner.SetAnimation(nextAttack.PrefixAnim + "_Charging", true);
    }


    public override void SpineAnimationState_Complete()
    {

    }

    public override void CreateAttack()
    {
        if (nextAttack == null) return;

        if (nextAttack.CurrentAttackType == AttackType.Totem)
        {
            base.CreateAttack();
            return;
        }

        CreateParticleAttack();
    }

    public void CreateParticleAttack()
    {
        foreach (BulletBehaviourInfoClass item in nextAttack.ParticlesAtk.BulletTrajectories)
        {
            CreateParticleBullet(item);
        }
    }

    public override void CreateParticleBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
        bullet = BulletManagerScript.Instance.GetBullet().GetComponent<BulletScript>();
        bullet.bts = GridManagerScript.Instance.GetBattleBestTileInsideTheBattlefield(new Vector2Int(UMS.CurrentTilePos.x + bulletBehaviourInfo.BulletDistanceInTile.x, UMS.CurrentTilePos.y - Mathf.Clamp(bulletBehaviourInfo.BulletDistanceInTile.y, 0, 11),
            UMS.CurrentTilePos.y + ((UMS.Facing == FacingType.Right ? 1 : -1) * bulletBehaviourInfo.BulletDistanceInTile.y)), UMS.Facing);
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
}


