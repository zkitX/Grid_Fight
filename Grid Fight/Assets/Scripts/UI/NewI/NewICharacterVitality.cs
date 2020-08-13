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
    public BaseCharacter assignedCharDetails { get; private set; } = null;

    IEnumerator ColorLerper;
    [SerializeField] protected float animationDuration = 0.2f;


    public GameObject SkillContainer;
    public Image Skill1;
    public Image Skill2;
    public Image Skill3;

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

    public void SetCharacter(BaseCharacter character)
    {
        if(character == null)
        {
            SetDefault();
            return;
        }
        ToggleVisible(true);
        ToggleDead(false);

        assignedCharDetails = character;
        if(character.CharInfo.Mask != null)
        {
            ((CharacterType_Script)assignedCharDetails).CurrentCharSkillCompletedEvent += RefillSkills;
        }
        else
        {
            SkillContainer.SetActive(false);
        }

        if(shieldBar != null) shieldBar.color = shieldColors.Evaluate(assignedCharDetails.CharInfo.ShieldPerc / 100f);
        characterName.text = assignedCharDetails.CharInfo.Name;
        characterIconSelected.sprite = assignedCharDetails.CharInfo.CharacterIcon;
       

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

    public void RefillSkills(AttackInputType inputSkill, float duration)
    {
        switch (inputSkill)
        {
            case AttackInputType.Skill1:
                StartCoroutine(RefillSkills_Co(Skill1, duration));
                break;
            case AttackInputType.Skill2:
                StartCoroutine(RefillSkills_Co(Skill2, duration));
                break;
            case AttackInputType.Skill3:
                StartCoroutine(RefillSkills_Co(Skill3, duration));
                break;
        }
    }

    IEnumerator RefillSkills_Co(Image skill, float duration)
    {
        float timer = 0;
        float res = 0;
        while (timer < duration)
        {
            res = Mathf.Lerp(0, 1, timer / duration);
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
            timer += BattleManagerScript.Instance.DeltaTime;
            skill.fillAmount = res;
        }

        skill.fillAmount = 1;
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

        float waitLeft = assignedCharDetails.CharInfo.CharacterRespawnLength;
        float startingWait = waitLeft;
        float prog = 0f;
        while (waitLeft != 0f)
        {
            deadText.text = (Mathf.FloorToInt(waitLeft) + 1).ToString();
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
        if (DamageSliceSequencer != null) return;
        DamageSliceSequencer = DamageSliceSequence();
        StartCoroutine(DamageSliceSequencer);
    }

    void EndDamageSliceCo()
    {
        DamageSliceSequencer = null;
    }

    IEnumerator DamageSliceSequencer = null;
    IEnumerator DamageSliceSequence()
    {
        Vector3 slicePos = damageSliceOrigin + (mapSide == SideType.LeftSide ? 1f : -1f) * (new Vector3(healthBar.rectTransform.sizeDelta.x * (assignedCharDetails.CharInfo.HealthPerc / 100f), 0f));
        damageSlice.transform.position = slicePos;
        Animation anim = damageSlice.GetComponentInChildren<Animation>();
        anim.Play();
        while (anim.isPlaying)
        {
            yield return null;
        }
        EndDamageSliceCo();
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
        deadText.enabled = /*state*/state; //Just set to false while it's not being used and the skull icon is
        characterIconIdle.color = state ? new Color(0.3f, 0.3f, 0.3f, 1f) : Color.white;
        characterIconSelected.color = state ? new Color(0.3f, 0.3f, 0.3f, 1f) : Color.white;
        if (deathIcon != null) deathIcon.enabled = state;
    }

    public void UpdateVitalities()
    {
        if (assignedCharDetails == null || charDead) return;

        if (assignedCharDetails.CharInfo.HealthPerc / 100f != healthBar.fillAmount)
        {
            if (HealthLerper != null) StopCoroutine(HealthLerper);
            HealthLerper = LerpVitality(healthBar, animationDuration, assignedCharDetails.CharInfo.HealthPerc / 100f);
            StartCoroutine(HealthLerper);
        }
        if (assignedCharDetails.CharInfo.EtherPerc / 100f != staminaBar.fillAmount)
        {
            if (StaminaLerper != null) StopCoroutine(StaminaLerper);
            StaminaLerper = LerpVitality(staminaBar, animationDuration, assignedCharDetails.CharInfo.EtherPerc / 100f);
            StartCoroutine(StaminaLerper);
        }
        if (assignedCharDetails.CharInfo.ShieldPerc / 100f != shieldBar.fillAmount)
        {
            if (ShieldLerper != null) StopCoroutine(ShieldLerper);
            ShieldLerper = LerpVitality(shieldBar, animationDuration, assignedCharDetails.CharInfo.ShieldPerc / 100f, shieldColors);
            StartCoroutine(ShieldLerper);
        }

        if (assignedCharDetails.CharInfo.HealthPerc == 0f && assignedCharDetails.died)
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
        if (assignedCharDetails.CharInfo.HealthPerc == 0f) DeselectCharacter();
    }

}
