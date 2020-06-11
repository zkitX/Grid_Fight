using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTileTargetsScript : MonoBehaviour
{
    public List<Vector3> TargetsPosition = new List<Vector3>();
    public List<TargetClass> Targets = new List<TargetClass>();
    public Transform Whiteline;


    private void Awake()
    {
        Whiteline = transform.GetChild(0);
    }
    public void SetAttack(float duration, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances)
    {
        GameObject nextT = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Tile);

        nextT.SetActive(true);
        TargetClass tc = new TargetClass(duration, nextT);
        nextT.transform.parent = transform;
        nextT.transform.localPosition = TargetsPosition[0];
        Targets.Add(tc);
        UpdateQueue();
        StartCoroutine(FireTarget(tc, pos, damage, ele, attacker, atkEffects, effectChances));
    }

    private IEnumerator FireTarget(TargetClass tc, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances)
    {
        float timer = 0;
        Whiteline.gameObject.SetActive(true);
        float duration = tc.Duration;
        bool attackerFiredAttackAnim = false;
        Animator anim = tc.TargetIndicator.GetComponent<Animator>();
        anim.speed = 1 / duration;
        ScriptableObjectAttackBase nextAttack = attacker.nextAttack;
        while (timer < duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance != null && ((BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets) && !BattleManagerScript.Instance.VFXScene))
            {
                yield return new WaitForFixedUpdate();
                anim.speed = 0;
            }
            anim.speed = (1 / duration) * (attacker.CharInfo.BaseSpeed / attacker.CharInfo.SpeedStats.B_BaseSpeed);
            timer += Time.fixedDeltaTime * (attacker.CharInfo.BaseSpeed / attacker.CharInfo.SpeedStats.B_BaseSpeed);
            tc.RemainingTime = duration - timer;
            if (!attacker.Attacking && tc.RemainingTime > duration * 0.1f)
            {
                //Stop the firing of the attacks to the tiles
                attacker.shotsLeftInAttack = 0;
                tc.RemainingTime = 0f;
                UpdateQueue(tc);
                yield break;
            }
            else if (tc.RemainingTime <= duration * 0.1f && attacker.UMS.CurrentAttackType == AttackType.Tile && !attackerFiredAttackAnim)
            {
                attackerFiredAttackAnim = true;
                attacker.shotsLeftInAttack--;
                attacker.fireAttackAnimation(transform.position); // trigger the shoot anim
            }
        }

        bool effectOn = true;
        BaseCharacter target = null;
        if (BattleManagerScript.Instance != null)
        {
            target = BattleManagerScript.Instance.GetCharInPos(pos);
            if (target != null)
            {
                bool iscritical = attacker.CharInfo.IsCritical(true);
                //Set damage to the hitting character
                float dmg = damage * (iscritical ? 2 : 1);
                effectOn = target.SetDamage(attacker, dmg, ele, iscritical);
                if (effectOn)
                {
                    int chances = Random.Range(0, 100);
                    if (chances < effectChances)
                    {
                        foreach (ScriptableObjectAttackEffect item in atkEffects.Effects.Where(r => !r.StatsToAffect.ToString().Contains("Tile")).ToList())
                        {
                            target.Buff_DebuffCo(new Buff_DebuffClass(item.Name, item.Duration.x, item.StatsToAffect == BuffDebuffStatsType.Damage_Cure ? item.Value *2 : item.Value,
                                item.StatsToAffect, item.StatsChecker, new ElementalResistenceClass(), ElementalType.Dark, item.AnimToFire, item.Particles, attacker));
                        }
                    }
                }
            }
            else
            {

                ScriptableObjectAttackEffect soAE = atkEffects.Effects.Where(r => r.StatsToAffect == BuffDebuffStatsType.BlockTile).FirstOrDefault();
                if (soAE != null)
                {
                    StartCoroutine(BlockTileForTime(soAE.Duration.x, pos, ParticleManagerScript.Instance.GetParticle(soAE.Particles)));
                }
            }
            if(atkEffects.IsEffectOnTile)
            {
                BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
                bts.SetupEffect(atkEffects.EffectsOnTile, atkEffects.DurationOnTile, atkEffects.TileParticlesID);
            }

        }


        if (attacker.CharInfo.Health > 0)
        {
            if (effectOn)
            {
                GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Hit, attacker.CharInfo.CharacterID, AttackParticlePhaseTypes.Hit, transform.position, attacker.UMS.Side, nextAttack.AttackInput);
                foreach (VFXOffsetToTargetVOL item in effect.GetComponentsInChildren<VFXOffsetToTargetVOL>())
                {
                    if (target != null)
                    {
                        item.gameObject.SetActive(true);
                        item.Target = attacker.transform;
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }  
            }
            if (attacker.GetAttackAudio() != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attacker.GetAttackAudio().Impact, AudioBus.MidPrio, transform);
            }
        }
        yield return new WaitForSeconds(0.2f);
        UpdateQueue(tc);
    }


    public void SetAttack(float duration, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances, float bulletTravelDuration)
    {
        GameObject nextT = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Tile);

        nextT.SetActive(true);
        TargetClass tc = new TargetClass(duration, nextT);
        nextT.transform.parent = transform;
        nextT.transform.localPosition = TargetsPosition[0];
        Targets.Add(tc);
        UpdateQueue();
        StartCoroutine(FireTarget_co(tc, pos, damage, ele, attacker, atkEffects, effectChances, bulletTravelDuration));
    }

    private IEnumerator FireTarget_co(TargetClass tc, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances, float bulletTravelDuration)
    {
        yield return FireTarget(tc, pos, damage, ele, attacker, atkEffects, effectChances, bulletTravelDuration);
        tc.RemainingTime = 0f;
        UpdateQueue(tc);
        attacker.shotsLeftInAttack--;
    }

    private IEnumerator FireTarget(TargetClass tc, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances, float bulletTravelDuration)
    {
        float timer = 0;
        Whiteline.gameObject.SetActive(true);
        float duration = tc.Duration;
        bool attackerFiredAttackAnim = false;
        Animator anim = tc.TargetIndicator.GetComponent<Animator>();
        anim.speed = 1 / duration;
        ScriptableObjectAttackBase nextAttack = attacker.nextAttack;
        while (timer < duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance != null && ((BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets) && !BattleManagerScript.Instance.VFXScene))
            {
                yield return new WaitForFixedUpdate();
                anim.speed = 0;
            }
            anim.speed = (1 / duration) * (attacker.CharInfo.BaseSpeed / attacker.CharInfo.SpeedStats.B_BaseSpeed);
            timer += Time.fixedDeltaTime * (attacker.CharInfo.BaseSpeed / attacker.CharInfo.SpeedStats.B_BaseSpeed);
            tc.RemainingTime = duration - timer;
            if (!attacker.Attacking && !attacker.bulletFired)
            {
                //Stop the firing of the attacks to the tiles
                yield break;
            }
            else if (tc.RemainingTime <= bulletTravelDuration && attacker.UMS.CurrentAttackType == AttackType.Tile && !attackerFiredAttackAnim)
            {
                attackerFiredAttackAnim = true;
                attacker.FireAttackAnimAndBullet(transform.position); // trigger the shoot anim
            }
        }

        bool effectOn = true;
        BaseCharacter target = null;
        if (BattleManagerScript.Instance != null)
        {
            target = BattleManagerScript.Instance.GetCharInPos(pos);
            if (target != null)
            {
                bool iscritical = attacker.CharInfo.IsCritical(true);
                //Set damage to the hitting character
                float dmg = damage * (iscritical ? 2 : 1);
                effectOn = target.SetDamage(attacker, dmg, ele, iscritical);
                if (effectOn)
                {
                    int chances = Random.Range(0, 100);
                    if (chances < effectChances)
                    {
                        foreach (ScriptableObjectAttackEffect item in atkEffects.Effects.Where(r => !r.StatsToAffect.ToString().Contains("Tile")).ToList())
                        {
                            target.Buff_DebuffCo(new Buff_DebuffClass(item.Name, item.Duration.x, item.StatsToAffect == BuffDebuffStatsType.Damage_Cure ? item.Value * 2 : item.Value,
                                item.StatsToAffect, item.StatsChecker, new ElementalResistenceClass(), ElementalType.Dark, item.AnimToFire, item.Particles, attacker));
                        }
                    }
                }
            }
            else
            {

                ScriptableObjectAttackEffect soAE = atkEffects.Effects.Where(r => r.StatsToAffect == BuffDebuffStatsType.BlockTile).FirstOrDefault();
                if (soAE != null)
                {
                    StartCoroutine(BlockTileForTime(soAE.Duration.x, pos, ParticleManagerScript.Instance.GetParticle(soAE.Particles)));
                }
            }
            if (atkEffects.IsEffectOnTile)
            {
                BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
                bts.SetupEffect(atkEffects.EffectsOnTile, atkEffects.DurationOnTile, atkEffects.TileParticlesID);
            }
        }

        if (attacker.CharInfo.Health > 0)
        {
            if (effectOn)
            {
                GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Hit, attacker.CharInfo.CharacterID, AttackParticlePhaseTypes.Hit, transform.position, attacker.UMS.Side, nextAttack.AttackInput);
                foreach (VFXOffsetToTargetVOL item in effect.GetComponentsInChildren<VFXOffsetToTargetVOL>())
                {
                    if (target != null)
                    {
                        item.gameObject.SetActive(true);
                        item.Target = attacker.transform;
                    }
                    else
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
            if (attacker.GetAttackAudio() != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attacker.GetAttackAudio().Impact, AudioBus.MidPrio, transform);
            }
        }
    }


    private IEnumerator BlockTileForTime(float duration, Vector2Int pos, GameObject ps)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
        float timer = 0;
        bts.BattleTileState = BattleTileStateType.Blocked;
        ps.transform.position = transform.position;
        ps.SetActive(true);
        PSTimeGroup pstg = ps.GetComponent<PSTimeGroup>();
        if (pstg != null)
        {
            pstg.UpdatePSTime(duration);
        }
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        ps.SetActive(false);
        bts.BattleTileState = BattleTileStateType.Empty;
    }


    public void UpdateQueue()
    {
        Targets = Targets.OrderByDescending(r => r.RemainingTime).ToList();
        for (int i = 0; i < Targets.Count; i++)
        {

            Targets[i].TargetIndicator.transform.localPosition = TargetsPosition[i];
            return;
        }

    }

    public void UpdateQueue(TargetClass completedTarget)
    {
        completedTarget.TargetIndicator.SetActive(false);
        Targets.Remove(completedTarget);
        UpdateQueue();
        if (Targets.Count == 0)
        {
            Whiteline.gameObject.SetActive(false);
        }
    }

}


public class TargetClass
{
    public float Duration;
    public GameObject TargetIndicator;
    public float RemainingTime;


    public TargetClass(float duration, GameObject targetIndicator)
    {
        Duration = duration;
        TargetIndicator = targetIndicator;
    }
}
