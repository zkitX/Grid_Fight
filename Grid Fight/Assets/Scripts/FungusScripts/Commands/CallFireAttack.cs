using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fungus;
using MyBox;

[CommandInfo("Scripting",
                "Call Fire Attacc",
                "Fires a single attack")]
[AddComponentMenu("")]
public class CallFireAttack : Command
{
    public bool IsPlayerCharacter = true;
    //Player chars
    public CharacterNameType characterID;
    //Wave
    [ConditionalField("IsPlayerCharacter", true)] public bool IsRandomWaveChar;
    [ConditionalField("IsPlayerCharacter", true)] public string Identifier;
    public bool WaitForAttackAnimEnd = true;

    public bool randomiseAttack = false;
    [ConditionalField("randomiseAttack", true)] public ScriptableObjectAttackBase attackType;

    public Vector2Int TargetPosition = Vector2Int.zero;

    protected IEnumerator attack()
    {


        IEnumerator attackOnceCoroutine = null;
        BaseCharacter character;
        if (IsPlayerCharacter)
        {
            character = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
            if (character == null)
            {
                Continue();
                yield break;
            }
        }
        else
        {

            if(IsRandomWaveChar)
            {
                List<BaseCharacter> res = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).ToList();
                character = res[Random.Range(0, res.Count)];
            }
            else
            {
                if (!WaveManagerScript.Instance.FungusSpawnedChars.ContainsKey(Identifier))
                {
                    Continue();
                    yield break;
                }
                character = WaveManagerScript.Instance.FungusSpawnedChars[Identifier];

            }
        }

        while (!character.CanAttack)
        {
            yield return null;
        }
        if (character.GetType() == typeof(MinionType_Script))
        {
            if (randomiseAttack || attackType == null)
            {
                character.currentInputProfile.GetRandomAttack();
            }
            else
            {
                character.nextAttack = attackType;
            }
            character.currentInputProfile.nextAttackPos = TargetPosition;
            attackOnceCoroutine = character.AttackSequence();
            StartCoroutine(attackOnceCoroutine);
        }
        else
        {
            character.StartWeakAttack(true);
        }

        yield return new WaitForSeconds(0.5f);
        if (WaitForAttackAnimEnd)
        {
            while (character.currentAttackPhase != AttackPhasesType.End && character.SpineAnim.CurrentAnim != CharacterAnimationStateType.Idle.ToString())
            {
                yield return null;
            }
        }
       
        if(attackOnceCoroutine != null) StopCoroutine(attackOnceCoroutine);
        Continue();

    }

    #region Public members

    public override void OnEnter()
    {
        StartCoroutine(attack());
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}

