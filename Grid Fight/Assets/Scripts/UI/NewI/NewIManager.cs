using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewIManager : MonoBehaviour
{
    public static NewIManager Instance;

    [SerializeField] protected string buttonTypeB;
    [SerializeField] protected string buttonTypeA;
    [SerializeField] protected string buttonTypeX;
    [SerializeField] protected string buttonTypeY;

    [SerializeField] protected GameObject timerBox = null;
    [SerializeField] protected TextMeshProUGUI timerText;
    [SerializeField] protected TextMeshProUGUI hitComboHighScoreText;
    [SerializeField] protected TextMeshProUGUI killComboHighScoreText;
    IEnumerator timeBoxUpdater;

    protected NewICharacterVitality[] vitalityBoxes;

    private void Awake()
    {
        Instance = this;
        vitalityBoxes = GetComponentsInChildren<NewICharacterVitality>();
        foreach(NewICharacterVitality vitalityBox in vitalityBoxes)
        {
            vitalityBox.SetCharacter(null);
        }

        ComboManager.OnComboTriggered -= UpdateComboCounts;
        ComboManager.OnComboTriggered += UpdateComboCounts;
        UpdateComboCounts();

        timeBoxUpdater = UpdateTimerText();
        StartCoroutine(timeBoxUpdater);
    }

    public void EnableTimerBox(bool state)
    {
        timerBox.SetActive(state);
        if(state) UpdateComboCounts();
    }

    IEnumerator UpdateTimerText()
    {
        while (WaveManagerScript.Instance == null) yield return null;
        //while (WaveManagerScript.Instance.battleTime.counting == false) yield return null;
        GameTime time;
        while (true)
        {
            time = WaveManagerScript.Instance.battleTime;

            string timerString = "";
            if (time.minutes < 10) timerString += "0";
            timerString += time.minutes.ToString();
            timerString += ":";
            if (time.seconds < 10) timerString += "0";
            timerString += Mathf.FloorToInt(time.seconds).ToString();

            timerText.text = timerString;

            yield return null;
        }
    }

    public void UpdateComboCounts()
    {
        if (ComboManager.Instance == null) return;

        if (hitComboHighScoreText != null && hitComboHighScoreText.isActiveAndEnabled)
        {
            hitComboHighScoreText.text = ComboManager.Instance.GetHighestAchievedCombo(ComboType.Attack).ToString();
        }
        if (killComboHighScoreText != null && killComboHighScoreText.isActiveAndEnabled)
        {
            killComboHighScoreText.text = ComboManager.Instance.GetHighestAchievedCombo(ComboType.Kill).ToString();
        }
    }

    public void SetSelected(bool state, ControllerType controller, CharacterNameType charName, SideType side = SideType.LeftSide)
    {
        if (state)
        {
            if(GetvitalityBoxOfCharacter(charName, side) == null)
            {
                
            }
            GetvitalityBoxOfCharacter(charName, side).SelectCharacter(controller);
        }
        else
        {
            GetvitalityBoxOfCharacter(charName, side).DeselectCharacter();
        }
    }

    public void SetUICharacterToButton(BaseCharacter character, CharacterSelectionType buttonToAssignTo)
    {
        GetvitalityBoxOfAssignedButton(buttonToAssignTo, character.UMS.Side).SetCharacter(character);
    }

    public void ToggleUICharacterDead(BaseCharacter character, bool state)
    {
        GetvitalityBoxOfCharacter(character.CharInfo.CharacterID, character.UMS.Side).ToggleDead(state);
    }

    public void UpdateVitalitiesOfCharacter(CharacterInfoScript character, SideType side)
    {
        GetvitalityBoxOfCharacter(character.CharacterID, side).UpdateVitalities();
    }

    public void TakeDamageSliceOnCharacter(CharacterNameType charName, SideType side)
    {
        GetvitalityBoxOfCharacter(charName, side).TakeDamageSlice();
    }

    public void PlayLowShieldIndicatorForCharacter(CharacterNameType charName, SideType side)
    {
        GetvitalityBoxOfCharacter(charName, side).PlayLowShieldAnimation();
    }

    public void StopLowShieldIndicatorForCharacter(CharacterNameType charName, SideType side)
    {
        GetvitalityBoxOfCharacter(charName, side).StopLowShieldAnimation();
    }

    public NewICharacterVitality GetvitalityBoxOfCharacter(CharacterNameType charName, SideType side)
    {
        return vitalityBoxes.Where(r =>r.assignedCharDetails != null && r.assignedCharDetails.CharInfo.CharacterID == charName && r.mapSide == side).FirstOrDefault();
    }

    public NewICharacterVitality GetvitalityBoxOfAssignedButton(CharacterSelectionType inButton, SideType side)
    {
        return vitalityBoxes.Where(r => r.assignedButton == inButton && r.mapSide == side).FirstOrDefault();
    }

    private void OnDestroy()
    {
        ComboManager.OnComboTriggered -= UpdateComboCounts;
    }

    public string GetButtonTypeString(CharacterSelectionType input)
    {
        return "";

        //Use this bit if each selection should have unique text
        
        switch (input)
        {
            case (CharacterSelectionType.Up):
                return buttonTypeB;
            case (CharacterSelectionType.Down):
                return buttonTypeA;
            case (CharacterSelectionType.Left):
                return buttonTypeX;
            case (CharacterSelectionType.Right):
                return buttonTypeY;
            default:
                return null;
        }
    }

}
