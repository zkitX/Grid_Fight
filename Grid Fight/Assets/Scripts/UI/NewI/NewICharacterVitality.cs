using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewICharacterVitality : MonoBehaviour
{
    [SerializeField] public CharacterSelectionType assignedButton = CharacterSelectionType.Up;

    public SideType mapSide = SideType.LeftSide;
    [SerializeField] protected Image healthBar;
    [SerializeField] protected Image staminaBar;
    [SerializeField] protected Image specialBar;
    protected Animator animator;

    [SerializeField] protected TextMeshProUGUI selectionDiamondText;
    [SerializeField] protected Image characterIconIdle;
    [SerializeField] protected Image characterIconSelected;
    [SerializeField] protected TextMeshProUGUI characterName;
    [SerializeField] protected TextMeshProUGUI deadText;

    protected Color[] baseSelectionColors;
    [SerializeField] protected Image[] backgroundImages;

    public Color colorToChange;
    public CharacterInfoScript assignedCharDetails { get; private set; } = null;

    IEnumerator ColorLerper;
    [SerializeField] protected float animationDuration = 0.2f;

    IEnumerator HealthLerper;
    IEnumerator StaminaLerper;
    IEnumerator SpecialLerper;


    private void Awake()
    {
        selectionDiamondText.text = NewIManager.Instance.GetButtonTypeString(assignedButton);
        baseSelectionColors = new Color[backgroundImages.Length];
        for (int i = 0; i < backgroundImages.Length; i++)
        {
            baseSelectionColors[i] = backgroundImages[i].color;
        }
        animator = GetComponent<Animator>();
    }

    public void SetCharacter(CharacterInfoScript character)
    {
        if(character == null)
        {
            SetDefault();
            return;
        }
        ToggleVisible(true);
        ToggleDeathTextVisablity(false);

        assignedCharDetails = character;
        characterName.text = assignedCharDetails.Name;
        characterIconSelected.sprite = assignedCharDetails.CharacterIcons[1];
        characterIconIdle.sprite = assignedCharDetails.CharacterIcons[0];

        UpdateVitalities();
    }

    public void SetDefault()
    {
        characterName.text = "NONE";
        characterIconSelected.sprite = null;
        characterIconIdle.sprite = null;
        healthBar.fillAmount = 0f;
        staminaBar.fillAmount = 0f;

        ToggleVisible(false);
        //ToggleDeathTextVisablity(false);
    }

    void ToggleVisible(bool state)
    {
        foreach (Image img in GetComponentsInChildren<Image>())
        {
            img.enabled = state;
        }
        foreach (TextMeshProUGUI text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.enabled = state;
        }
    }

    void ToggleDeathTextVisablity(bool state)
    {
        deadText.enabled = state;
    }

    public void UpdateVitalities()
    {

        if (assignedCharDetails == null) return;

        if (assignedCharDetails.HealthPerc == 0f)
        {
            SetCharacter(null);
            ToggleDeathTextVisablity(true);
            return;
        }

        if (assignedCharDetails.HealthPerc / 100f != healthBar.fillAmount)
        {
            if (HealthLerper != null) StopCoroutine(HealthLerper);
            HealthLerper = LerpVitality(healthBar, animationDuration, assignedCharDetails.HealthPerc / 100f);
            StartCoroutine(HealthLerper);
        }
        if (assignedCharDetails.StaminaPerc / 100f != staminaBar.fillAmount)
        {
            if (StaminaLerper != null) StopCoroutine(StaminaLerper);
            StaminaLerper = LerpVitality(staminaBar, animationDuration, assignedCharDetails.StaminaPerc / 100f);
            StartCoroutine(StaminaLerper);
        }
    }

    public void SelectCharacter(ControllerType player)
    {
        switch (player)
        {
            case (ControllerType.Player1):
                colorToChange = BattleManagerScript.Instance.playersColor[0];
                selectionDiamondText.text = "P1";
                break;
            case (ControllerType.Player2):
                colorToChange = BattleManagerScript.Instance.playersColor[1];
                selectionDiamondText.text = "P2";
                break;
            case (ControllerType.Player3):
                colorToChange = BattleManagerScript.Instance.playersColor[2];
                selectionDiamondText.text = "P3";
                break;
            case (ControllerType.Player4):
                colorToChange = BattleManagerScript.Instance.playersColor[3];
                selectionDiamondText.text = "P4";
                break;
            default:
                colorToChange = Color.white;
                break;
        }
        ToggleSelectChar(true, colorToChange);
    }

    public void DeselectCharacter()
    {
        ToggleSelectChar(false, baseSelectionColors[0]);
        selectionDiamondText.text = NewIManager.Instance.GetButtonTypeString(assignedButton);
    }

    public void ToggleSelectChar(bool state, Color changeUITo)
    {
        if (assignedCharDetails == null) return;
        if (state)
        {
            animator.SetBool("indicatorOn", true);
        }
        else
        {
            animator.SetBool("indicatorOn", false);
        }

        if (changeUITo != backgroundImages[0].color)
        {
            if (ColorLerper != null) StopCoroutine(ColorLerper);
            ColorLerper = LerpBackgroundColors(animationDuration, changeUITo);
            StartCoroutine(ColorLerper);
        }
    }

    IEnumerator LerpBackgroundColors(float duration, Color endColor)
    {
        float startDuration = duration;
        Color[] startColors = new Color[backgroundImages.Length];
        for (int j = 0; j < startColors.Length; j++)
        {
            startColors[j] = backgroundImages[j].color;
        }

        while(duration > 0f)
        {
            duration = Mathf.Clamp(duration - Time.deltaTime, 0f, 99999f);
            for (int i = 0; i < backgroundImages.Length; i++)
            {
                backgroundImages[i].color = Color.Lerp(startColors[i], endColor, 1f - (duration / startDuration));
            }
            yield return null;
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
        if (assignedCharDetails.HealthPerc == 0f) DeselectCharacter();
    }

}
