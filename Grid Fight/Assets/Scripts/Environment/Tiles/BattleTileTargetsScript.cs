using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTileTargetsScript : MonoBehaviour
{
    public List<Vector3> TargetsPosition = new List<Vector3>();
    public List<TargetClass> Targets = new List<TargetClass>();

    public void SetAttack(float duration, AttackParticleTypes atkPS, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker)
    {
        GameObject nextT = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Tile);
        
        nextT.SetActive(true);
        TargetClass tc = new TargetClass(duration, nextT);
        nextT.transform.parent = transform;
        Targets.Add(tc);
        UpdateQueue();
        StartCoroutine(FireTarget(tc, atkPS, pos, damage, ele, attacker));
    }

    private IEnumerator FireTarget(TargetClass tc, AttackParticleTypes atkPS, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker)
    {
        float timer = 0;
        float duration = tc.Duration;
        bool attackerFiredAttackAnim = false;
        //Animation animToFire = tc.TargetIndicator.GetComponent<Animation>();
        //animToFire["ExclamationAnim"].speed = 1 / duration;
        Animator anim = tc.TargetIndicator.GetComponent<Animator>();
        anim.speed = 1 / duration;
        while (timer < duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance != null && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && !BattleManagerScript.Instance.VFXScene))
            {
                yield return new WaitForFixedUpdate();
                //animToFire["ExclamationAnim"].speed = 0;
                anim.speed = 0;
            }
            //animToFire["ExclamationAnim"].speed = 1 / duration;
            anim.speed = 1 / duration;
            timer += Time.fixedDeltaTime;
            tc.RemainingTime = duration - timer;
            if (tc.RemainingTime <= 0.2f/*value at which to fire the animation for the shot*/ && attacker.CharInfo.BaseCharacterType == BaseCharType.MinionType_Script && !attackerFiredAttackAnim)
            {
                attackerFiredAttackAnim = true;
                //((MinionType_Script)attacker).fireAttackAnimation(); // trigger the shoot anim
            }
        }
        //animToFire["ExclamationAnim"].speed = 1;
        GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(atkPS, AttackParticlePhaseTypes.Effect, transform.position, SideType.LeftSide);
        LayerParticleSelection lps = effect.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            lps.Shot = CharacterLevelType.Novice;
            lps.SelectShotLevel();

        }

        if(BattleManagerScript.Instance != null)
        {
            BaseCharacter target;
            target = BattleManagerScript.Instance.GetCharInPos(pos);
            if (target != null)
            {
                target.SetDamage(damage, ele);
            }
        }
        yield return new WaitForSecondsRealtime(0.5f);
        UpdateQueue(tc);
    }


    public void UpdateQueue()
    {
        Targets = Targets.OrderByDescending(r => r.RemainingTime).ToList();
        for (int i = 0; i < Targets.Count; i++)
        {
            if(i < 5)
            {
                Targets[i].TargetIndicator.transform.localPosition = TargetsPosition[i];
            }
            else
            {
                return;
            }
        }
    }

    public void UpdateQueue(TargetClass completedTarget)
    {
        completedTarget.TargetIndicator.SetActive(false);
        Targets.Remove(completedTarget);
        UpdateQueue();
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
