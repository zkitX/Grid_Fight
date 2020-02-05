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
    public CharacterNameType characterID;
    public bool randomiseAttack = false;
    [ConditionalField("randomiseAttack", true)] public ScriptableObjectAttackBase attackType;

    protected IEnumerator attack()
    {
        IEnumerator attackOnceCoroutine = null;

        BaseCharacter character = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if (character == null) character = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if (character == null)
        {
            Continue();
            yield break;
        }

        if (character.GetType() == typeof(MinionType_Script))
        {
            if (randomiseAttack || attackType == null) character.GetAttack();
            else character.nextAttack = attackType;
            attackOnceCoroutine = character.AttackSequence();
            StartCoroutine(attackOnceCoroutine);
        }
        else
        {
            ((CharacterType_Script)character).StartQuickAttack(true);
        }

        yield return new WaitForSeconds(0.5f);
        while (character.currentAttackPhase != AttackPhasesType.End && character.SpineAnim.CurrentAnim != CharacterAnimationStateType.Idle)
        {
            yield return null;
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

