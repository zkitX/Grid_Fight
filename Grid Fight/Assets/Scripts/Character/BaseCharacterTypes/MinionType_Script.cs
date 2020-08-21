using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//||
public class MinionType_Script : BaseCharacter
{
    protected bool MoveCoOn = true;
  


    protected bool UnderAttack
    {
        get
        {
            if (Time.time > LastAttackTime + 10f)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    protected bool AIMove = false;


    public override bool EndAxisMovement
    {
        get
        {
            return true;
        }
        set
        {

        }
    }


    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
        CurrentPlayerController = ControllerType.Enemy;
        shotsLeftInAttack = 0;
    }

    public override void SetUpLeavingBattle()
    {
     
    }
    public override void SetAttackReady(bool value)
    {
        if (value)
        {
            StartAI();

        }
        base.SetAttackReady(value);
    }

    public override void SetCharDead()
    {
        if (died) return;

        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
        if(AICo != null)
        {
            StopCoroutine(AICo);
            AICo = null;
        }
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        Attacking = false;
        CurrentAIState?.ResetStats(CharInfo);
        BuffsDebuffsList.ForEach(r =>
        {
            r.Duration = 0;
            r.CurrentBuffDebuff.Stop_Co = true;
        }
        );
        for (int i = 0; i < HittedByList.Count; i++)
        {
            StatisticInfoClass sic = StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == HittedByList[i].CharacterId).FirstOrDefault();
            if (sic != null)
            {
                sic.BaseExp += (HittedByList[i].Damage / totDamage) * CharInfo.ExperienceValue;
            }
        }
        totDamage = 0;

      
        if (HittedByList.Count > 0)
        {
            ComboManager.Instance.TriggerComboForCharacter(HittedByList[HittedByList.Count - 1].CharacterId, ComboType.Kill, true, transform.position);
        }
        SpineAnim.transform.localPosition = LocalSpinePosoffset;
        SpineAnim.SpineAnimationState.ClearTracks();
        SpineAnim.CurrentAnim = "";
        switch (DeathAnim)
        {
            case DeathAnimType.Explosion:
                for (int i = 0; i < UMS.Pos.Count; i++)
                {
                    GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
                    UMS.Pos[i] = Vector2Int.zero;
                }
                transform.position = new Vector3(100, 100, 100);
                SetAnimation(CharacterAnimationStateType.Idle);
                if (isActiveAndEnabled)
                {
                    StartCoroutine(DisableChar());
                }
                break;
            case DeathAnimType.Defeat:
                SetAnimation(CharacterAnimationStateType.Defeat);
                break;
            case DeathAnimType.Reverse_Arrives:
                for (int i = 0; i < UMS.Pos.Count; i++)
                {
                    GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
                    UMS.Pos[i] = Vector2Int.zero;
                }
                SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
                break;
        }
        base.SetCharDead();

    }

    private IEnumerator DisableChar()
    {
        yield return BattleManagerScript.Instance.WaitFor(0.5f);
        SetAttackReady(false);
        gameObject.SetActive(false);

    }
   
    protected override void Update()
    {
        base.Update();
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle && SpineAnim.CurrentAnim.Contains("Defeat"))
        {
            return;
        }

        if (animState == CharacterAnimationStateType.Defending && SpineAnim.CurrentAnim.Contains("Defending"))
        {
            return;
        }
      
        SetAnimation(animState.ToString(), loop, transition);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }

        if (animState.Contains("GettingHit") && SpineAnim.CurrentAnim.Contains("GettingHit"))
        {
            return;
        }

        base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }


    //Basic attack sequence
    public override IEnumerator AttackSequence()
    {
        Attacking = true;
        bulletFired = false;
        string animToFire;
        bool isLooped = false;
   
        if(nextAttack != null)
        {
            isLooped = false;
            animToFire = nextAttack.PrefixAnim + "_IdleToAtk";
            strongAnimDone = false;
            currentAttackPhase = AttackPhasesType.Start;
            SetAnimation(animToFire, isLooped, 0f);

            if (nextAttack.isSequencedAttack && nextSequencedAttacks.Count > 0)
            {
                nextSequencedAttacks.RemoveAt(0);
            }

            while (Attacking && currentAttackPhase != AttackPhasesType.End)
            {
                if(nextAttack == null)
                {

                }
                yield return null;
            }

        }
    }



    //public override void fireAttackAnimation(Vector3 pos)
    //{
    //    if (!SpineAnim.CurrentAnim.Contains("Loop"))
    //    {
    //        if(nextAttack.PrefixAnim != AttackAnimPrefixType.Atk1)
    //        {
    //            if(!strongAnimDone)
    //            {
    //                strongAnimDone = true;
    //                SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
    //            }
    //        }
    //        else
    //        {
    //            SetAnimation(nextAttack.PrefixAnim + "_Loop");
    //        }
    //    }

    //    if (chargeParticles != null && shotsLeftInAttack <= 0)
    //    {
    //        chargeParticles.SetActive(false);

    //        chargeParticles = null;
    //    }
    //}


    //public override void FireAttackAnimAndBullet(Vector3 pos)
    //{
    //    if (nextAttack.AttackInput != AttackInputType.Weak)
    //    {
    //        if (!strongAnimDone)
    //        {
    //            strongAnimDone = true;
    //            SetAnimation(nextAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? nextAttack.PrefixAnim + "_Loop" : nextAttack.PrefixAnim + "_AtkToIdle");
    //        }
    //    }
    //    else
    //    {
    //        SetAnimation(nextAttack.PrefixAnim + "_Loop");
    //    }
    //}

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (isAttackBlocking)
        {
         /*   int rand = UnityEngine.Random.Range(0, 100);

            if (rand <= 200)
            {
                Attacking = false;
                shotsLeftInAttack = 0;
            }*/
        }

        LastAttackTime = Time.time;
        return SetDamage(attacker, damage, elemental, isCritical);
    }


    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical)
    {
        if(attacker != this)
        {
            float defenceChances = Random.Range(0, 100);
            if (defenceChances < CharInfo.ShieldStats.MinionPerfectShieldChances && !SpineAnim.CurrentAnim.Contains("Atk"))
            {
                isDefending = true;
                DefendingHoldingTimer = 0;
                SetAnimation(CharacterAnimationStateType.Defending);
                damage = 0;
            }
            else if (defenceChances < (CharInfo.ShieldStats.MinionPerfectShieldChances + CharInfo.ShieldStats.MinionShieldChances) && !SpineAnim.CurrentAnim.Contains("Atk"))
            {
                isDefending = true;
                DefendingHoldingTimer = 10;
                SetAnimation(CharacterAnimationStateType.Defending);
                damage = damage - CharInfo.ShieldStats.ShieldAbsorbtion;
            }
            else
            {
                isDefending = false;
                DefendingHoldingTimer = 0;
            }
        }
        return base.SetDamage(attacker, damage, elemental, isCritical);
    }



    public override void SetFinalDamage(BaseCharacter attacker, float damage, HitInfoClass hic = null)
    {
        hic = HittedByList.Where(r => r.CharacterId == attacker.CharInfo.CharacterID).FirstOrDefault();
        if (hic == null)
        {
            HittedByList.Add(new HitInfoClass(attacker, damage));
        }
        else
        {
            hic.Damage += damage;
        }
        if(attacker.CurrentPlayerController != ControllerType.None && attacker.CurrentPlayerController != ControllerType.Enemy)
        {
            AggroInfoClass aggro = AggroInfoList.Where(r => r.PlayerController == attacker.CurrentPlayerController).FirstOrDefault();
            if (aggro == null)
            {
                AggroInfoList.Add(new AggroInfoClass(attacker.CurrentPlayerController, 1));
            }
            else
            {
                aggro.Hit++;
                AggroInfoList.ForEach(r =>
                {
                    if (r.PlayerController != attacker.CurrentPlayerController)
                    {
                        r.Hit = r.Hit == 0 ? 0 : r.Hit - 1;
                    }
                });
            }
        }
        
        attacker.Sic.DamageMade += damage;
        totDamage += damage;
        base.SetFinalDamage(attacker, damage, hic);
    }


    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();
        UMS.SetUnit(UnitBehaviourType.NPC);
    }

    public override void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        CastLoopImpactAudioClipInfoClass attackTypeAudioInfo = GetAttackAudio();
        if (e.Data.Name.Contains("FireArrivingParticle"))
        {
            ArrivingEvent();
        }
        else if (e.Data.Name.Contains("FireCastParticle"))
        {
            if (attackTypeAudioInfo != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackTypeAudioInfo.Cast, AudioBus.LowPrio, transform);
            }

            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Cast_Weak;
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;

            }
            FireCastParticles();
        }
        else if (e.Data.Name.Contains("FireBulletParticle"))
        {
            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Cast_Weak;
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;
                    
            }
            BulletAttack();
        }
        else if (e.Data.Name.Contains("FireTileAttack") && !trackEntry.Animation.Name.Contains("Loop"))
        {
            currentAttackPhase = SpineAnim.CurrentAnim.Contains("Atk1") ? AttackPhasesType.Bullet_Weak : AttackPhasesType.Bullet_Strong;
            CreateTileAttack();
        }
    }


    public void BulletAttack()
    {
        bulletFired = true;
        if (nextAttack.AttackInput == AttackInputType.Strong)
        {
            CreateBullet(nextAttack.TilesAtk.BulletTrajectories[0]);
        }
        else if(nextAttack.AttackInput == AttackInputType.Weak)
        {
            Debug.Log(nextAttack.TilesAtk.BulletTrajectories.Count - shotsLeftInAttack - 1);
            CreateBullet(nextAttack.TilesAtk.BulletTrajectories[0]);
        }
    }

    public override void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "<empty>" 
         || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString() )
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;

        if (completedAnim == CharacterAnimationStateType.Defeat.ToString())
        {
            // SpineAnim.SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Defeat.ToString() + "_Loop", true);
            //SpineAnim.CurrentAnim = CharacterAnimationStateType.Defeat.ToString() + "_Loop";
            return;
        }

        if (completedAnim == CharacterAnimationStateType.Defending.ToString())
        {
            isDefending = false;
            isDefendingStop = true;
            DefendingHoldingTimer = 0;
        }

        if (completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString() || completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString())
        {
            transform.position = new Vector3(100, 100, 100);
            SpineAnim.SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
            SetAttackReady(false);
            gameObject.SetActive(false);
            return;
        }

        if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            if (shotsLeftInAttack > 0)
            {
                SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
                return;
            }
            else
            {
                Attacking = false;
            }
        }

        if (completedAnim.Contains("_Loop") && SpineAnim.CurrentAnim.Contains("_Loop"))
        {
            if(nextAttack != null)
            {
                SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
                return;
            }
            else
            {

            }
            
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            Attacking = false;
        }

        base.SpineAnimationState_Complete(trackEntry);
    }

}

[System.Serializable]
public class HitInfoClass
{
    public BaseCharacter hitter = null;
    public CharacterNameType CharacterId;
    public float Damage;
    public float TimeLastHit = 0;

    public HitInfoClass()
    {
        TimeLastHit = Time.time;
    }

    public HitInfoClass(BaseCharacter character, float damage)
    {
        hitter = character;
        CharacterId = character.CharInfo.CharacterID;
        Damage = damage;
        TimeLastHit = Time.time;
    }

    public void UpdateLastHitTime()
    {
        TimeLastHit = Time.time;
    }
}

[System.Serializable]
public class AggroInfoClass
{
    public ControllerType PlayerController;
    public int Hit;

    public AggroInfoClass()
    {

    }

    public AggroInfoClass(ControllerType playerController, int hit)
    {
        PlayerController = playerController;
        Hit = hit;
    }
}
