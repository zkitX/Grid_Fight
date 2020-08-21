using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{


    public delegate void CurrentCharSkillCompleted(AttackInputType inputSkill, float duration);
    public event CurrentCharSkillCompleted CurrentCharSkillCompletedEvent;

    protected bool MoveCoOn = true;
    public bool Atk1Queueing = false;
    [SerializeField] protected bool CharacterJumping = false;
    ManagedAudioSource chargingAudio = null;
    ManagedAudioSource chargingAudioStrong = null;
    public float chargingAttackTimer = 0;
    public GameTime battleTime;

    private List<SkillCoolDownClass> skillCoolDown = new List<SkillCoolDownClass>()
    {
        new SkillCoolDownClass(AttackInputType.Skill1, false),
        new SkillCoolDownClass(AttackInputType.Skill2, false),
        new SkillCoolDownClass(AttackInputType.Skill3, false)
    };
    public IEnumerator SkillActivation = null;
    public float LastAxisValue = 0;

    #region Unity Life Cycles
    public override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo, UMS.Side);
        base.Update();
    }
    #endregion

    #region Setup Character

    /// <summary>
    /// Must be called to set up the battleTime Variable
    /// </summary>
    public void SetBattleTime()
    {
        battleTime = new GameTime();
        battleTime.SetupBasics();
        battleTime.isStopped = true;
        StartCoroutine(battleTime.standardTicker);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        CurrentCharSkillCompletedEvent = null;
    }

    

    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();
            MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
            switch (matchType)
            {
                case MatchType.PvE:
                    UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    break;
                case MatchType.PvP:
                    if (UMS.PlayerController.Contains(ControllerType.Player2))
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    else
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    break;
                case MatchType.PPvE:
                    UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    break;
                case MatchType.PPvPP:
                    if (UMS.PlayerController.Contains(ControllerType.Player3) && UMS.PlayerController.Contains(ControllerType.Player4))
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    else
                    {
                        UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                    }
                    break;
            case MatchType.PPPPvE:
                UMS.SetUnit(UnitBehaviourType.ControlledByPlayer);
                break;
        }
      //  UMS.SelectionIndicator.eulerAngles = new Vector3(0, 0, CharInfo.CharacterSelection == CharacterSelectionType.Up ? 90 :
       //     CharInfo.CharacterSelection == CharacterSelectionType.Down ? -90 :
      //      CharInfo.CharacterSelection == CharacterSelectionType.Left ? 180 : 0);
        CharInfo.SetupChar();

    }


    public override void SetCharDead()
    {
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        BuffsDebuffsList.ForEach(r =>
        {
            if (r.Stat != BuffDebuffStatsType.Zombie)
            {
                r.Duration = 0;
                r.CurrentBuffDebuff.Stop_Co = true;
            }
        }
        );
        SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
        SetAttackReady(false);
        if(battleTime != null)battleTime.isStopped = true;
        base.SetCharDead();
        NewIManager.Instance.UpdateVitalitiesOfCharacter(CharInfo, UMS.Side);
        ResetAudioManager();
        StartCoroutine(ReviveSequencer());
    }


    public void ResetAudioManager()
    {
        if (chargingAudio != null)
        {
            chargingAudio.ResetSource();
            chargingAudio = null;
        }
        if (chargingAudioStrong != null)
        {
            chargingAudioStrong.ResetSource();
            chargingAudioStrong = null;
        }
    }

    IEnumerator ReviveSequencer()
    {
        float timeElapsed = 0f;
        float timeToWait = CharInfo.CharacterRespawnLength;
        while (timeElapsed != timeToWait)
        {
            if (BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle
                || BattleManagerScript.Instance.CurrentBattleState == BattleState.FungusPuppets)
            {
                timeElapsed = Mathf.Clamp(timeElapsed + BattleManagerScript.Instance.DeltaTime, 0f, timeToWait);
            }
            yield return null;
        }
        CharBackFromDeath();
    }

    public void CharBackFromDeath()
    {
        gameObject.SetActive(true);
        CharInfo.HealthStats.Health = CharInfo.HealthStats.Base;
        CharInfo.ShieldStats.Shield = CharInfo.ShieldStats.Base;
        CharInfo.EtherStats.Ether = CharInfo.EtherStats.Base;
        //SET UI OF THE CHARACTER TO ALIVE HERE
        NewIManager.Instance.ToggleUICharacterDead(this, false);
        //NewIManager.Instance.SetUICharacterToButton(this, CharInfo.CharacterSelection);
    }


    public override void SetUpEnteringOnBattle()
    {
        if(battleTime == null)
        {
            SetBattleTime();
        }
        battleTime.isStopped = false;
        SetAnimation(CharacterAnimationStateType.Arriving);
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, BattleManagerScript.Instance.AudioProfile.ArrivalSpawn, AudioBus.MidPrio);
        //AudioManager.Instance.PlayGeneric("Arriving_Spawn_20200108_V5");
        EventManager.Instance?.AddCharacterArrival(this);
       
    }

    public override void SetUpLeavingBattle()
    {
        if(battleTime != null)
        {
            battleTime.isStopped = true;
        }

        SetAnimation(CharacterAnimationStateType.Reverse_Arriving);
        isDefendingStop = true;
        isDefending = false;
        CurrentPlayerController = ControllerType.None;
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Ui, BattleManagerScript.Instance.AudioProfile.ExitBattleJump, AudioBus.HighPrio);
        EventManager.Instance?.AddCharacterSwitched(this);
    }

    #endregion

    #region Attack

   
   
    public bool GetCanUseStamina(float valueRequired)
    {
        if (CharInfo.EtherStats.Ether - valueRequired >= 0) return true;

        UMS.StaminaBarContainer.GetComponentInChildren<Animation>().Play();
        return false;
    }

   
    public override void SetFinalDamage(BaseCharacter attacker, float damage, HitInfoClass hic = null)
    {
        Sic.DamageReceived += damage;
        base.SetFinalDamage(attacker, damage, hic);
    }

    #endregion

    #region Move

    #endregion

    //Used to indicate the character that is selected in the battlefield
    public void SetCharSelected(bool isSelected, ControllerType player)
    {
        NewIManager.Instance.SetSelected(isSelected, player, CharInfo.CharacterID, UMS.Side);
    }


    public override void SpineAnimationState_Event(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        CastLoopImpactAudioClipInfoClass attackTypeAudioInfo = GetAttackAudio();
        if (e.Data.Name.Contains("StopDefending"))
        {
            SpineAnim.SetAnimationSpeed(0);
        }
        else if (e.Data.Name.Contains("FireArrivingParticle"))
        {
            ArrivingEvent();
        }
        else if (e.Data.Name.Contains("FireCastParticle"))
        {
            if (attackTypeAudioInfo != null)
            {
                AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, attackTypeAudioInfo.Cast, AudioBus.LowPrio, transform);
            }

            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Firing;
                CreateParticleAttack();
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;

            }
            FireCastParticles();
        }
        else if (e.Data.Name.Contains("FireBulletParticle"))
        {
            if (SpineAnim.CurrentAnim.Contains("Atk1"))
            {
                currentAttackPhase = AttackPhasesType.Firing;
            }
            else
            {
                currentAttackPhase = AttackPhasesType.Cast_Strong;
                CreateParticleAttack();
            }
        }
        else if (e.Data.Name.Contains("FireTileAttack") && !trackEntry.Animation.Name.Contains("Loop"))
        {
            
        }
        else if(e.Data.Name.Contains("EndLoop"))
        {

        }
    }

    public override void SpineAnimationState_Complete(Spine.TrackEntry trackEntry)
    {

        if (PlayQueuedAnim()) return;
        //Debug.Log(skeletonAnimation.AnimationState.Tracks.ToArray()[trackEntry.TrackIndex].Animation.Name + "   " + CurrentAnim.ToString());
        if (trackEntry.Animation.Name == "<empty>" || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString()
           || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString() || (isMoving && (!trackEntry.Animation.Name.Contains("Dash") && !trackEntry.Animation.Name.Contains("_"))))
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;


        if (completedAnim == CharacterAnimationStateType.Defeat_ReverseArrive.ToString())
        {
            IsSwapping = false;
            SwapWhenPossible = false;
            for (int i = 0; i < UMS.Pos.Count; i++)
            {
                GridManagerScript.Instance.SetBattleTileState(UMS.Pos[i], BattleTileStateType.Empty);
                UMS.Pos[i] = Vector2Int.zero;
            }
            SetAttackReady(false);
            transform.position = new Vector3(100, 100, 100);
            return;
        }


        if (completedAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
        {
            IsSwapping = false;
            SwapWhenPossible = false;
            transform.position = new Vector3(100, 100, 100);
            SetAttackReady(false);
        }

        if (completedAnim == CharacterAnimationStateType.JumpTransition_OUT.ToString())
        {
            transform.position = new Vector3(100, 100, 100);
            SpineAnim.SpineAnimationState.SetAnimation(0, CharacterAnimationStateType.Idle.ToString(), true);
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
            SetAttackReady(false);
            return;
        }


        if (completedAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString() && SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_IdleToAtk.ToString())
        {
            QuickAttack();
            return;
        }
        if (completedAnim == CharacterAnimationStateType.Atk1_Loop.ToString() &&
            SpineAnim.CurrentAnim == CharacterAnimationStateType.Atk1_Loop.ToString())
        {
            Debug.Log("Loop ------- completed     " + Atk1Queueing);
            if (Atk1Queueing)
            {
                QuickAttack();
            }
            else
            {
                SetAnimation(CharacterAnimationStateType.Atk1_AtkToIdle);
                Debug.Log("Atk1_AtkToIdle ------- started     " + Atk1Queueing);
                SpineAnim.SetAnimationSpeed(SpineAnim.GetAnimLenght(CharacterAnimationStateType.Atk1_IdleToAtk) / CharInfo.SpeedStats.IdleToAtkDuration);
            }
            Atk1Queueing = false;
            return;
        }
        if (completedAnim.Contains("Atk2") || completedAnim.Contains("S_Buff") || completedAnim.Contains("S_DeBuff") || completedAnim.Contains("Atk3"))
        {
            if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.ToString().Contains("IdleToAtk"))
            {
                string[] res = completedAnim.Split('_');
                ChargingLoop(res.Length == 2 ? res.First() : res[0] + "_" + res[1]);
                return;
            }
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            Attacking = false;
        }

        base.SpineAnimationState_Complete(trackEntry);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (SpineAnim == null)
        {
            SpineAnimatorsetup();
        }
        //Debug.Log(animState.ToString());

        if (!animState.ToString().Contains("Atk") && !animState.ToString().Contains("S_DeBuff") && !animState.ToString().Contains("S_Buff"))
        {
            currentAttackPhase = AttackPhasesType.End;
            Atk1Queueing = false;
        }

        if((SpineAnim.CurrentAnim.Contains("Atk2_AtkToIdle") || 
            (SpineAnim.CurrentAnim.Contains("S_Buff") && !animState.Contains("S_Buff")) ||
            (SpineAnim.CurrentAnim.Contains("S_DeBuff") && !animState.Contains("S_DeBuff"))) && !animState.Contains("Reverse"))
        {
            return;
        }

        if(animState.Contains("rriv") || animState.Contains("Transition"))
        {
            ResetAudioManager();
        }

        base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);
        Sic.HitReceived++;

        damage *= UniversalGameBalancer.Instance.difficulty.enemyDamageScaler;

        NewIManager.Instance.TakeDamageSliceOnCharacter(CharInfo.CharacterID, UMS.Side);

        return base.SetDamage(attacker ,damage, elemental, isCritical);
    }



    public override IEnumerator AI()
    {
        nextAttack = CharInfo.CurrentAttackTypeInfo.Where(r=> r.PrefixAnim == AttackAnimPrefixType.Atk1).First();
        bool val = true;
        while (val)
        {
            yield return null;
            if (IsOnField && CharInfo.Health > 0)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
                }
                CurrentAIState = CharInfo.AIs[0];
                target = WaveManagerScript.Instance.WaveCharcters.Where(r => r.isActiveAndEnabled && r.IsOnField).ToList().OrderBy(a => (Mathf.Abs(a.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x))).FirstOrDefault();
                   
                SetCurrentAIValues();
                CurrentAIState.ModifyStats(CharInfo);
                AICoolDownOffset = 0;

                tempInt_1 = Random.Range(0, 100);
                if (target != null && tempInt_1 < AttackWillPerc &&  (Time.time - lastAttackTime > nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler))
                {
                    lastAttackTime = Time.time;
                    nextAttackPos = target.UMS.CurrentTilePos;
                    if (possiblePos != null)
                    {
                        possiblePos.isTaken = false;
                        possiblePos = null;
                    }

                    yield return AttackSequence();
                }
                else
                {
                    tempInt_2 = Random.Range(0, 100);
                    if (AreTileNearEmpty() && tempInt_2 < MoveWillPerc)
                    {
                        if (possiblePos == null)
                        {
                            int movementChances = UnityEngine.Random.Range(0, (TowardMovementPerc + AwayMovementPerc));
                            if (TowardMovementPerc > movementChances && (Time.time - AICoolDownOffset) > CurrentAIState.CoolDown)
                            {
                                if (target != null)
                                {
                                    possiblePositions = GridManagerScript.Instance.BattleTiles.Where(r => r.WalkingSide == UMS.WalkingSide &&
                                    r.BattleTileState != BattleTileStateType.NonUsable
                                    ).OrderBy(a => Mathf.Abs(a.Pos.x - target.UMS.CurrentTilePos.x)).ThenBy(b => b.Pos.y).ToList();
                                    AICoolDownOffset = Time.time;
                                }
                            }
                            else if ((Time.time - AICoolDownOffset) > CurrentAIState.CoolDown)
                            {
                                if (target != null)
                                {
                                    possiblePositions = GridManagerScript.Instance.BattleTiles.Where(r => r.WalkingSide == UMS.WalkingSide &&
                                    r.BattleTileState != BattleTileStateType.NonUsable
                                    ).OrderByDescending(a => Mathf.Abs(a.Pos.x - target.UMS.CurrentTilePos.x)).ThenByDescending(b => b.Pos.y).ToList();
                                    AICoolDownOffset = Time.time;
                                }
                            }
                            if (possiblePositions.Count > 0)
                            {
                                found = false;
                                while (!found)
                                {
                                    if (possiblePositions.Count > 0)
                                    {
                                        possiblePos = possiblePositions.First();
                                        if (possiblePos.Pos != UMS.CurrentTilePos)
                                        {
                                            if (possiblePos.BattleTileState == BattleTileStateType.Empty)
                                            {
                                                path = GridManagerScript.Pathfinding.GetPathTo(possiblePos.Pos, UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(UMS.WalkingSide));
                                                if (path != null && path.Length > 0)
                                                {
                                                    found = true;
                                                    Vector2Int move = path[0] - UMS.CurrentTilePos;
                                                    possiblePos.isTaken = true;
                                                    yield return MoveCharOnDir_Co(move == new Vector2Int(1, 0) ? InputDirectionType.Down : move == new Vector2Int(-1, 0) ? InputDirectionType.Up : move == new Vector2Int(0, 1) ? InputDirectionType.Right : InputDirectionType.Left);
                                                }
                                                else
                                                {
                                                    possiblePositions.Remove(possiblePos);
                                                }
                                            }
                                            else
                                            {
                                                possiblePositions.Remove(possiblePos);
                                            }
                                        }
                                        else
                                        {
                                            if (CurrentAIState.IdleMovement > Random.Range(0f,1f))
                                            {
                                                possiblePos = null;
                                                found = true;
                                            }
                                            else
                                            {
                                                if (possiblePositions.Count <= 1)
                                                {
                                                    possiblePos = null;
                                                    found = true;
                                                }
                                                else
                                                {
                                                    possiblePositions.Insert(0, GridManagerScript.Instance.GetFreeBattleTile(possiblePos.WalkingSide));
                                                    yield return null;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        possiblePos = null;
                                        found = true;
                                    }

                                }
                                yield return null;
                            }
                            else
                            {
                                found = true;
                                possiblePos = null;
                            }
                        }
                        else
                        {
                            if (possiblePos.Pos != UMS.CurrentTilePos)
                            {
                                path = GridManagerScript.Pathfinding.GetPathTo(possiblePos.Pos, UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(UMS.WalkingSide));
                                if (path == null || (path != null && path.Length == 1) || possiblePos.Pos == UMS.CurrentTilePos)
                                {
                                    possiblePos.isTaken = false;
                                    possiblePos = null;
                                }
                                if (path.Length > 0)
                                {
                                    Vector2Int move = path[0] - UMS.CurrentTilePos;

                                    yield return MoveCharOnDir_Co(move == new Vector2Int(1, 0) ? InputDirectionType.Down : move == new Vector2Int(-1, 0) ? InputDirectionType.Up : move == new Vector2Int(0, 1) ? InputDirectionType.Right : InputDirectionType.Left);
                                }
                            }
                            else
                            {
                                possiblePos = null;
                            }
                        }

                    }
                    else
                    {
                        if (possiblePos != null)
                        {
                            possiblePos.isTaken = false;
                            possiblePos = null;
                        }
                    }
                }
                yield return null;
            }
            else
            {
                if (possiblePos != null)
                {
                    possiblePos.isTaken = false;
                    possiblePos = null;
                }

            }
        }
    }



    public override IEnumerator AttackSequence()
    {
        yield return CharacterInputQueue(InputActionType.Weak);
        while (Attacking)
        {
            yield return null;
        }
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