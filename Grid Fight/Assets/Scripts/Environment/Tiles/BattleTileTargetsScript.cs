using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTileTargetsScript : MonoBehaviour
{
    public List<Vector3> TargetsPosition = new List<Vector3>();
    public List<TargetClass> Targets = new List<TargetClass>();

    public void SetAttack(float duration, AttackParticleTypes atkPS, Vector2Int pos, float damage, ElementalType ele)
    {
        GameObject nextT = EnvironmentManager.Instance.AllExclamationTargets.Where(r => !r.gameObject.activeInHierarchy).FirstOrDefault();
        if(nextT == null)
        {
            nextT = EnvironmentManager.Instance.CreateNewExclamationTarget();
        }
        nextT.SetActive(true);
        TargetClass tc = new TargetClass(duration, nextT);
        nextT.transform.parent = transform;
        Targets.Add(tc);
        UpdateQueue();
        StartCoroutine(FireTarget(tc, atkPS, pos, damage, ele));
    }

    private IEnumerator FireTarget(TargetClass tc, AttackParticleTypes atkPS, Vector2Int pos, float damage, ElementalType ele)
    {
        float timer = 0;
        float duration = tc.Duration;
        Animation animToFire = tc.TargetIndicator.GetComponent<Animation>();
        animToFire["ExclamationAnim"].speed = 1 / duration;
        while (timer < duration)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
            {
                yield return new WaitForFixedUpdate();
                animToFire["ExclamationAnim"].speed = 0;
            }
            animToFire["ExclamationAnim"].speed = 1 / duration;
            timer += Time.fixedDeltaTime;
            tc.RemainingTime = duration - timer;
        }
        animToFire["ExclamationAnim"].speed = 1;

        GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(atkPS, AttackParticlePhaseTypes.Effect, transform.position, SideType.LeftSide);
        LayerParticleSelection lps = effect.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            lps.Shot = CharacterLevelType.Novice;
            lps.SelectShotLevel();

        }
        BaseCharacter target;
        target = BattleManagerScript.Instance.GetCharInPos(pos);
        if (target != null)
        {
            target.SetDamage(damage, ele);
        }

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
