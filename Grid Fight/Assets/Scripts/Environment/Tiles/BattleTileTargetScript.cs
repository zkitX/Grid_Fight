using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileTargetScript : MonoBehaviour
{

    public Vector2Int Pos;
    public float Damage;
    public ElementalType Elemental;
    public void StartTarget(float duration)
    {
        StartCoroutine(TargetAnim(duration));
    }
    public void StartTarget(float duration, AttackParticleTypes atkPS, Vector2Int pos, float damage, ElementalType ele)
    {
        Pos = pos;
        Damage = damage;
        Elemental = ele;
        StartCoroutine(TargetAnim(duration, atkPS));
    }

    private IEnumerator TargetAnim(float duration)
    {
        float timer = 0;
      //  Debug.Log(duration);
        while (timer < 1)
        {
            yield return new WaitForFixedUpdate();

            while (BattleManagerScript.Instance != null && BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime / duration;

            transform.localScale = new Vector3(1 - timer, 1 - timer, 1);
        }

        gameObject.SetActive(false);
    }

    private IEnumerator TargetAnim(float duration, AttackParticleTypes atkPS)
    {
        float timer = 0;
        //  Debug.Log(duration);
        while (timer < 1)
        {
            yield return new WaitForFixedUpdate();

            while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime / duration;

            transform.localScale = new Vector3(1 - (timer > 0.9f ? 1 : timer) , 1 - (timer > 0.9f ? 1 : timer), 1);
        }
       
        BaseCharacter target;
        bool effectOn = true;
        target = BattleManagerScript.Instance.GetCharInPos(Pos);
        if (target != null)
        {
            effectOn = target.SetDamage(Damage, Elemental);
        }
        if(effectOn)
        {
            GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(atkPS, AttackParticlePhaseTypes.Effect, transform.position, SideType.LeftSide);
            LayerParticleSelection lps = effect.GetComponent<LayerParticleSelection>();
            if (lps != null)
            {
                lps.Shot = CharacterLevelType.Novice;
                lps.SelectShotLevel();

            }
        }
       
        gameObject.SetActive(false);
    }
}
