using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlaytraGamesLtd;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharaqcterInput/AIInput")]
public class ScriptableObjectAIInput : ScriptableObjectBaseCharaterInput
{
    //NPC BEHAVIOUR
    [Header("NPC Behaviour")]
    public int AttackWillPerc = 13;
    public int MoveWillPerc = 100;
    public int UpDownMovementPerc = 13;
    public int TowardMovementPerc = 13;
    public int AwayMovementPerc = 13;

    //AI STATE
    [HideInInspector] public ScriptableObjectAI CurrentAIState;
    protected ScriptableObjectAI previousAIState;
    public IEnumerator AICo = null;

    protected List<AggroInfoClass> AggroInfoList = new List<AggroInfoClass>();
    protected BaseCharacter target = null;
    protected List<HitInfoClass> HittedByList = new List<HitInfoClass>();
    protected HitInfoClass LastHitter
    {
        get
        {
            HitInfoClass lastHitter = null;
            foreach (HitInfoClass hitter in HittedByList)
            {
                if (lastHitter == null || lastHitter.TimeLastHit < hitter.TimeLastHit) lastHitter = hitter;
            }
            return lastHitter;
        }
    }

    [HideInInspector] public Vector2Int nextAttackPos;
    protected float lastAttackTime = 0;
    public float AICoolDownOffset = 0;

    //Visuals
    protected bool strongAnimDone = false;
    protected GameObject psAI = null;

    //PATHFINDING
    protected BattleTileScript possiblePos = null;
    protected Vector2Int[] path;
    protected bool pathFound = false;
    protected List<BattleTileScript> possiblePositions = new List<BattleTileScript>();

    //Temp
    protected float totDamage = 0;
    

    public override void SetUpEnteringOnBattle()
    {
        CharOwner.SetAnimation(CharacterAnimationStateType.Arriving);
        base.SetUpEnteringOnBattle();
    }

    protected void SetCurrentAIValues()
    {
        AttackWillPerc = CurrentAIState.AttackWill;
        MoveWillPerc = CurrentAIState.MoveWill;
        TowardMovementPerc = CurrentAIState.Chaseing_Flee;
    }


    public override void StartInput()
    {
        AICo = AI();
        CharOwner.StartCoroutine(AICo);
    }

    public override void EndInput()
    {
        CharOwner.StopCoroutine(AICo);
    }

    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();
        CharOwner.UMS.SetUnit(UnitBehaviourType.NPC);
    }
    public bool AreTileNearEmpty()
    {
        List<BattleTileScript> res = CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.up);
        res.AddRange(CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.down));
        res.AddRange(CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.left));
        res.AddRange(CharOwner.currentMoveProfile.CheckTileAvailabilityUsingDir(Vector2Int.right));

        if (res.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual IEnumerator AI()
    {
        bool val = true;
        while (val)
        {
            yield return null;
            if (CharOwner.IsOnField && CharOwner.CharInfo.Health > 0)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
                }
                previousAIState = CurrentAIState;
                CurrentAIState = CharOwner.CharInfo.GetCurrentAI(AggroInfoList, CharOwner.UMS.CurrentTilePos, CharOwner, ref target);
                if (previousAIState == null || previousAIState.AI_Type != CurrentAIState.AI_Type)
                {
                    SetCurrentAIValues();
                    if (previousAIState != null)
                    {
                        previousAIState.ResetStats(CharOwner.CharInfo);

                    }
                    CurrentAIState.ModifyStats(CharOwner.CharInfo);
                    if (CurrentAIState.AIPs != null && CurrentAIState.AIPs.PSType != ParticlesType.None)
                    {
                        if (psAI != null)
                        {
                            psAI.SetActive(false);
                        }
                        psAI = ParticleManagerScript.Instance.GetParticle(CurrentAIState.AIPs.PSType);
                        psAI.transform.parent = CharOwner.SpineAnim.transform;
                        psAI.transform.localPosition = Vector3.zero;
                        psAI.SetActive(true);
                    }

                    AICoolDownOffset = 0;
                }

                tempInt_1 = UnityEngine.Random.Range(0, 100);
                CharOwner.nextAttack = null;
                if (target != null)
                {
                    CharOwner.nextAttack = GetRandomAttack();
                }

                if (target != null && tempInt_1 < AttackWillPerc && CharOwner.nextAttack != null && (Time.time - lastAttackTime > CharOwner.nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler))
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
                            tempInt_1 = Random.Range(0, (TowardMovementPerc + AwayMovementPerc));
                            if (TowardMovementPerc > tempInt_1 && (Time.time - AICoolDownOffset) > CurrentAIState.CoolDown)
                            {
                                if (target != null)
                                {
                                    possiblePositions = GridManagerScript.Instance.BattleTiles.Where(r => r.WalkingSide == CharOwner.UMS.WalkingSide &&
                                    r.BattleTileState != BattleTileStateType.NonUsable
                                    ).OrderBy(a => Mathf.Abs(a.Pos.x - target.UMS.CurrentTilePos.x)).ThenBy(b => b.Pos.y).ToList();
                                    AICoolDownOffset = Time.time;
                                }
                            }
                            else if ((Time.time - AICoolDownOffset) > CurrentAIState.CoolDown)
                            {
                                if (target != null)
                                {
                                    possiblePositions = GridManagerScript.Instance.BattleTiles.Where(r => r.WalkingSide == CharOwner.UMS.WalkingSide &&
                                    r.BattleTileState != BattleTileStateType.NonUsable
                                    ).OrderByDescending(a => Mathf.Abs(a.Pos.x - target.UMS.CurrentTilePos.x)).ThenByDescending(b => b.Pos.y).ToList();
                                    AICoolDownOffset = Time.time;
                                }
                            }
                            if (possiblePositions.Count > 0)
                            {
                                pathFound = false;
                                while (!pathFound)
                                {
                                    if (possiblePositions.Count > 0)
                                    {
                                        possiblePos = possiblePositions.First();
                                        if (possiblePos.Pos != CharOwner.UMS.CurrentTilePos)
                                        {
                                            if (possiblePos.BattleTileState == BattleTileStateType.Empty)
                                            {
                                                path = GridManagerScript.Pathfinding.GetPathTo(possiblePos.Pos, CharOwner.UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(CharOwner.UMS.WalkingSide));
                                                if (path != null && path.Length > 0)
                                                {
                                                    pathFound = true;
                                                    tempVector2Int = path[0] - CharOwner.UMS.CurrentTilePos;
                                                    possiblePos.isTaken = true;
                                                    yield return CharOwner.currentMoveProfile.StartMovement(tempVector2Int == Vector2Int.right ? InputDirectionType.Down : tempVector2Int == Vector2Int.left ? InputDirectionType.Up : tempVector2Int == Vector2Int.up ? InputDirectionType.Right : InputDirectionType.Left);
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
                                            if (CurrentAIState.IdleMovement > UnityEngine.Random.Range(0f, 1f))
                                            {
                                                possiblePos = null;
                                                pathFound = true;
                                            }
                                            else
                                            {
                                                if (possiblePositions.Count <= 1)
                                                {
                                                    possiblePos = null;
                                                    pathFound = true;
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
                                        pathFound = true;
                                    }

                                }
                                yield return null;
                            }
                            else
                            {
                                pathFound = true;
                                possiblePos = null;
                            }
                        }
                        else
                        {
                            if (possiblePos.Pos != CharOwner.UMS.CurrentTilePos)
                            {
                                path = GridManagerScript.Pathfinding.GetPathTo(possiblePos.Pos, CharOwner.UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(CharOwner.UMS.WalkingSide));
                                if (path == null || (path != null && path.Length == 1) || possiblePos.Pos == CharOwner.UMS.CurrentTilePos)
                                {
                                    possiblePos.isTaken = false;
                                    possiblePos = null;
                                }
                                if (path.Length > 0)
                                {
                                    tempVector2Int = path[0] - CharOwner.UMS.CurrentTilePos;

                                    yield return CharOwner.currentMoveProfile.StartMovement(tempVector2Int == Vector2Int.right ? InputDirectionType.Down : tempVector2Int == Vector2Int.left ? InputDirectionType.Up : tempVector2Int == Vector2Int.up ? InputDirectionType.Right : InputDirectionType.Left);
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


    public override ScriptableObjectAttackBase GetRandomAttack()
    {
        ScriptableObjectAttackBase nextAtk = null;

        if (CharOwner.nextSequencedAttacks.Count > 0)
        {
            nextAtk = CharOwner.nextSequencedAttacks[0];
            nextAtk.isSequencedAttack = true;
            return nextAtk;
        }

        ScriptableObjectAttackBase[] nextAttackSequence = CharOwner.CharInfo.NextAttackSequence;
        if (nextAttackSequence != null)
        {
            CharOwner.nextSequencedAttacks = nextAttackSequence.ToList();
            nextAtk = GetRandomAttack();
            return nextAtk;
        }

        CharOwner.currentTileAtks = CharOwner.CharInfo.CurrentAttackTypeInfo.Where(r => r != null && r.CurrentAttackType == AttackType.Tile).ToList();
        CharOwner.availableAtks.Clear();
        for (int i = 0; i < CharOwner.currentTileAtks.Count; i++)
        {
            CharOwner.atkToCheck = CharOwner.currentTileAtks[i];
            switch (CharOwner.atkToCheck.TilesAtk.StatToCheck)
            {
                case StatsCheckType.Health:
                    Utils.CheckStatsValues(new Vector2(CharOwner.atkToCheck.TilesAtk.PercToCheck, 0f), CharOwner.atkToCheck.TilesAtk.ValueChecker, CharOwner.CharInfo.HealthPerc);
                    break;
                case StatsCheckType.Ether:
                    Utils.CheckStatsValues(new Vector2(CharOwner.atkToCheck.TilesAtk.PercToCheck, 0f), CharOwner.atkToCheck.TilesAtk.ValueChecker, CharOwner.CharInfo.EtherPerc);
                    break;
                case StatsCheckType.None:
                    CharOwner.availableAtks.Add(CharOwner.atkToCheck);
                    break;
            }
        }

        int totalchances = 0;

        List<ScriptableObjectAttackBase> resAtkBase = new List<ScriptableObjectAttackBase>();
        CharOwner.availableAtks.ForEach(r =>
        {
            switch (r.Fov)
            {
                case FieldOfViewType.NearRange:
                    if (target.UMS.CurrentTilePos.x == CharOwner.UMS.CurrentTilePos.x)
                    {
                        resAtkBase.Add(r);
                    }
                    break;
                case FieldOfViewType.MidRange:
                    if (Mathf.Abs(target.UMS.CurrentTilePos.x - CharOwner.UMS.CurrentTilePos.x) <= 1)
                    {
                        resAtkBase.Add(r);
                    }
                    break;
                case FieldOfViewType.LongRange:
                    resAtkBase.Add(r);
                    break;
            }
        });

        resAtkBase.ForEach(r =>
        {
            totalchances += r.TilesAtk.Chances;
        });
        int chances = 0;
        int sumc = 0;
        for (int i = 0; i < resAtkBase.Count; i++)
        {
            chances = Random.Range(0, totalchances);
            sumc += resAtkBase[i].TilesAtk.Chances;

            if (chances < sumc)
            {
                return resAtkBase[i];
            }
            totalchances -= sumc;
        }
        return null;
    }


    public override IEnumerator AttackSequence()
    {
        yield return null;
    }

    public override void SetAttackReady(bool value)
    {
        if (value)
        {
            HittedByList.Clear();
            StartInput();
        }
        
    }

    public override void SetCharDead()
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Arrival);
        if (AICo != null)
        {
            CharOwner.StopCoroutine(AICo);
            AICo = null;
        }
        Instantiate(CharOwner.UMS.DeathParticles, CharOwner.transform.position, Quaternion.identity);
        CurrentAIState?.ResetStats(CharOwner.CharInfo);
        CurrentAIState = null;
        previousAIState = null;
        
        for (int i = 0; i < HittedByList.Count; i++)
        {
            StatisticInfoClass sic = StatisticInfoManagerScript.Instance.CharaterStats.Where(r => r.CharacterId == HittedByList[i].CharacterId).FirstOrDefault();
            if (sic != null)
            {
                sic.BaseExp += (HittedByList[i].Damage / totDamage) * CharOwner.CharInfo.ExperienceValue;
            }
        }
        totDamage = 0;


        if (HittedByList.Count > 0)
        {
            ComboManager.Instance.TriggerComboForCharacter(HittedByList[HittedByList.Count - 1].CharacterId, ComboType.Kill, true, CharOwner.transform.position);
        }
        
        switch (CharOwner.DeathAnim)
        {
            case DeathBehaviourType.Explosion:
                for (int i = 0; i < CharOwner.UMS.Pos.Count; i++)
                {
                    GridManagerScript.Instance.SetBattleTileState(CharOwner.UMS.Pos[i], BattleTileStateType.Empty);
                    CharOwner.UMS.Pos[i] = Vector2Int.zero;
                }
                CharOwner.transform.position = new Vector3(100, 100, 100);
                CharOwner.SetAnimation(CharacterAnimationStateType.Idle);
                if (CharOwner.isActiveAndEnabled)
                {
                    CharOwner.StartCoroutine(DisableChar());
                }
                break;
            case DeathBehaviourType.Defeat:
                CharOwner.SetAnimation(CharacterAnimationStateType.Defeat);
                break;
            case DeathBehaviourType.Reverse_Arrives:
                for (int i = 0; i < CharOwner.UMS.Pos.Count; i++)
                {
                    GridManagerScript.Instance.SetBattleTileState(CharOwner.UMS.Pos[i], BattleTileStateType.Empty);
                    CharOwner.UMS.Pos[i] = Vector2Int.zero;
                }
                CharOwner.SetAnimation(CharacterAnimationStateType.Defeat_ReverseArrive);
                break;
        }
        base.SetCharDead();

    }

    private IEnumerator DisableChar()
    {
        yield return BattleManagerScript.Instance.WaitFor(0.5f);
        CharOwner.gameObject.SetActive(false);
    }


    public override bool SetDamage(BaseCharacter attacker, ElementalType elemental, bool isCritical, bool isAttackBlocking, ref float damage)
    {
        if (attacker != this)
        {
            temp_Bool = true;
            float defenceChances = Random.Range(0, 100);
            if (defenceChances < CharOwner.CharInfo.ShieldStats.MinionPerfectShieldChances && !CharOwner.SpineAnim.CurrentAnim.Contains("Atk"))
            {
                isDefending = true;
                DefendingHoldingTimer = 0;
                CharOwner.SetAnimation(CharacterAnimationStateType.Defending);
                damage = 0;
                temp_Bool = false;
            }
            else if (defenceChances < (CharOwner.CharInfo.ShieldStats.MinionPerfectShieldChances + CharOwner.CharInfo.ShieldStats.MinionShieldChances) && !CharOwner.SpineAnim.CurrentAnim.Contains("Atk"))
            {
                isDefending = true;
                DefendingHoldingTimer = 10;
                CharOwner.SetAnimation(CharacterAnimationStateType.Defending);
                damage = damage - CharOwner.CharInfo.ShieldStats.ShieldAbsorbtion;
                temp_Bool = false;
            }
            else
            {
                isDefending = false;
                DefendingHoldingTimer = 0;
            }
        }
        return temp_Bool;
    }

    public override void SetFinalDamage(BaseCharacter attacker,ref float damage, HitInfoClass hic = null)
    {
        hic = HittedByList.Where(r => r.CharacterId == attacker.CharInfo.CharacterID).FirstOrDefault();
        if (hic == null)
        {
            HittedByList.Add(new HitInfoClass(attacker, damage));
        }
        else
        {
            hic.Damage += damage;
        }
        if (attacker.CurrentPlayerController != ControllerType.None && attacker.CurrentPlayerController != ControllerType.Enemy)
        {
            AggroInfoClass aggro = AggroInfoList.Where(r => r.PlayerController == attacker.CurrentPlayerController).FirstOrDefault();
            if (aggro == null)
            {
                AggroInfoList.Add(new AggroInfoClass(attacker.CurrentPlayerController, 1));
            }
            else
            {
                aggro.Hit++;
                AggroInfoList.ForEach(r =>
                {
                    if (r.PlayerController != attacker.CurrentPlayerController)
                    {
                        r.Hit = r.Hit == 0 ? 0 : r.Hit - 1;
                    }
                });
            }
        }

        attacker.Sic.DamageMade += damage;
        base.SetFinalDamage(attacker,ref damage, hic);
    }


    public override bool SpineAnimationState_Complete(string completedAnim)
    {
        if (completedAnim == CharacterAnimationStateType.Defending.ToString())
        {
            isDefending = false;
            isDefendingStop = true;
            DefendingHoldingTimer = 0;
        }
        return base.SpineAnimationState_Complete(completedAnim);
    }

    public override bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if ((BattleManagerScript.Instance.CurrentBattleState == BattleState.Battle && CharOwner.SpineAnim.CurrentAnim.Contains("Defeat")) ||
            (animState == CharacterAnimationStateType.Defending.ToString() && CharOwner.SpineAnim.CurrentAnim.Contains("Defending")) ||
            (animState.Contains("GettingHit") && CharOwner.SpineAnim.CurrentAnim.Contains("GettingHit"))) 
        {
            return true;
        }

        return base.SetAnimation(animState, loop, transition, _pauseOnLastFrame);
    }
}


[System.Serializable]
public class HitInfoClass
{
    public BaseCharacter hitter = null;
    public CharacterNameType CharacterId;
    public float Damage;
    public float TimeLastHit = 0;

    public HitInfoClass()
    {
        TimeLastHit = Time.time;
    }

    public HitInfoClass(BaseCharacter character, float damage)
    {
        hitter = character;
        CharacterId = character.CharInfo.CharacterID;
        Damage = damage;
        TimeLastHit = Time.time;
    }

    public void UpdateLastHitTime()
    {
        TimeLastHit = Time.time;
    }
}

[System.Serializable]
public class AggroInfoClass
{
    public ControllerType PlayerController;
    public int Hit;

    public AggroInfoClass()
    {

    }

    public AggroInfoClass(ControllerType playerController, int hit)
    {
        PlayerController = playerController;
        Hit = hit;
    }
}
