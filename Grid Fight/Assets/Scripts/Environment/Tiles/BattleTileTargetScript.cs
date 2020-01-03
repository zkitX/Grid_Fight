using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileTargetScript : MonoBehaviour
{

    public void StartTarget(float duration)
    {
        StartCoroutine(TargetAnim(duration));
    }
    public void StartTarget(float duration, AttackParticleTypes atkPS)
    {
        StartCoroutine(TargetAnim(duration, atkPS));
    }

    private IEnumerator TargetAnim(float duration)
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

            transform.localScale = new Vector3(1 - timer, 1 - timer, 1);
        }

        Destroy(gameObject);
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
        GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(atkPS, ParticleTypes.Effect, transform.position,  SideType.LeftSide);
        LayerParticleSelection lps = effect.GetComponent<LayerParticleSelection>();
        if (lps != null)
        {
            lps.Shot = CharacterLevelType.Novice;
            lps.SelectShotLevel();

        }
        Destroy(gameObject);
    }
}
