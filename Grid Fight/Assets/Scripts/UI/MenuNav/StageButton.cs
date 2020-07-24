using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;


public class StageButton : MonoBehaviour
{
    public StageProfile stage;
    protected StageLoadInformation info = null;
    public TextMeshProUGUI textDisplay;
    public Image stageImage;
    public Transform lockObj;
    public Animation lockAnim;
    public Color lockedColor;
    public Color unlockedColor;
    protected StageUnlockType displayedUnlockType = StageUnlockType.locked;

    private void Awake()
    {
        info = SceneLoadManager.Instance.loadedStages.Where(r => r.stageProfile.ID == stage.ID).FirstOrDefault();
        UpdateLockStateDisplay();
    }

    void UpdateLockStateDisplay()
    {
        lockObj.gameObject.SetActive(true);

        switch (info.lockState)
        {
            case StageUnlockType.locked:
                if (lockAnim.isPlaying)
                {
                    lockAnim.clip = null;
                    lockAnim.Play();
                }
                textDisplay.text = "???";
                stageImage.color = lockedColor;
                break;

            case StageUnlockType.unlocking:
                lockAnim.clip = lockAnim.GetClip("Padlock_Shake");
                lockAnim.Play();
                textDisplay.text = "???";
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
                textDisplay.text = info.stageProfile.Name;
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
        textDisplay.text = info.stageProfile.Name;
        stageImage.color = unlockedColor;
    }

    public void ProcessLockState()
    {
        if(info.lockState == StageUnlockType.unlocking)
        {
            info.lockState = StageUnlockType.unlocked;
        }
        UpdateLockStateDisplay();
    }

    public void DisplayBriefing()
    {
        if (info.lockState != StageUnlockType.unlocked) return;
        Grid_UIBriefing.Instance.gameObject.SetActive(true);
        Grid_UIBriefing.Instance.SetupBriefing(info.stageProfile);
        Grid_UINavigator.Instance.TriggerUIActivator("WorldBriefingTransition");
        WorldStageSelect.Instance.ShowStageButtonsNames(false);
    }

    public void ShowStageText(bool state)
    {
        textDisplay.gameObject.SetActive(state);
    }
}
