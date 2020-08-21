using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectBaseCharacterBaseAttack : ScriptableObjectBaseCharaterAction
{

    #region WeakAtk
    protected float WeakAttackOffset = 0;
    #endregion


    #region Strong Attack
    public float strongAttackTimer = 0f;
    protected bool isStrongChargingParticlesOn = false;
    protected ManagedAudioSource strongChargeAudio = null;
    protected ManagedAudioSource strongChargeudioStrong = null;
    public bool isSrongLoading
    {
        get
        {
            return isStrongStop ? false : _isStrongLoading;
        }
        set
        {
            _isStrongLoading = value;
        }
    }
    public bool _isStrongLoading = false;
    public bool isStrongStop = false;
    protected GameObject strongChargePs = null;
    #endregion


    #region Phase
    public int shotsLeftInAttack
    {
        get
        {
            return _shotsLeftInAttack;
        }
        set
        {
            _shotsLeftInAttack = value;
            _shotsLeftInAttack = _shotsLeftInAttack < 0 ? 0 : _shotsLeftInAttack;
            if (_shotsLeftInAttack == 0)
            {
                Attacking = false;
            }
        }
    }
    public int _shotsLeftInAttack = 0;
    public virtual bool Attacking
    {
        get
        {
            return shotsLeftInAttack > 0 ? true : _Attacking;
        }
        set
        {
            _Attacking = value;
        }
    }
    public bool _Attacking = false;
    [HideInInspector] public bool bulletFired = false;
    public AttackPhasesType currentAttackPhase = AttackPhasesType.End;
    #endregion


    #region Temp
    [HideInInspector] public List<ScriptableObjectAttackBase> availableAtks = new List<ScriptableObjectAttackBase>();
    [HideInInspector] public List<ScriptableObjectAttackBase> currentTileAtks = new List<ScriptableObjectAttackBase>();
    [HideInInspector] public ScriptableObjectAttackBase atkToCheck;
    [HideInInspector] public BulletScript bullet = null;
    #endregion



    public virtual IEnumerator Attack()
    {
        yield break;
    }

    public virtual IEnumerator StartStrongAttack()
    {
        yield break;
    }


    public virtual IEnumerator StartStrongAttack_Co()
    {
        yield break;
    }

    public virtual void StrongAttack(ScriptableObjectAttackBase atkType)
    {

    }

    public virtual void StartWeakAttack(bool attackRegardless)
    {

    }


    public virtual void WeakAttack()
    {
       
    }

    public virtual void ChargingLoop()
    {
        
    }

    public virtual void CastAttackParticles()
    {
        if (CharOwner.nextAttack == null) return;

        tempGameObject = ParticleManagerScript.Instance.FireParticlesInPosition(CharOwner.UMS.Side == SideType.LeftSide ? CharOwner.nextAttack.Particles.Left.Cast : CharOwner.nextAttack.Particles.Right.Cast, CharOwner.CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
        CharOwner.SpineAnim.FiringPints[(int)CharOwner.nextAttack.AttackAnim].position, CharOwner.UMS.Side, CharOwner.nextAttack.AttackInput);
        tempGameObject.GetComponent<ParticleHelperScript>().SetSimulationSpeed(CharOwner.CharInfo.BaseSpeed);

        if (CharOwner.nextAttack.CurrentAttackType == AttackType.Particles)
        {
            CharOwner.CharInfo.Ether -= CharOwner.nextAttack.StaminaCost;
            EventManager.Instance?.UpdateStamina(CharOwner);

            if (CharOwner.nextAttack.AttackInput > AttackInputType.Weak)
            {
                CameraManagerScript.Instance.CameraShake(CameraShakeType.Powerfulattack);
            }
        }
    }

    public virtual void CreateAttack()
    {
        if (CharOwner.nextAttack == null) return;

        CreateTotemAttack();
    }

    public virtual void CreateParticleBullet(BulletBehaviourInfoClass bulletBehaviourInfo)
    {
    }

    public virtual void CreateTileBullet(BulletBehaviourInfoClassOnBattleFieldClass bulletBehaviourInfo)
    {
    }

    public void CompleteBulletSetup()
    {
        if (CharOwner.HasBuffDebuff(BuffDebuffStatsType.Backfire))
        {
            CharOwner.BackfireEffect(CharOwner.NextAttackDamage);
            return;
        }

        bullet.transform.position = CharOwner.SpineAnim.FiringPints[(int)CharOwner.nextAttack.AttackAnim].position;
        bullet.SOAttack = CharOwner.nextAttack;
        bullet.Facing = CharOwner.UMS.Facing;
        bullet.Elemental = CharOwner.CharInfo.DamageStats.CurrentElemental;
        bullet.Side = CharOwner.UMS.Side;
        bullet.CharOwner = CharOwner;
        bullet.attackAudioType = CharOwner.GetAttackAudio();
        bullet.gameObject.SetActive(true);
        bullet.StartMoveToTile();
    }

    public void CreateTotemAttack()
    {
        if (CharOwner.nextAttack != null && CharOwner.nextAttack.CurrentAttackType == AttackType.Totem && CharOwner.CharInfo.Health > 0 && CharOwner.IsOnField)
        {
            CharOwner.CharInfo.WeakAttack.DamageMultiplier = CharOwner.CharInfo.WeakAttack.B_DamageMultiplier * CharOwner.nextAttack.DamageMultiplier;
            CharOwner.CharInfo.StrongAttack.DamageMultiplier = CharOwner.CharInfo.StrongAttack.B_DamageMultiplier * CharOwner.nextAttack.DamageMultiplier;
            CharOwner.StartCoroutine(Totem());
        }
    }



    //TODO to rework
    IEnumerator Totem()
    {
        yield return BattleManagerScript.Instance.WaitUpdate(() => (currentAttackPhase != AttackPhasesType.End || CharOwner.CharInfo.HealthPerc <= 0));
        BattleTileScript res;
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        SideType side = nextAttack.TotemAtk.IsPlayerSide ? UMS.Side : UMS.Side == SideType.LeftSide ? SideType.RightSide : SideType.LeftSide;
        res = GridManagerScript.Instance.GetFreeBattleTile(side == SideType.LeftSide ? WalkingSideType.LeftSide : WalkingSideType.RightSide);
        res.SetupEffect(nextAttack.TotemAtk.Effects, nextAttack.TotemAtk.DurationOnField, nextAttack.TotemAtk.TotemIn);
        List<TotemTentacleClass> tentacles = new List<TotemTentacleClass>();
        TotemTentacleClass checker;
        GameObject ps = null;
        if (nextAttack.TotemAtk.TentaclePrefab != ParticlesType.None)
        {
            float timer = 0;
            while (timer < nextAttack.TotemAtk.DurationOnField)
            {
                foreach (TotemTentacleClass item in tentacles)
                {
                    item.isActive = false;
                }
                yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
                timer += BattleManagerScript.Instance.DeltaTime;
                List<BaseCharacter> enemy = (matchType == MatchType.PPPPvE || matchType == MatchType.PPvE || matchType == MatchType.PvE) ?
                WaveManagerScript.Instance.WaveCharcters.Where(r => r.IsOnField && r.gameObject.activeInHierarchy).ToList() :
                BattleManagerScript.Instance.GetAllPlayerActiveChars().Where(r => r.UMS.Side == side).ToList();

                foreach (BaseCharacter item in enemy)
                {
                    checker = tentacles.Where(r => r.CharAffected == item).FirstOrDefault();
                    if (checker != null)
                    {
                        checker.isActive = true;
                    }
                    else
                    {
                        if (nextAttack.TotemAtk.TentaclePrefab != ParticlesType.None)
                        {
                            ps = ParticleManagerScript.Instance.GetParticle(nextAttack.TotemAtk.TentaclePrefab);
                            ps.transform.position = res.transform.position;
                            ps.SetActive(true);
                            foreach (VFXOffsetToTargetVOL pstimeG in ps.GetComponentsInChildren<VFXOffsetToTargetVOL>())
                            {
                                pstimeG.Target = item.CharInfo.Head;
                            }
                        }

                        foreach (ScriptableObjectAttackEffect effect in nextAttack.TotemAtk.Effects.Where(r => r.StatsToAffect != BuffDebuffStatsType.BlockTile).ToList())
                        {
                            item.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(), ElementalType.Dark, this, effect));
                        }


                        foreach (ScriptableObjectAttackEffect effect in nextAttack.TotemAtk.Effects.Where(r => r.StatsToAffect == BuffDebuffStatsType.BlockTile).ToList())
                        {
                            res.BlockTileForTime(effect.Duration, ParticleManagerScript.Instance.GetParticle(effect.Particles));
                        }


                        tentacles.Add(new TotemTentacleClass(item, ps, true));
                    }
                }

                foreach (TotemTentacleClass item in tentacles.Where(r => !r.isActive).ToList())
                {
                    item.PS.gameObject.SetActive(false);
                    tentacles.Remove(item);
                }
            }
        }
    }

}
