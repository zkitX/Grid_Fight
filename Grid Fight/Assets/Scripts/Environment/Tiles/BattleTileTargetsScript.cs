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
    public void SetAttack(float duration, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker, List<ScriptableObjectAttackEffect> atkEffects, float effectChances)
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

    private IEnumerator FireTarget(TargetClass tc, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker, List<ScriptableObjectAttackEffect> atkEffects, float effectChances)
    {
        float timer = 0;
        Whiteline.gameObject.SetActive(true);
        float duration = tc.Duration;
        bool attackerFiredAttackAnim = false;
        Animator anim = tc.TargetIndicator.GetComponent<Animator>();
        anim.speed = 1 / duration;
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
            if (!attacker.Attacking)
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
        if (BattleManagerScript.Instance != null)
        {
            BaseCharacter target;
            target = BattleManagerScript.Instance.GetCharInPos(pos);
            if (target != null)
            {
                bool iscritical = attacker.CharInfo.IsCritical(true);
                //Set damage to the hitting character
                effectOn = target.SetDamage(damage * (iscritical ? 2 : 1), ele, iscritical);
                if (effectOn)
                {
                    int chances = Random.Range(0, 100);
                    if (chances < effectChances)
                    {
                        foreach (ScriptableObjectAttackEffect item in atkEffects)
                        {
                            target.Buff_DebuffCo(new Buff_DebuffClass(item.Name, item.Duration.x, item.Value.x, item.StatsToAffect, item.StatsChecker, new ElementalResistenceClass(), ElementalType.Dark, item.AnimToFire, item.Particles));
                        }
                    }
                }
            }
        }


        if (attacker.CharInfo.Health > 0 && attacker.Attacking)
        {
            if (effectOn)
            {
                attacker.SpecialAttackImpactEffects(transform.position);
            }
            if (attacker.GetAttackAudio() != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attacker.GetAttackAudio().Impact, AudioBus.MediumPriority, transform);
            }
        }
        yield return new WaitForSeconds(0.2f);
        UpdateQueue(tc);
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
