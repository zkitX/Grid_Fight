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
    protected float previousHealthChange;
    [SerializeField] protected Image staminaBar;
    [SerializeField] protected Image specialBar;
    [SerializeField] protected Image shieldBar;
    [SerializeField] protected Image shieldBackPlate;
    [SerializeField] protected Gradient shieldColors;
    protected Animator animator;

    [SerializeField] protected TextMeshProUGUI selectionDiamondText;
    [SerializeField] protected Image deathIcon = null;
    [SerializeField] protected Image characterIconIdle;
    [SerializeField] protected Image characterIconSelected;
    [SerializeField] protected TextMeshProUGUI characterName;
    [SerializeField] protected TextMeshProUGUI deadText;
    float colorDampenAmount = 0.7f;
    protected bool charDead = false;

    protected Vector3 damageSliceOrigin;
    [SerializeField] protected GameObject damageSlice;

    protected Color[] baseSelectionColors;
    [SerializeField] protected Image[] backgroundImages;

    public Color colorToChange;
    public CharacterInfoScript assignedCharDetails { get; private set; } = null;

    IEnumerator ColorLerper;
    [SerializeField] protected float animationDuration = 0.2f;

    IEnumerator HealthLerper;
    IEnumerator StaminaLerper;
    IEnumerator ShieldLerper;
    IEnumerator SpecialLerper;

    private void Awake()
    {
        damageSliceOrigin = damageSlice.transform.position;
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
        ToggleDead(false);

        assignedCharDetails = character;

        previousHealthChange = assignedCharDetails.HealthPerc / 100f;
        if(shieldBar != null) shieldBar.color = shieldColors.Evaluate(assignedCharDetails.ShieldPerc / 100f);
        characterName.text = assignedCharDetails.Name;
        if (assignedCharDetails.CharacterIcons.Length > 0)
        {
            characterIconIdle.sprite = assignedCharDetails.CharacterIcons[0];
            if (assignedCharDetails.CharacterIcons.Length > 1)
            {
                characterIconSelected.sprite = assignedCharDetails.CharacterIcons[1];
            }
        }
       

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

    public void ToggleDead(bool state)
    {
        charDead = state;
        if (state)
        {
            DeselectCharacter();
            ToggleDeathTextVisablity(true);
            StartCoroutine(ReviveSequence());
        }
        else
        {
            ToggleDeathTextVisablity(false);
        }
    }

    IEnumerator ReviveSequence()
    {
        Color healthBarColor = healthBar.color;
        Color staminaBarColor = staminaBar.color;
        Color shieldBarColor = shieldBar.color;
        Color shieldBarBackPlateColor = shieldBackPlate.color;
        Color characterIconColor = characterIconIdle.color;
        healthBar.color *= colorDampenAmount;
        staminaBar.color *= colorDampenAmount;
        shieldBar.color *= colorDampenAmount;
        shieldBackPlate.color *= colorDampenAmount;
        characterIconIdle.color *= colorDampenAmount;
        float healthStart = healthBar.fillAmount;
        float staminaStart = staminaBar.fillAmount;
        float shieldStart = shieldBar.fillAmount;

        float waitLeft = assignedCharDetails.CharacterRespawnLength;
        float startingWait = waitLeft;
        float prog = 0f;
        while (waitLeft != 0f)
        {
            waitLeft = Mathf.Clamp(waitLeft - Time.deltaTime, 0f, 9999f);
            prog = 1f - (waitLeft / startingWait);
            healthBar.fillAmount = Mathf.Lerp(healthStart, 1f, prog);
            staminaBar.fillAmount = Mathf.Lerp(staminaStart, 1f, prog);
            shieldBar.fillAmount = Mathf.Lerp(shieldStart, 1f, prog);
            yield return null;
        }

        healthBar.color = healthBarColor;
        staminaBar.color = staminaBarColor;
        shieldBar.color = shieldBarColor;
        shieldBackPlate.color = shieldBarBackPlateColor;
        characterIconIdle.color = characterIconColor;
    }

    public void TakeDamageSlice()
    {
        if (DamageSliceSequencer != null) StopCoroutine(DamageSliceSequencer);
        DamageSliceSequencer = DamageSliceSequence();
        StartCoroutine(DamageSliceSequencer);
    }

    IEnumerator DamageSliceSequencer = null;
    IEnumerator DamageSliceSequence()
    {
        Vector3 slicePos = damageSliceOrigin + (mapSide == SideType.LeftSide ? 1f : -1f) * (new Vector3(healthBar.rectTransform.sizeDelta.x * (assignedCharDetails.HealthPerc / 100f), 0f));
        damageSlice.transform.position = slicePos;
        damageSlice.GetComponentInChildren<Animator>().SetTrigger("SliceDamage");
        yield return null;
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
        deadText.enabled = /*state*/false; //Just set to false while it's not being used and the skull icon is
        if (deathIcon != null) deathIcon.enabled = state;
    }

    public void UpdateVitalities()
    {
        if (assignedCharDetails == null || charDead) return;

        if (assignedCharDetails.HealthPerc / 100f != healthBar.fillAmount)
        {
            if(assignedCharDetails.HealthPerc / 100f < healthBar.fillAmount && (previousHealthChange > assignedCharDetails.HealthPerc / 100f))
            {
                TakeDamageSlice();
            }
            previousHealthChange = assignedCharDetails.HealthPerc / 100f;
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
        if (assignedCharDetails.ShieldPerc / 100f != shieldBar.fillAmount)
        {
            if (ShieldLerper != null) StopCoroutine(ShieldLerper);
            ShieldLerper = LerpVitality(shieldBar, animationDuration, assignedCharDetails.ShieldPerc / 100f, shieldColors);
            StartCoroutine(ShieldLerper);
        }

        if (assignedCharDetails.HealthPerc == 0f)
        {
            ///SET UI OF THE CHARACTER TO DEAD HERE
            ToggleDead(true);
            //SetCharacter(null);
            //ToggleDeathTextVisablity(true);
            return;
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

    public void PlayLowShieldAnimation()
    {
        shieldBackPlate.GetComponent<Animation>().Play();
    }

    public void StopLowShieldAnimation()
    {
        shieldBackPlate.GetComponent<Animation>().Stop();
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

    IEnumerator LerpVitality(Image vitality, float duration, float endFloat, Gradient colorChangeGradient = null)
    {
        float startFloat = vitality.fillAmount;
        float startDuration = duration;
        while (duration > 0f)
        {
            duration = Mathf.Clamp(duration - Time.deltaTime, 0f, 9999f);
            vitality.fillAmount = Mathf.Lerp(startFloat, endFloat, 1f - duration / startDuration);
            if (colorChangeGradient != null)
            {
                vitality.color = colorChangeGradient.Evaluate(vitality.fillAmount) * (!charDead ? 1f : colorDampenAmount);
            }
            yield return null;
        }
        if (assignedCharDetails.HealthPerc == 0f) DeselectCharacter();
    }

}
