using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

[RequireComponent(typeof(Animation))]
public class Grid_UITally : MonoBehaviour
{
    protected StatisticInfoClass info;

    [Header("References")]
    public Image portrait;

    [HideInInspector] public Animation anim;

    public TextMeshProUGUI baseXPText;

    public TextMeshProUGUI accuracyXPText;
    public Grid_UIStarRanking accuracyXPStars;

    public TextMeshProUGUI reflexXPText;
    public Grid_UIStarRanking reflexXPStars;

    public TextMeshProUGUI damageXPText;
    public Grid_UIStarRanking damageXPStars;

    public TextMeshProUGUI totalXPText;

    public Image XPBarFill;
    public Grid_UIStarRanking levelDisplay;
    public TextMeshProUGUI levelProgressionText;
    public Image levelProgressionBox;
    public Color filledProgressBarColor = Color.yellow;

    protected Transform startPortraitTransform;

    protected bool resolved = true;


    private void Awake()
    {
        startPortraitTransform = portrait.transform;
        anim = GetComponent<Animation>();
    }

    public void RevealTally()
    {
        anim.Play();
    }


    public void SetupTally(CharacterNameType character, bool outDented)
    {
        if (!resolved) return;
        resolved = false;

        InputController.Instance.ButtonADownEvent += SpeedUpTime;
        InputController.Instance.ButtonAUpEvent += ResetTimeSpeedUp;

        info = StatisticInfoManagerScript.Instance.GetCharacterStatsFor(character);

        if(BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == character).FirstOrDefault().CharInfo.CharacterIcon != null)
        {
            portrait.sprite = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == character).FirstOrDefault().CharInfo.CharacterIcon;
        }
       // if (outDented) portrait.transform.position = startPortraitTransform.position - new Vector3((portrait.rectTransform.sizeDelta.x / 2f), 0f);

        baseXPText.text = info.BaseExp.ToString();
        ValueShufflers = new List<IEnumerator>();
        ValueShufflers.Add(ShuffleValue(baseXPText, Mathf.RoundToInt(info.BaseExp)));
        ValueShufflers.Add(ShuffleValue(accuracyXPText, Mathf.RoundToInt(info.AccuracyExp), accuracyXPStars));
        ValueShufflers.Add(ShuffleValue(reflexXPText, Mathf.RoundToInt(info.ReflexExp), reflexXPStars));
        ValueShufflers.Add(ShuffleValue(damageXPText, Mathf.RoundToInt(info.DamageExp), damageXPStars));
        ValueShufflers.Add(ShuffleValue(totalXPText, Mathf.RoundToInt(info.BaseExp + info.Exp)));
        foreach (IEnumerator shuffler in ValueShufflers) StartCoroutine(shuffler);

        SetLevelStarSystem();
    }


    List<IEnumerator> ValueShufflers = new List<IEnumerator>();
    IEnumerator ShuffleValue(TextMeshProUGUI text, int endVal, Grid_UIStarRanking stars = null)
    {
        int currentStar = 0;
        int starChange = 1;
        while (true)
        {
            text.text = Random.Range(0, endVal * 4).ToString();
            if (stars != null) stars.SetStarRanking(((currentStar) *2)/10f);
            currentStar += starChange;
            if (currentStar == 5 || currentStar == 0) starChange *= -1;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    float timeMultiplier = 1f;
    public void SpeedUpTime(int player)
    {
        timeMultiplier = 4f;
    }

    public void ResetTimeSpeedUp(int player)
    {
        timeMultiplier = 1f;
    }



    float waitBetweenResults = 0.3f;
    float timeLeft = 0f;
    public IEnumerator ResolveTally()
    {
        resolved = true;

        StartCoroutine(GrowShrink(baseXPText.transform));
        StopCoroutine(ValueShufflers[0]);
        baseXPText.text = Mathf.Round(info.BaseExp).ToString();

        timeLeft = waitBetweenResults;
        while (timeLeft != 0)
        {
            timeLeft = Mathf.Clamp(timeLeft - (Time.deltaTime * timeMultiplier), 0f, 100f);
            yield return null;
        }

        StartCoroutine(GrowShrink(accuracyXPText.transform));
        StopCoroutine(ValueShufflers[1]);
        accuracyXPText.text = Mathf.Round(info.AccuracyExp).ToString();
        accuracyXPStars.SetStarRanking(info.Accuracy / BattleManagerBaseObjectGeneratorScript.Instance.stage.bestAccuracyRating.ValueToAchieve);

        timeLeft = waitBetweenResults;
        while (timeLeft != 0)
        {
            timeLeft = Mathf.Clamp(timeLeft - (Time.deltaTime * timeMultiplier), 0f, 100f);
            yield return null;
        }

        StartCoroutine(GrowShrink(reflexXPText.transform));
        StopCoroutine(ValueShufflers[2]);
        reflexXPText.text = Mathf.Round(info.ReflexExp).ToString();
        reflexXPStars.SetStarRanking(info.Reflexes / BattleManagerBaseObjectGeneratorScript.Instance.stage.bestReflexRating.ValueToAchieve);

        timeLeft = waitBetweenResults;
        while (timeLeft != 0)
        {
            timeLeft = Mathf.Clamp(timeLeft - (Time.deltaTime * timeMultiplier), 0f, 100f);
            yield return null;
        }

        StartCoroutine(GrowShrink(damageXPText.transform));
        StopCoroutine(ValueShufflers[3]);
        damageXPText.text = Mathf.Round(info.DamageExp).ToString();
        damageXPStars.SetStarRanking(info.DamageMade / BattleManagerBaseObjectGeneratorScript.Instance.stage.bestDamageRating.ValueToAchieve);

        timeLeft = waitBetweenResults * 2f;
        while (timeLeft != 0)
        {
            timeLeft = Mathf.Clamp(timeLeft - (Time.deltaTime * timeMultiplier), 0f, 100f);
            yield return null;
        }

        StartCoroutine(GrowShrink(totalXPText.transform));
        StopCoroutine(ValueShufflers[4]);
        totalXPText.text = Mathf.Round(info.BaseExp + info.Exp).ToString();

        yield return AddXP(info.BaseExp + info.Exp, 3f);
    }

    IEnumerator GrowShrink(Transform tran)
    {
        float timing = 0.5f;
        Vector3 startingScale = tran.localScale;
        Vector3 growScale = startingScale * 1.2f;
        float timeRemaining = timing * 0.8f;
        while (timeRemaining != 0f)
        {
            timeRemaining = Mathf.Clamp(timeRemaining - (Time.deltaTime * timeMultiplier), 0f, 99f);
            tran.localScale = Vector3.Lerp(tran.localScale, growScale, 1f - (timeRemaining / (timing * 0.8f)));
            yield return null;
        }
        timeRemaining = timing;
        while (timeRemaining != 0f)
        {
            timeRemaining = Mathf.Clamp(timeRemaining - (Time.deltaTime * timeMultiplier), 0f, 99f);
            tran.localScale = Vector3.Lerp(tran.localScale, startingScale, 1f - (timeRemaining / timing));
            yield return null;
        }

    }

    bool masteredDisplayed = false;
    void SetLevelStarSystem()
    {
        if (masteredDisplayed) return;

        CharacterLoadInformation charInfo = SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == info.CharacterId).FirstOrDefault();

        XPBarFill.fillAmount = charInfo.ProgressToNextLevel;
        levelDisplay.SetStarRanking((float)charInfo.Level / ((float)SceneLoadManager.Instance.charLevelThresholds.Length + 1f));

        if (charInfo.Level > SceneLoadManager.Instance.charLevelThresholds.Length)
        {
            masteredDisplayed = true;
            levelProgressionText.text = "MASTERED!";
            levelProgressionText.color = filledProgressBarColor;
            XPBarFill.color = filledProgressBarColor;
            levelProgressionBox.color = Color.black;
            return;
        }

        levelProgressionText.text = Mathf.FloorToInt(charInfo.xp).ToString() + "/" + SceneLoadManager.Instance.charLevelThresholds[charInfo.Level - 1].ToString();
    }


    IEnumerator AddXP(float XPToAdd, float duration)
    {
        float timeLeft = duration;
        float startXP = SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == info.CharacterId).FirstOrDefault().xp;
        float endXP = XPToAdd + startXP;
        while (SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == info.CharacterId).FirstOrDefault().xp != endXP)
        {
            timeLeft = Mathf.Clamp(timeLeft - Time.deltaTime, 0f, duration);
            SceneLoadManager.Instance.loadedCharacters.Where(r => r.characterID == info.CharacterId).FirstOrDefault().xp = Mathf.Lerp(startXP, endXP,  1f - (timeLeft/duration));
            SetLevelStarSystem();
            yield return null;
        }

        InputController.Instance.ButtonADownEvent -= SpeedUpTime;
        InputController.Instance.ButtonAUpEvent -= ResetTimeSpeedUp;
    }


    private void OnValidate()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        texts.OrderBy(r => r.transform.position.x);
        if (baseXPText == null) baseXPText = texts[0];
        if (accuracyXPText == null) accuracyXPText = texts[1];
        if (reflexXPText == null) reflexXPText = texts[2];
        if (damageXPText == null) damageXPText = texts[3];
        if (totalXPText == null) totalXPText = texts[4];

        Grid_UIStarRanking[] rankings = GetComponentsInChildren<Grid_UIStarRanking>();
        rankings.OrderBy(r => r.transform.position.x);
        if (accuracyXPStars == null) accuracyXPStars = rankings[0];
        if (reflexXPStars == null) reflexXPStars = rankings[1];
        if (damageXPStars == null) damageXPStars = rankings[2];

        Image[] images = GetComponentsInChildren<Image>();
        images.OrderBy(r => r.transform.localPosition.x);
        if (portrait == null) portrait = images[0];
    }
}
