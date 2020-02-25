using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewIBoss : MonoBehaviour
{
    [SerializeField] protected Image healthBar;
    IEnumerator HealthLerper;
    [SerializeField] protected float animationDuration = 0.2f;


    public void UpdateHp(float HealthPerc)
    {
        if (HealthPerc / 100f != healthBar.fillAmount)
        {
            if (HealthLerper != null) StopCoroutine(HealthLerper);
            HealthLerper = LerpVitality(healthBar, animationDuration, HealthPerc / 100f);
            StartCoroutine(HealthLerper);
        }
    }

    IEnumerator LerpVitality(Image vitality, float duration, float endFloat)
    {
        float startFloat = vitality.fillAmount;
        float startDuration = duration;
        while (duration > 0f)
        {
            duration = Mathf.Clamp(duration - Time.deltaTime, 0f, 9999f);
            vitality.fillAmount = Mathf.Lerp(startFloat, endFloat, 1f - duration / startDuration);
            yield return null;
        }
    }
}
