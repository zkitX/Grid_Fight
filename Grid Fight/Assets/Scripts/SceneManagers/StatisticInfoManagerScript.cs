using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;

public class StatisticInfoManagerScript : MonoBehaviour
{
    public static StatisticInfoManagerScript Instance;
    public List<StatisticInfoClass> CharaterStats = new List<StatisticInfoClass>();

    [Header("Comboing")]
    [SerializeField] protected bool useComboGroupings = false;
    [SerializeField] protected List<ComboGroupInspectorClass> comboGroups = new List<ComboGroupInspectorClass>();
    public List<PlayerComboInfoGroupClass> comboInfo = new List<PlayerComboInfoGroupClass>();
    public ComboThresholInfoClass[] comboThresholds = new ComboThresholInfoClass[0];
    public float maxIntensityCombo = 25f;
    public Color AttackComboColor = Color.red;
    public Color DefenceComboColor = Color.blue;

    private void Awake()
    {
        Instance = this;
        SetupComboInfo();
    }

    //Return a combination of the xp stats class on the gameobject and the actual stats on the character
    public StatisticInfoClass GetCharacterStatsFor(CharacterNameType ID)
    {
        StatisticInfoClass returnable = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == ID).FirstOrDefault().Sic;
        StatisticInfoClass additive = CharaterStats.Where(r => r.CharacterId == ID).FirstOrDefault();
        returnable.BaseExp += additive.BaseExp;
        returnable.AccuracyExp += additive.AccuracyExp;
        returnable.DamageExp += additive.DamageExp;
        returnable.ReflexExp += additive.ReflexExp;
        return returnable;
    }

    void SetupComboInfo()
    {
        for (int i = 0; i < (useComboGroupings ?  comboGroups.Count : InputController.Instance.PlayersNumber); i++)
        {
            comboInfo.Add(new PlayerComboInfoGroupClass(useComboGroupings ? comboGroups[i].playerIndexes : new int[] { i }));
        }
    }

    public void TriggerComboForCharacter(CharacterNameType charName, ComboType combo, bool hit, Transform displayTarget = null)
    {
        if(BattleManagerScript.Instance.CurrentSelectedCharacters.Values.Where(r => r.Character != null && r.Character.CharInfo.CharacterID == charName).FirstOrDefault() != null)
        {
            TriggerComboForPlayer((int)BattleManagerScript.Instance.CurrentSelectedCharacters.Where(r => r.Value.Character != null && r.Value.Character.CharInfo.CharacterID == charName).FirstOrDefault().Key, combo, hit, displayTarget);
        }
    }

    protected void TriggerComboForPlayer(int playerIndex, ComboType combo, bool hit, Transform displayTarget = null)
    {
        int comboNum = comboInfo.Where(r => r.ContainsPlayer(playerIndex)).FirstOrDefault().TriggerCombo(combo, hit);
        if (displayTarget == null)
        {
            return;
        }

        string text = "";
        Color displayColor = combo == ComboType.Attack ? AttackComboColor : combo == ComboType.Defence ? DefenceComboColor : Color.white;
        bool finalColorFound = false;
        int lastThresholdVal = 0;
        for(int i = 0; i < comboThresholds.Length; i++)
        {
            if(comboThresholds[i].val <= comboNum)
            {
                lastThresholdVal = comboThresholds[i].val;
                displayColor = combo == ComboType.Attack ? comboThresholds[i].attackColor : combo == ComboType.Defence ? comboThresholds[i].defenceColor : Color.white;
            }
            else if (!finalColorFound)
            {
                finalColorFound = true;
                displayColor = Color.Lerp(displayColor, combo == ComboType.Attack ? comboThresholds[i].attackColor : combo == ComboType.Defence ? comboThresholds[i].defenceColor : Color.white, (float)(comboNum - lastThresholdVal) / (float)(comboThresholds[i].val - lastThresholdVal));
            }
            if (comboNum == comboThresholds[i].val) text = comboThresholds[i].Text;
        }

        UIBattleFieldManager.Instance.DisplayCombo(combo, comboNum, displayTarget.position, text, displayColor) ;
    }

    public void TriggerComboInfoClass(ComboInfoClass combo)
    {
        StartCoroutine(combo.comboCountDown);
    }

    private void OnValidate()
    {
        foreach (ComboThresholInfoClass combT in comboThresholds)
        {
            combT.Name = combT.val.ToString() + (combT.texts.Length == 0 ? " NO TEXTS ASSIGNED!!!!" : "");
        }
    }
}

[System.Serializable]
public class ComboThresholInfoClass
{
    [HideInInspector] public string Name;
    public int val;
    public string[] texts;
    public Color attackColor;
    public Color defenceColor;
    public string Text
    {
        get
        {
           return texts[Random.Range(0, texts.Length)];
        }
    }
}

[System.Serializable]
public class ComboGroupInspectorClass
{
    public string Name;
    public int[] playerIndexes = new int[0];
}

[System.Serializable]
public class StatisticInfoClass
{
    public List<ControllerType> PlayerController;
    public CharacterNameType CharacterId;
    public float DamageMade;
    public float DamageReceived;
    public float TimeOnField;
    public int BulletFired;
    public int BulletHits;
    public float Accuracy
    {
        get
        {
            return (float)BulletHits / ((float)BulletFired != 0f ? (float)BulletFired : 1f);
        }
    }
    public int HitReceived;
    public int Defences;
    public int CompleteDefences;
    public float Reflexes
    {
        get
        {
            return (((float)Defences / ((float)HitReceived != 0f ? (float)HitReceived : 1f)) + ((float)CompleteDefences / ((float)Defences != 0f ? (float)Defences : 1f))) / 2f;
        }
    }
    public float HPGotBySkill;
    public float HPHealed;
    public int PotionPicked;
    public float Exp
    {
        get
        {
            return AccuracyExp + DamageExp + ReflexExp;
        }
    }
    protected float accuracyExp;
    public float AccuracyExp
    {
        get
        {
            return BattleManagerBaseObjectGeneratorScript.Instance.stage.bestAccuracyRating.UseMaximumRewardSystem ?
                Mathf.Clamp(accuracyExp, 0f, BattleManagerBaseObjectGeneratorScript.Instance.stage.bestAccuracyRating.MaximumReward) : accuracyExp;
        }
        set
        {
            accuracyExp = value;
        }
    }
    protected float damageExp;
    public float DamageExp
    {
        get
        {
            return BattleManagerBaseObjectGeneratorScript.Instance.stage.bestDamageRating.UseMaximumRewardSystem ?
                Mathf.Clamp(damageExp, 0f, BattleManagerBaseObjectGeneratorScript.Instance.stage.bestDamageRating.MaximumReward) : damageExp;
        }
        set
        {
            damageExp = value;
        }
    }
    protected float reflexExp;
    public float ReflexExp
    {
        get
        {
            return BattleManagerBaseObjectGeneratorScript.Instance.stage.bestReflexRating.UseMaximumRewardSystem ?
                Mathf.Clamp(reflexExp, 0f, BattleManagerBaseObjectGeneratorScript.Instance.stage.bestReflexRating.MaximumReward) : reflexExp;
        }
        set
        {
            reflexExp = value;
        }
    }
    public float baseExp;
    public float BaseExp
    {
        get
        {
            return baseExp;
        }
        set
        {
            baseExp = value;
        }
    }

    public StatisticInfoClass()
    {

    }

    public StatisticInfoClass(CharacterNameType characterId, List<ControllerType> playerController, float startingExperience = 0f)
    {
        CharacterId = characterId;
        PlayerController = playerController;
        BaseExp = startingExperience;
    }
}

[System.Serializable]
public class GlobalCharacterStatisticInfoClass
{
    public float Experience;
    public float TotalDamageMade;
    public float TotalDamageReceived;
    public float TotalTimeOnField;
    public int TotalBulletFired;
    public int TotalBulletHits;
    public int TotalHitReceived;
    public int TotalDefences;
    public int TotalCompleteDefences;
    public float TotalHPGotBySkill;
    public float TotalHPHealed;
    public int TotalPotionPicked;
    public float TotalExp;

    public GlobalCharacterStatisticInfoClass()
    {

    }

}

[System.Serializable]
public class PlayerComboInfoGroupClass
{
    public List<int> playerIndexes = new List<int>();
    public Dictionary<ComboType, ComboInfoClass> comboInfo = new Dictionary<ComboType, ComboInfoClass>
    {
        { ComboType.Attack, new ComboInfoClass(UniversalGameBalancer.Instance.attackComboDuration) },
        { ComboType.Defence, new ComboInfoClass(UniversalGameBalancer.Instance.defenceComboDuration) },
    };

    public PlayerComboInfoGroupClass(int[] playerIndex)
    {
        playerIndexes = playerIndex.ToList();
        comboInfo = new Dictionary<ComboType, ComboInfoClass>
        {
            { ComboType.Attack, new ComboInfoClass(UniversalGameBalancer.Instance.attackComboDuration) },
            { ComboType.Defence, new ComboInfoClass(UniversalGameBalancer.Instance.defenceComboDuration) },
        };
    }

    public bool ContainsPlayer(int playerIndex)
    {
        if (playerIndexes.Contains(playerIndex)) return true;
        return false;
    }

    public int TriggerCombo(ComboType combo, bool hit)
    {
        comboInfo[combo].TriggerCombo(hit);
        return comboInfo[combo].comboCount;
    }
}

[System.Serializable]
public class ComboInfoClass
{
    public int comboCount = 0;
    protected float resetTime = 0f;
    public float timeRemaining = 0f;
    public IEnumerator comboCountDown = null;

    public ComboInfoClass(float resetTiming)
    {
        resetTime = resetTiming;
        comboCount = 0;
        timeRemaining = 0f;
        comboCountDown = null;
    }


    public void TriggerCombo(bool hit)
    {
        bool timeBelowZero = timeRemaining <= 0f;
        comboCount = hit ? comboCount + 1 : 0;
        Debug.Log("<b>" + comboCount.ToString() + "</b> with seconds remaining: " + timeRemaining.ToString());
        timeRemaining = hit ? resetTime : 0f;
        if (timeBelowZero && hit)
        {
            comboCountDown = CountComboDown();
            StatisticInfoManagerScript.Instance.TriggerComboInfoClass(this);
        }
    }

    IEnumerator CountComboDown()
    {
        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.unscaledDeltaTime;
            yield return BattleManagerScript.Instance.WaitUpdate(() => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle && BattleManagerScript.Instance.CurrentBattleState != BattleState.FungusPuppets);
        }
        comboCount = 0;
    }
}