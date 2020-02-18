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
    public void SetAttack(float duration, AttackParticleTypes atkPS, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker)
    {
        GameObject nextT = TargetIndicatorManagerScript.Instance.GetTargetIndicator(AttackType.Tile);
        
        nextT.SetActive(true);
        TargetClass tc = new TargetClass(duration, nextT);
        nextT.transform.parent = transform;
        nextT.transform.localPosition = TargetsPosition[0];
        Targets.Add(tc);
        UpdateQueue();
        StartCoroutine(FireTarget(tc, atkPS, pos, damage, ele, attacker));
    }

    private IEnumerator FireTarget(TargetClass tc, AttackParticleTypes atkPS, Vector2Int pos, float damage, ElementalType ele, BaseCharacter attacker)
    {
        float timer = 0;
        Whiteline.gameObject.SetActive(true);
        float duration = tc.Duration;
        bool attackerFiredAttackAnim = false;
        //Animation animToFire = tc.TargetIndicator.GetComponent<Animation>();
        //animToFire["ExclamationAnim"].speed = 1 / duration;
        Animator anim = tc.TargetIndicator.GetComponent<Animator>();
        anim.speed = 1 / duration;
        while (timer < duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance != null && ((BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets) && !BattleManagerScript.Instance.VFXScene))
            {
                yield return new WaitForFixedUpdate();
                //animToFire["ExclamationAnim"].speed = 0;
                anim.speed = 0;
            }
            //animToFire["ExclamationAnim"].speed = 1 / duration;
            anim.speed = 1 / duration;
            timer += Time.fixedDeltaTime;
            tc.RemainingTime = duration - timer;
            if ((!attacker.gameObject.activeInHierarchy || ((MinionType_Script)attacker).shotsLeftInAttack == 0) && !attackerFiredAttackAnim)
            {
                //Stop the firing of the attacks to the tiles
                ((MinionType_Script)attacker).shotsLeftInAttack = 0;
                tc.RemainingTime = 0f;
                UpdateQueue(tc);
                yield break;
            }
            else if (tc.RemainingTime <= duration*0.1f && attacker.UMS.CurrentAttackType == AttackType.Tile && !attackerFiredAttackAnim)
            {
                attackerFiredAttackAnim = true;
                ((MinionType_Script)attacker).fireAttackAnimation(); // trigger the shoot anim
            }
        }

        bool effectOn = true;
        if (BattleManagerScript.Instance != null)
        {
            BaseCharacter target;
            target = BattleManagerScript.Instance.GetCharInPos(pos);
            if (target != null)
            {
                effectOn = target.SetDamage(damage, ele);
            }
        }
        ((MinionType_Script)attacker).shotsLeftInAttack--;
        if (effectOn)
        {
            //animToFire["ExclamationAnim"].speed = 1;
            GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(atkPS, AttackParticlePhaseTypes.EffectRight, transform.position, attacker.UMS.Side);
            LayerParticleSelection lps = effect.GetComponent<LayerParticleSelection>();
            if (lps != null)
            {
                lps.Shot = CharacterLevelType.Novice;
                lps.SelectShotLevel();
            }
        }
        //anim.speed = 1;
        //yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - 1);
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
