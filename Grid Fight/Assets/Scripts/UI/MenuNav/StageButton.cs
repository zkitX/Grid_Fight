using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using MyBox;

[RequireComponent(typeof(CanvasGroup))]
public class StageButton : MonoBehaviour
{
    public StageProfile stage;
    public bool displayLockEffects = true;

    protected StageLoadInformation info = null;
    [ConditionalField("displayLockEffects")] public TextMeshProUGUI textDisplay;
    [ConditionalField("displayLockEffects")] public Image stageImage;
    [ConditionalField("displayLockEffects")] public Transform lockObj;
    [ConditionalField("displayLockEffects")] public Animation lockAnim;
    [ConditionalField("displayLockEffects")] public Color lockedColor;
    [ConditionalField("displayLockEffects")] public Color unlockedColor;
    protected StageUnlockType displayedUnlockType = StageUnlockType.locked;

    protected bool hiddenAndUnfocused = false;
    protected CanvasGroup canvGroup = null;

    private void Awake()
    {
        info = stage != null ? SceneLoadManager.Instance.loadedStages.Where(r => r.stageProfile.ID == stage.ID).FirstOrDefault() : null;
        canvGroup = GetComponent<CanvasGroup>();
        UpdateLockStateDisplay();
    }

    void UpdateLockStateDisplay()
    {
        if (!displayLockEffects) return;

        lockObj.gameObject.SetActive(true);

        switch (info.lockState)
        {
            case StageUnlockType.locked:
                if (lockAnim.isPlaying)
                {
                    lockAnim.clip = null;
                    lockAnim.Play();
                }
                if(textDisplay != null) textDisplay.text = "???";
                stageImage.color = lockedColor;
                break;

            case StageUnlockType.unlocking:
                lockAnim.clip = lockAnim.GetClip("Padlock_Shake");
                lockAnim.Play();
                if (textDisplay != null) textDisplay.text = "???";
                stageImage.color = lockedColor;
                break;

            case StageUnlockType.unlocked:
                if(displayedUnlockType == StageUnlockType.unlocking)
                {
                    lockAnim.clip = lockAnim.GetClip("Padlock_Open");
                    lockAnim.Play();
                    StartCoroutine(LerpUnlockStage(1f));
                    break;
                }
                if (textDisplay != null) textDisplay.text = info.stageProfile.Name;
                stageImage.color = unlockedColor;
                lockObj.gameObject.SetActive(false);
                break;

            default:
                break;
        }

        displayedUnlockType = info.lockState;
    }

    IEnumerator LerpUnlockStage(float time)
    {
        float startTime = time;
        while(time > 0f)
        {
            time -= Time.deltaTime;
            stageImage.color = Color.Lerp(stageImage.color, unlockedColor, 1f - (time / startTime));
            yield return null;
        }
        if (textDisplay != null) textDisplay.text = info.stageProfile.Name;
        stageImage.color = unlockedColor;
    }

    public void ProcessLockState()
    {
        if (info == null) return;

        if(info.lockState == StageUnlockType.unlocking)
        {
            info.lockState = StageUnlockType.unlocked;
        }
        UpdateLockStateDisplay();
    }

    public void DisplayBriefing()
    {
        if (info == null)
        {
            Debug.LogError("CANNOT DISPLAY BRIEFING OF A NULL STAGE");
            return;
        }
        if (info.lockState != StageUnlockType.unlocked) return;
        Grid_UIBriefing.Instance.gameObject.SetActive(true);
        Grid_UIBriefing.Instance.SetupBriefing(info.stageProfile);
        Grid_UINavigator.Instance.TriggerUIActivator("WorldBriefingTransition");
        WorldStageSelect.Instance.ShowStageButtonsNames(false);
    }

    public void UpdateBriefing()
    {
        SceneLoadManager.Instance.stagePrimedToLoad = info.stageProfile;
    }

    public void ShowStageText(bool state)
    {
        textDisplay?.gameObject.SetActive(state);
    }

    public void ShowButton(bool state)
    {
        if (state != hiddenAndUnfocused) return;
        hiddenAndUnfocused = !state;

        if (ShowHider != null) StopCoroutine(ShowHider);
        ShowHider = ShowHideCo(state ? 1f : 0f);
        StartCoroutine(ShowHider);
    }

    IEnumerator ShowHider = null;
    IEnumerator ShowHideCo(float endVal, float duration = 0.4f)
    {
        float remain = duration;
        while (remain != 0f)
        {
            remain = Mathf.Clamp(remain - Time.deltaTime, 0f, 10f);
            canvGroup.alpha = Mathf.Lerp(canvGroup.alpha, endVal, 1f - (remain / duration));
            yield return null;
        }
    }
}
