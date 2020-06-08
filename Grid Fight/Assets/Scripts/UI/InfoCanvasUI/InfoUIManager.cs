using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoUIManager : MonoBehaviour
{
    public static InfoUIManager Instance;

    public GameObject loadingScreen;
    public float loadingScreenFadeTime = 1f;
    protected IEnumerator LoadingFader;

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator FadeLoadingCanvas(bool phaseState, float duration = 1f)
    {
        float startAlpha = loadingScreen.GetComponent<CanvasGroup>().alpha;
        float endAlpha = phaseState ? 1f : 0f;
        float startingDuration = duration;
        float lerpProg = 0f;
        while (duration >= 0f)
        {
            //GIORGIO: fadeout audio
            duration = Mathf.Clamp(duration - Time.deltaTime, 0, startingDuration);
            if (duration != 0f) lerpProg = 1f - (duration / startingDuration);
            else lerpProg = 1f;
            loadingScreen.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startAlpha, endAlpha, lerpProg);
            yield return null;
        }
    }

    public void EnableLoadingScreen(bool state, bool lerpTo = true)
    {
        if(!loadingScreen.activeInHierarchy) loadingScreen.SetActive(state);
        if (LoadingFader != null) StopCoroutine(LoadingFader);
        LoadingFader = FadeLoadingCanvas(state, lerpTo ? loadingScreenFadeTime : 0f);
        StartCoroutine(LoadingFader);
    }
}
