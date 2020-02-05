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
        Debug.Log("Meant to attack");
        BaseCharacter character = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if (character == null) character = WaveManagerScript.Instance.WaveCharcters.Where(r => r.CharInfo.CharacterID == characterID).FirstOrDefault();
        if (character == null)
        {
            Continue();
            yield break;
        }
        Debug.Log("Meant to attack");

        if (randomiseAttack || attackType == null) character.GetAttack();
        else character.nextAttack = attackType;

        StartCoroutine(character.AttackSequence());
        yield return new WaitForSeconds(0.5f);
        while (character.currentAttackPhase != AttackPhasesType.End)
        {
            yield return null;
        }
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

