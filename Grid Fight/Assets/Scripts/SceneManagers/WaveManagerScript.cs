using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManagerScript : MonoBehaviour
{
    public static WaveManagerScript Instance;
    public List<WavePhaseClass> WavePhases = new List<WavePhaseClass>();
    public bool isWaveComplete = false;
    public int CurrentNumberOfWaveChars = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaveCo());
    }

    private IEnumerator WaveCo()
    {
        float timer = 0;
        foreach (WavePhaseClass wavePhase in WavePhases)
        {
            while (BattleManagerScript.Instance == null)
            {
                yield return new WaitForEndOfFrame();
            }
            CharacterBase newChar = BattleManagerScript.Instance.GetWaveCharacter(GetAvailableWaveCharacter(wavePhase), transform);
            SetCharInRandomPos(newChar);
            isWaveComplete = false;
            while (!isWaveComplete)
            {
                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return new WaitForEndOfFrame();
                }

                if(wavePhase.WavePhaseT == WavePhaseType.Combat && timer > wavePhase.DelayBetweenChars && CurrentNumberOfWaveChars < wavePhase.MaxEnemyOnScreen)
                {

                    newChar = BattleManagerScript.Instance.GetWaveCharacter(GetAvailableWaveCharacter(wavePhase), transform);
                    SetCharInRandomPos(newChar);
                    timer = 0;
                }
            }
        }
    }

    public void SetCharInRandomPos(CharacterBase currentCharacter)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetFreeBattleTile(currentCharacter.UMS.WalkingSide, currentCharacter.UMS.Pos);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
        {
            currentCharacter.UMS.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }

        foreach (Vector2Int item in currentCharacter.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
        }
        currentCharacter.SetAnimation(CharacterAnimationStateType.Arriving);
        StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(0.2f, currentCharacter, bts.transform.position));
        CurrentNumberOfWaveChars++;
    }


    public CharacterNameType GetAvailableWaveCharacter(WavePhaseClass wavePhase)
    {
        List<WaveCharClass> ListOfEnemy = wavePhase.ListOfEnemy.Where(r => r.NumberOfCharacter > 0).ToList();
        WaveCharClass enemy = ListOfEnemy[Random.Range(0, ListOfEnemy.Count)];
        enemy.NumberOfCharacter--;
        return enemy.TypeOfCharacter.CharacterName;
    }
}

[System.Serializable]
public class WavePhaseClass
{
    public WavePhaseType WavePhaseT;

    [Header("If Combat")]
    public int MaxEnemyOnScreen;
    public List<WaveCharClass> ListOfEnemy = new List<WaveCharClass>();
    public WaveCharacterInfoClass RecruitableCharacter;
    public float DelayBetweenChars;

    [Header("If Event")]
    public string EventName;



}

[System.Serializable]
public class WaveCharacterInfoClass
{
    public CharacterNameType CharacterName;
    public CharacterLevelType CharacterClass;
    public int MinHp;
    public int MaxHp;
    public int MinDamage;
    public int MaxDamage;
    public int MinBaseSpeed;
    public int MaxBaseSpeed;
    public int MinStamina;
    public int MaxStamina;
    public int MinStaminaRegeneration;
    public int MaxStaminaRegeneration;
}


[System.Serializable]
public class WaveCharClass
{
    public int NumberOfCharacter;
    public WaveCharacterInfoClass TypeOfCharacter;
}