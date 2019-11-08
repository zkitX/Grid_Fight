using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTileTargetScript : MonoBehaviour
{

    public void StartTarget(float duration)
    {
        StartCoroutine(TargetAnim(duration));
    }

    private IEnumerator TargetAnim(float duration)
    {
        float timer = 0;
        Debug.Log(duration);
        while (timer < 1)
        {
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime / duration;

            transform.localScale = new Vector3(1 - timer, 1 - timer, 1);
        }

        Destroy(gameObject);
    }

   
}
