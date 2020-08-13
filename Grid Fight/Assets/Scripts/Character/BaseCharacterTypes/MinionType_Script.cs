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
        if (!SpineAnim.CurrentAnim.Contains("rriv"))
        {
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
        }
        base.SetCharDead();

    }
    private IEnumerator DisableChar()
    {
        yield return BattleManagerScript.Instance.WaitFor(0.5f);
        SetAttackReady(false);
        gameObject.SetActive(false);

    }
   
    public virtual IEnumerator Move()
    {
        while (true)
        {
            if (MoveCoOn && currentAttackPhase == AttackPhasesType.End && !Attacking)
            {
                float timer = 0;
                float MoveTime = Random.Range(CharInfo.MovementTimer.x, CharInfo.MovementTimer.y) / 3;
                while (timer < MoveTime && !AIMove)
                {
                    yield return null;
                    while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || Attacking)
                    {
                        yield return null;
                    }
                    // Debug.Log(timer + "    " + MoveTime);
                    timer += BattleManagerScript.Instance.DeltaTime;
                }
                AIMove = false;
                if (CharInfo.Health > 0)
                {
                    while (currentAttackPhase != AttackPhasesType.End)
                    {
                        yield return null;
                    }
                    InputDirectionType dir = InputDirectionType.Up;

                    foreach (var item in BattleManagerScript.Instance.AllCharactersOnField.Where(a => a.IsOnField).OrderBy(r => Mathf.Abs(r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x)))
                    {
                        dir = item.UMS.CurrentTilePos.x > UMS.CurrentTilePos.x ? InputDirectionType.Down : InputDirectionType.Up;
                        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos + GridManagerScript.Instance.GetVectorFromDirection(dir));
                        if (bts != null && bts.BattleTileState == BattleTileStateType.Empty)
                        {
                            break;
                        }
                        else
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                bts = GridManagerScript.Instance.GetBattleTile(UMS.CurrentTilePos + GridManagerScript.Instance.GetVectorFromDirection((InputDirectionType)1 + i));
                                if (bts != null && bts.BattleTileState == BattleTileStateType.Empty)
                                {
                                    break;
                                }
                            }
                            if (bts != null && bts.BattleTileState == BattleTileStateType.Empty)
                            {
                                break;
                            }
                        }
                    }
                    MoveCharOnDirection(dir);
                }
                else
                {
                    timer = 0;
                }
            }
            yield return null;
        }
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

        if (animState == CharacterAnimationStateType.GettingHit && SpineAnim.CurrentAnim.Contains("GettingHit"))
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

            if (nextAttack.isSequencedAttack && nextSequencedAttacks.Count > 0) nextSequencedAttacks.RemoveAt(0);

            while (Attacking)
            {
                if(nextAttack == null)
                {

                }
                yield return null;
            }

        }
    }

    public override void GetAttack()
    {
        if (nextSequencedAttacks.Count > 0)
        {
            nextAttack = nextSequencedAttacks[0];
            nextAttack.isSequencedAttack = true;
            return;
        }

        ScriptableObjectAttackBase[] nextAttackSequence = CharInfo.NextAttackSequence;
        if (nextAttackSequence != null)
        {
            nextSequencedAttacks = nextAttackSequence.ToList();
            GetAttack();
            return;
        }

        currentTileAtks = CharInfo.CurrentAttackTypeInfo.Where(r => r != null && r.CurrentAttackType == AttackType.Tile).ToList();
        availableAtks.Clear();
        for (int i = 0; i < currentTileAtks.Count; i++)
        {
            atkToCheck = currentTileAtks[i];
            switch (atkToCheck.TilesAtk.StatToCheck)
            {
                case StatsCheckType.Health:
                    switch (atkToCheck.TilesAtk.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            if (CharInfo.HealthPerc < atkToCheck.TilesAtk.PercToCheck)
                            {

                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.EqualTo:
                            if (CharInfo.HealthPerc == atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.MoreThan:
                            if (CharInfo.HealthPerc > atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.Between:
                            if (CharInfo.HealthPerc <= atkToCheck.TilesAtk.InBetween.x && CharInfo.HealthPerc >= atkToCheck.TilesAtk.InBetween.y)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                    }
                    break;
                case StatsCheckType.Ether:
                    switch (atkToCheck.TilesAtk.ValueChecker)
                    {
                        case ValueCheckerType.LessThan:
                            if (CharInfo.EtherPerc < atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.EqualTo:
                            if (CharInfo.EtherPerc == atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.MoreThan:
                            if (CharInfo.EtherPerc > atkToCheck.TilesAtk.PercToCheck)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                        case ValueCheckerType.Between:
                            if (CharInfo.EtherPerc <= atkToCheck.TilesAtk.InBetween.x && CharInfo.EtherPerc >= atkToCheck.TilesAtk.InBetween.y)
                            {
                                availableAtks.Add(atkToCheck);
                            }
                            break;
                    }
                    break;
                case StatsCheckType.None:
                    availableAtks.Add(atkToCheck);
                    break;
            }
        }

        int totalchances = 0;

        List<ScriptableObjectAttackBase> resAtkBase = new List<ScriptableObjectAttackBase>();
        availableAtks.ForEach(r =>
        {
            switch (r.Fov)
            {
                case FieldOfViewType.NearRange:
                    if(target.UMS.CurrentTilePos.x == UMS.CurrentTilePos.x)
                    {
                        resAtkBase.Add(r);
                    }
                    break;
                case FieldOfViewType.MidRange:
                    if (Mathf.Abs(target.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x) <= 1)
                    {
                        resAtkBase.Add(r);
                    }
                    break;
                case FieldOfViewType.LongRange:
                    resAtkBase.Add(r);
                    break;
            }
        });

        resAtkBase.ForEach(r =>
        {
            totalchances += r.TilesAtk.Chances;
        });
        int chances = 0;
        int sumc = 0;
        for (int i = 0; i < resAtkBase.Count; i++)
        {
            chances = UnityEngine.Random.Range(0, totalchances);
            sumc += resAtkBase[i].TilesAtk.Chances;

            if (chances < sumc)
            {
                nextAttack = resAtkBase[i];
                return;
            }
            totalchances -= sumc;
        }
    }


    public override void CreateBullet(BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo)
    {
        GameObject bullet = BulletManagerScript.Instance.GetBullet();
        bullet.transform.position = SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position;
        BulletScript bs = bullet.GetComponent<BulletScript>();
        bs.gameObject.SetActive(true);
        bs.SOAttack = nextAttack;
        bs.BulletBehaviourInfo = null; 
        bs.BulletBehaviourInfoTile = bulletBehaviourInfo;
        bs.Facing = UMS.Facing;
        bs.Elemental = CharInfo.DamageStats.CurrentElemental;
        bs.Side = UMS.Side;
        bs.isColliding = false;
        bs.CharOwner = this;
        bs.attackAudioType = GetAttackAudio();
        bs.BulletEffects.Clear();
        bs.DestinationTile = nextAttackPos;
        float duration = bulletBehaviourInfo.BulletTravelDurationPerTile * (float)(Mathf.Abs(UMS.CurrentTilePos.y - nextAttackPos.y));

        bs.BulletDuration = duration > bulletBehaviourInfo.Delay ? bulletBehaviourInfo.Delay - SpineAnim.SpineAnimationState.GetCurrent(0).TrackTime : duration;
        bs.StartMoveToTile();
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


    public override void FireAttackAnimAndBullet(Vector3 pos)
    {
        if (nextAttack.AttackInput != AttackInputType.Weak)
        {
            if (!strongAnimDone)
            {
                strongAnimDone = true;
                SetAnimation(nextAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? nextAttack.PrefixAnim + "_Loop" : nextAttack.PrefixAnim + "_AtkToIdle");
            }
        }
        else
        {
            SetAnimation(nextAttack.PrefixAnim + "_Loop");
        }
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (isAttackBlocking)
        {
            int rand = UnityEngine.Random.Range(0, 100);

            if (rand <= 200)
            {
                Attacking = false;
                shotsLeftInAttack = 0;
            }
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
            SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            return;
        }

        if (completedAnim.Contains("_Loop") && SpineAnim.CurrentAnim.Contains("_Loop"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
            currentAttackPhase = AttackPhasesType.End;
            return;
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
