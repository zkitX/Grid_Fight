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

    public void SetAttack(float duration, Vector2Int pos, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances, float bulletTravelDuration)
    {
        GameObject nextT = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Tile);

        nextT.SetActive(true);
        TargetClass tc = new TargetClass(duration, nextT);
        nextT.transform.parent = transform;
        nextT.transform.localPosition = TargetsPosition[0];
        Targets.Add(tc);
        UpdateQueue();
        StartCoroutine(FireTarget_co(tc, pos, ele, attacker, atkEffects, effectChances, bulletTravelDuration));
    }

    private IEnumerator FireTarget_co(TargetClass tc, Vector2Int pos, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances, float bulletTravelDuration)
    {
        yield return FireTarget(tc, pos, ele, attacker, atkEffects, effectChances, bulletTravelDuration);
        tc.RemainingTime = 0f;
        UpdateQueue(tc);
    }

    private IEnumerator FireTarget(TargetClass tc, Vector2Int pos, ElementalType ele, BaseCharacter attacker, BattleFieldAttackTileClass atkEffects, float effectChances, float bulletTravelDuration)
    {
        float timer = 0;
        Whiteline.gameObject.SetActive(true);
        float duration = tc.Duration;
        bool attackerFiredAttackAnim = false;
        Animator anim = tc.TargetIndicator.GetComponent<Animator>();
        anim.speed = 1 / duration;
        float damage = attacker.NextAttackTileDamage;
        ScriptableObjectAttackBase nextAttack = attacker.nextAttack;
        while (timer < duration)
        {

            yield return BattleManagerScript.Instance.WaitFixedUpdate(new System.Action(() => { anim.speed = 0; }), () => BattleManagerScript.Instance != null && ((BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets) && !BattleManagerScript.Instance.VFXScene));

            anim.speed = (1 / duration) * (attacker.CharInfo.BaseSpeed / attacker.CharInfo.SpeedStats.B_BaseSpeed) * BattleManagerScript.Instance.BattleSpeed;
            timer += attackerFiredAttackAnim ? BattleManagerScript.Instance.FixedDeltaTime : BattleManagerScript.Instance.FixedDeltaTime * (attacker.CharInfo.BaseSpeed / attacker.CharInfo.SpeedStats.B_BaseSpeed);
            tc.RemainingTime = duration - timer;
            if (!attacker.currentAttackProfile.Attacking && attacker.currentAttackProfile.currentAttackPhase < AttackPhasesType.Firing)
            {
                //Stop the firing of the attacks to the tiles
                yield break;
            }
        }

        bool effectOn = true;
        BaseCharacter target = null;
        if (BattleManagerScript.Instance != null)
        {
            target = BattleManagerScript.Instance.GetCharInPos(pos);
            if (target != null)
            {
               // if ((attacker.nextAttack.AttackInput >= AttackInputType.Strong ? Random.Range(attacker.CharInfo.StrongAttack.Chances.x, attacker.CharInfo.StrongAttack.Chances.y) :
               //     Random.Range(attacker.CharInfo.WeakAttack.Chances.x, attacker.CharInfo.WeakAttack.Chances.y)) >= Random.Range(0f, 1f))
                {
                    bool iscritical = attacker.CharInfo.IsCritical(true);
                    //Set damage to the hitting character
                    effectOn = target.SetDamage(attacker, damage * (iscritical ? 2 : 1), ele, iscritical, false);
                    if (effectOn)
                    {
                        int chances = Random.Range(0, 100);
                        if (chances < effectChances)
                        {
                            foreach (ScriptableObjectAttackEffect item in atkEffects.Effects.Where(r => !r.StatsToAffect.ToString().Contains("Tile")).ToList())
                            {
                                target.Buff_DebuffCo(new Buff_DebuffClass(new ElementalResistenceClass(), ElementalType.Dark, attacker, item));
                            }
                        }
                    }
                }
            }
            else
            {
                ScriptableObjectAttackEffect soAE = atkEffects.Effects.Where(r => r.StatsToAffect == BuffDebuffStatsType.BlockTile).FirstOrDefault();
                if (soAE != null)
                {
                    BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
                    bts.BlockTileForTime(soAE.Duration, ParticleManagerScript.Instance.GetParticle(soAE.Particles));
                }
            }
            if (effectOn && atkEffects.IsEffectOnTile)
            {
                BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
                bts.SetupEffect(atkEffects.EffectsOnTile, atkEffects.DurationOnTile, atkEffects.TileParticlesID);
            }
        }

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
