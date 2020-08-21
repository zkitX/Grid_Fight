using MyBox;
using PlaytraGamesLtd;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharaqcterInput/PlayerInput")]
public class ScriptableObjectPlayerInput : ScriptableObjectBaseCharaterInput
{
    public delegate void CurrentCharSkillCompleted(AttackInputType inputSkill, float duration);
    public event CurrentCharSkillCompleted CurrentCharSkillCompletedEvent;

    [HideInInspector] public List<SkillCoolDownClass> skillCoolDown = new List<SkillCoolDownClass>()
    {
        new SkillCoolDownClass(AttackInputType.Skill1, false),
        new SkillCoolDownClass(AttackInputType.Skill2, false),
        new SkillCoolDownClass(AttackInputType.Skill3, false)
    };
    public IEnumerator SkillActivation = null;


    public void StartChargingAtk(AttackInputType atkType)
    {
        switch (atkType)
        {
            case AttackInputType.Strong:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.StrongAttack))
                {
                    return;
                }
                CharOwner.StartCoroutine(CharOwner.StartChargingAttack(atkType));
                break;
            case AttackInputType.Skill1:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.Skill1) || CharOwner.CharInfo.Mask == null || CharOwner.SkillActivation != null)
                {
                    return;
                }
                CharOwner.SkillActivation = CharOwner.StartSkillAttack(AttackInputType.Skill1);
                CharOwner.StartCoroutine(CharOwner.SkillActivation);

                break;
            case AttackInputType.Skill2:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.Skill2) || CharOwner.CharInfo.Mask == null || CharOwner.SkillActivation != null)
                {
                    return;
                }
                CharOwner.SkillActivation = CharOwner.StartSkillAttack(AttackInputType.Skill2);
                CharOwner.StartCoroutine(CharOwner.SkillActivation);
                break;
            case AttackInputType.Skill3:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.Skill3) || CharOwner.CharInfo.Mask == null || CharOwner.SkillActivation != null)
                {
                    return;
                }
                CharOwner.SkillActivation = CharOwner.StartSkillAttack(AttackInputType.Skill3);
                CharOwner.StartCoroutine(CharOwner.SkillActivation);
                break;

        }
    }


    public void CharacterInputHandler(InputActionType action)
    {
        if (!HasBuffDebuff(BuffDebuffStatsType.Rage))
        {
            StartCoroutine(CharacterInputQueue(action));
        }
    }

    IEnumerator CharacterInputQueue(InputActionType action)
    {
        isSpecialStop = false;
        if (action == InputActionType.Defend)
        {
            IsDefStartCo = true;
        }
        while (isMoving)
        {
            yield return null;
        }
        Debug.Log(action);
        switch (action)
        {
            case InputActionType.Weak:
                StartWeakAttack(false);
                break;
            case InputActionType.Strong:
                StartChargingAtk(AttackInputType.Strong);
                break;
            case InputActionType.Skill1:
                StartChargingAtk(AttackInputType.Skill1);
                break;
            case InputActionType.Skill2:
                StartChargingAtk(AttackInputType.Skill2);
                break;
            case InputActionType.Skill3:
                StartChargingAtk(AttackInputType.Skill3);
                break;
            case InputActionType.Defend:
                StartDefending();
                break;
            case InputActionType.Defend_Stop:
                while (IsDefStartCo)
                {
                    yield return null;
                }
                StopDefending();
                break;
            case InputActionType.Move_Up:
                break;
            case InputActionType.Move_Down:
                break;
            case InputActionType.Move_Left:
                break;
            case InputActionType.Move_Right:
                break;
            default:
                break;
        }
    }

    public IEnumerator StartSkillAttack(AttackInputType inputSkill)
    {
        ScriptableObjectAttackBase nxtAtk = null;

        switch (inputSkill)
        {
            case AttackInputType.Skill1:
                nxtAtk = CharOwner.CharInfo.Mask.Skill1;
                break;
            case AttackInputType.Skill2:
                nxtAtk = CharOwner.CharInfo.Mask.Skill2;
                break;
            case AttackInputType.Skill3:
                nxtAtk = CharOwner.CharInfo.Mask.Skill3;
                break;
        }

        SkillCoolDownClass scdc = skillCoolDown.Where(r => r.Skill == inputSkill).First();
        if (!CharOwner.GetCanUseStamina(nxtAtk.StaminaCost) || scdc.IsCoGoing)
        {
            SkillActivation = null;
            yield break;
        }
        switch (inputSkill)
        {
            case AttackInputType.Skill1:
                CharOwner.FireActionEvent(CharacterActionType.Skill1);

                break;
            case AttackInputType.Skill2:
                CharOwner.FireActionEvent(CharacterActionType.Skill2);

                break;
            case AttackInputType.Skill3:
                CharOwner.FireActionEvent(CharacterActionType.Skill3);

                break;
        }


        CharOwner.nextAttack = nxtAtk;
        scdc.IsCoGoing = true;
        yield return BattleManagerScript.Instance.WaitUpdate(() => CharOwner.currentAttackPhase != AttackPhasesType.End);
        CharOwner.CharInfo.BaseSpeed *= 100;
        BattleManagerScript.Instance.BattleSpeed = 0.01f;
        CharOwner.SpineAnim.SetSkeletonOrderInLayer(300);
        CharOwner.SetAnimation(nxtAtk.PrefixAnim + "_IdleToAtk", false, 0);
        CharOwner.currentAttackPhase = AttackPhasesType.Start;
        yield return BattleManagerScript.Instance.WaitFor(0.018f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle, () => CharOwner.CharInfo.HealthPerc <= 0);
        if (CharOwner.CharInfo.HealthPerc > 0)
        {
            CharOwner.SetAnimation(nxtAtk.PrefixAnim + "_AtkToIdle", false, 0);
        }
        yield return BattleManagerScript.Instance.WaitUpdate(() => (CharOwner.currentAttackPhase != AttackPhasesType.End || CharOwner.CharInfo.HealthPerc <= 0));
        BattleManagerScript.Instance.BattleSpeed = 1;
        CharOwner.SpineAnim.SetSkeletonOrderInLayer(CharOwner.CharOredrInLayer);
        CharOwner.CharInfo.BaseSpeed /= 100;
        float coolDown = nxtAtk.CoolDown;
        CurrentCharSkillCompletedEvent?.Invoke(inputSkill, coolDown);
        yield return BattleManagerScript.Instance.WaitFor(0.5f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        SkillActivation = null;
        yield return BattleManagerScript.Instance.WaitFor(coolDown - 0.5f, () => BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle);
        scdc.IsCoGoing = false;
    }
}



