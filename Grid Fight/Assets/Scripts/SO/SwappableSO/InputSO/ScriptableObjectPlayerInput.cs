using MyBox;
using PlaytraGamesLtd;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharaqcterInput/PlayerInput")]
public class ScriptableObjectPlayerInput : ScriptableObjectBaseCharaterInput
{
 

    public GameTime battleTime;

    [HideInInspector] public List<SkillCoolDownClass> skillCoolDown = new List<SkillCoolDownClass>()
    {
        new SkillCoolDownClass(AttackInputType.Skill1, false),
        new SkillCoolDownClass(AttackInputType.Skill2, false),
        new SkillCoolDownClass(AttackInputType.Skill3, false)
    };
    public IEnumerator SkillActivation = null;

    public override void Reset()
    {
      
        base.Reset();
    }

    public override void StartInput()
    {
    }

    public override void EndInput()
    {
    }

    public void StartChargingAtk(AttackInputType atkType)
    {
        switch (atkType)
        {
            case AttackInputType.Strong:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.StrongAttack))
                {
                    return;
                }
                CharOwner.StartCoroutine(CharOwner.currentAttackProfile.StartStrongAttack());
                break;
            case AttackInputType.Skill1:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.Skill1) || CharOwner.CharInfo.Mask == null || SkillActivation != null)
                {
                    return;
                }
                SkillActivation = StartSkillAttack(AttackInputType.Skill1);
                CharOwner.StartCoroutine(SkillActivation);

                break;
            case AttackInputType.Skill2:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.Skill2) || CharOwner.CharInfo.Mask == null || SkillActivation != null)
                {
                    return;
                }
                SkillActivation = StartSkillAttack(AttackInputType.Skill2);
                CharOwner.StartCoroutine(SkillActivation);
                break;
            case AttackInputType.Skill3:
                if (!CharOwner.CharActionlist.Contains(CharacterActionType.Skill3) || CharOwner.CharInfo.Mask == null || SkillActivation != null)
                {
                    return;
                }
                SkillActivation = StartSkillAttack(AttackInputType.Skill3);
                CharOwner.StartCoroutine(SkillActivation);
                break;

        }
    }


    public override void CharacterInputHandler(InputActionType action)
    {
        if (!CharOwner.HasBuffDebuff(BuffDebuffStatsType.Rage))
        {
            CharOwner.StartCoroutine(CharacterInputQueue(action));
        }
    }

    IEnumerator CharacterInputQueue(InputActionType action)
    {
        //isSpecialStop = false;
        //if (action == InputActionType.Defend)
        //{
        //    IsDefStartCo = true;
        //}
        while (CharOwner.isMoving)
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


    /*    CharOwner.nextAttack = nxtAtk;
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
        scdc.IsCoGoing = false;*/
    }

    #region Defence

    public void StartDefending()
    {
        if (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || !CharOwner.CharActionlist.Contains(CharacterActionType.Defence) || !canDefend)
        {
            return;
        }
        CharOwner.SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
        CharOwner.SpineAnim.SetAnimationSpeed(defenceAnimSpeedMultiplier);
        if (canDefend && CharOwner.CharInfo.Shield >= UniversalGameBalancer.Instance.defenceCost)
        {
            CharOwner.CharInfo.Shield -= UniversalGameBalancer.Instance.defenceCost;
            isDefendingStop = false;
            isDefending = true;
            DefendingHoldingTimer = 0;
            CharOwner.StartCoroutine(Defending_Co());
        }
        else
        {
            CharOwner.StartCoroutine(RejectDefending_Co());
        }
    }

    private IEnumerator RejectDefending_Co()
    {
        BattleManagerScript.Instance.WaitFor((CharOwner.SpineAnim.GetAnimLenght(CharacterAnimationStateType.Defending) / defenceAnimSpeedMultiplier) * 0.25f);
        CharOwner.SetAnimation(CharacterAnimationStateType.Idle, true);
        yield return null;
    }

    private IEnumerator ReloadDefending_Co()
    {
        NewIManager.Instance.PlayLowShieldIndicatorForCharacter(CharOwner.CharInfo.CharacterID, CharOwner.UMS.Side);
        StopDefending();
        canDefend = false;
        while (CharOwner.CharInfo.ShieldPerc != 100f)
        {
            yield return null;
        }

        NewIManager.Instance.StopLowShieldIndicatorForCharacter(CharOwner.CharInfo.CharacterID, CharOwner.UMS.Side);
        canDefend = true;
    }

    private IEnumerator Defending_Co()
    {
        while (isDefending && CharOwner.CharInfo.Shield > 0f && canDefend)
        {
            if (CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString())
            {
                CharOwner.SetAnimation(CharacterAnimationStateType.Defending, true, 0.0f);
                CharOwner.SpineAnim.SetAnimationSpeed(5);
            }
            yield return null;
            DefendingHoldingTimer += BattleManagerScript.Instance.DeltaTime;
            if (CharOwner.CharInfo.ShieldPerc == 0)
            {
                CharOwner.StartCoroutine(ReloadDefending_Co());
            }
        }
        DefendingHoldingTimer = 0;
    }

    public void StopDefending()
    {
        if (isDefending)
        {
            isDefendingStop = true;
            if (CharOwner.SpineAnim.CurrentAnim.Contains(CharacterAnimationStateType.Defending.ToString()))
            {
                Debug.Log("FINISHED STOP <color=blue>DEFENDING</color>");
                CharOwner.SetAnimation(CharacterAnimationStateType.Idle, true, 0.1f);
            }
        }

    }

    #endregion

    //Used to indicate the character that is selected in the battlefield
    public override void SetCharSelected(bool isSelected, ControllerType player)
    {
        NewIManager.Instance.SetSelected(isSelected, player, CharOwner.CharInfo.CharacterID, CharOwner.UMS.Side);
    }

    public override IEnumerator AttackSequence()
    {
        yield return CharacterInputQueue(InputActionType.Weak);
        while (CharOwner.currentAttackProfile.Attacking)
        {
            yield return null;
        }
    }

    public override void SetAttackReady(bool value)
    {

    }

    public override void SetCharDead()
    {
        Instantiate(CharOwner.UMS.DeathParticles, CharOwner.transform.position, Quaternion.identity);
        CharOwner.BuffsDebuffsList.ForEach(r =>
        {
            if (r.Stat != BuffDebuffStatsType.Zombie)
            {
                r.Duration = 0;
                r.CurrentBuffDebuff.Stop_Co = true;
            }
        }
        );
        CharOwner.SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
        SetAttackReady(false);
        if (battleTime != null) battleTime.isStopped = true;
        base.SetCharDead();
        NewIManager.Instance.UpdateVitalitiesOfCharacter(CharOwner.CharInfo, CharOwner.UMS.Side);
        CharOwner.ResetAudioManager();
        CharOwner.StartCoroutine(CharOwner.ReviveSequencer());
    }

    public override void SetUpLeavingBattle()
    {
        if (battleTime != null)
        {
            battleTime.isStopped = true;
        }

        CharOwner.SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        isDefendingStop = true;
        isDefending = false;
        CharOwner.CurrentPlayerController = ControllerType.None;
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.ExitBattleJump, AudioBus.HighPrio);
        EventManager.Instance?.AddCharacterSwitched(CharOwner);
        base.SetUpLeavingBattle();
    }

    public override void SetUpEnteringOnBattle()
    {
        if (battleTime == null)
        {
           SetBattleTime();
        }
        battleTime.isStopped = false;
        CharOwner.SetAnimation(CharacterAnimationStateType.Arriving);
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.ArrivalSpawn, AudioBus.MidPrio);
        //AudioManager.Instance.PlayGeneric("Arriving_Spawn_20200108_V5");
        EventManager.Instance?.AddCharacterArrival(CharOwner);
    }

    public override bool SetDamage(BaseCharacter attacker, ElementalType elemental, bool isCritical, bool isAttackBlocking, ref float damage)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);
        CharOwner.Sic.HitReceived++;

        damage *= UniversalGameBalancer.Instance.difficulty.enemyDamageScaler;
        temp_Bool = true;
        if (isDefending)
        {
            GameObject go;
            if (DefendingHoldingTimer < CharOwner.CharInfo.ShieldStats.Invulnerability)
            {
                CharOwner.Sic.ReflexExp += damage;
                damage = 0;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldTotalDefence);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.Shield_Full, AudioBus.MidPrio);
                go.transform.position = CharOwner.transform.position;
                CharOwner.CharInfo.Shield -= UniversalGameBalancer.Instance.fullDefenceCost;
                CharOwner.CharInfo.Ether += UniversalGameBalancer.Instance.staminaRegenOnPerfectBlock;
                EventManager.Instance.AddBlock(CharOwner, BlockInfo.BlockType.full);
                CharOwner.Sic.CompleteDefences++;
                ComboManager.Instance.TriggerComboForCharacter(CharOwner.CharInfo.CharacterID, ComboType.Defence, true, CharOwner.transform.position);
                temp_Bool = false;
            }
            else
            {
                CharOwner.Sic.ReflexExp += damage * 0.5f;

                damage = damage - CharOwner.CharInfo.ShieldStats.ShieldAbsorbtion;
                go = ParticleManagerScript.Instance.GetParticle(ParticlesType.ShieldNormal);
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.Shield_Partial, AudioBus.HighPrio);
                go.transform.position = CharOwner.transform.position;
                CharOwner.CharInfo.Shield -= UniversalGameBalancer.Instance.partialDefenceCost;
                EventManager.Instance.AddBlock(CharOwner, BlockInfo.BlockType.partial);
                CharOwner.Sic.Defences++;
                ComboManager.Instance.TriggerComboForCharacter(CharOwner.CharInfo.CharacterID, ComboType.Defence, false);
                damage = damage < 0 ? 1 : damage;
                temp_Bool = false;
            }

            CharOwner.FireActionEvent(CharacterActionType.Defence);
            CharOwner.healthCT = BattleFieldIndicatorType.Defend;

            if (CharOwner.UMS.Facing == FacingType.Left)
            {
                go.transform.localScale = Vector3.one;
            }
            else
            {
                go.transform.localScale = new Vector3(-1, 1, 1);
            }
            NewIManager.Instance.TakeDamageSliceOnCharacter(CharOwner.CharInfo.CharacterID, CharOwner.UMS.Side);
        }

        return temp_Bool;
    }

    public override void SetFinalDamage(BaseCharacter attacker,ref float damage, HitInfoClass hic = null)
    {
        CharOwner.Sic.DamageReceived += damage;
        base.SetFinalDamage(attacker,ref damage, hic);
    }

    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();

        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        switch (matchType)
        {
            case MatchType.PvE:
                CharOwner.UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                break;
            case MatchType.PvP:
                if (CharOwner.UMS.PlayerController.Contains(ControllerType.Player2))
                {
                    CharOwner.UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                }
                else
                {
                    CharOwner.UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PPvE:
                CharOwner.UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                break;
            case MatchType.PPvPP:
                if (CharOwner.UMS.PlayerController.Contains(ControllerType.Player3) && CharOwner.UMS.PlayerController.Contains(ControllerType.Player4))
                {
                    CharOwner.UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                }
                else
                {
                    CharOwner.UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PPPPvE:
                CharOwner.UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                break;
        }
        CharOwner.CharInfo.SetupChar();
    }

    public override void UpdateVitalities()
    {
        NewIManager.Instance.UpdateVitalitiesOfCharacter(CharOwner.CharInfo, CharOwner.UMS.Side);
        base.UpdateVitalities();
    }


    public void SetBattleTime()
    {
        battleTime = new GameTime();
        battleTime.SetupBasics();
        battleTime.isStopped = true;
        CharOwner.StartCoroutine(battleTime.standardTicker);
    }


    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveCurrentCharSkillCompleted();
    }

    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if (completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString() ||
            completedAnim == CharacterAnimationStateType.JumpTransition_OUT.ToString() ||
            completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            CharOwner.IsSwapping = false;
            CharOwner.SwapWhenPossible = false;
            /*  for (int i = 0; i < UMS.Pos.Count; i++)
              {
                  GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
                  UMS.Pos[i] = Vector2Int.zero;
              }*/
            SetAttackReady(false);
            CharOwner.transform.position = new Vector3(100, 100, 100);
        }

        return base.SpineAnimationState_Complete(completedAnim);
    }

    public override bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (CharOwner.SpineAnim == null)
        {
            CharOwner.SpineAnimatorsetup();
        }
        //Debug.Log(animState.ToString());

   
        if (animState.Contains("rriv") || animState.Contains("Transition"))
        {
            CharOwner.ResetAudioManager();
        }

        return base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }


}

public class SkillCoolDownClass
{
    public AttackInputType Skill;
    public bool IsCoGoing;

    public SkillCoolDownClass()
    {

    }

    public SkillCoolDownClass(AttackInputType skill, bool isCoGoing)
    {
        Skill = skill;
        IsCoGoing = isCoGoing;
    }
}