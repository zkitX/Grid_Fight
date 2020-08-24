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
    }


    public virtual IEnumerator AI()
    {
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
                prev = CurrentAIState;
                CurrentAIState = CharInfo.GetCurrentAI(AggroInfoList, UMS.CurrentTilePos, this, ref target);
                if (prev == null || prev.AI_Type != CurrentAIState.AI_Type)
                {
                    SetCurrentAIValues();
                    if (prev != null)
                    {
                        prev.ResetStats(CharInfo);

                    }
                    CurrentAIState.ModifyStats(CharInfo);
                    if (CurrentAIState.AIPs != null && CurrentAIState.AIPs.PSType != ParticlesType.None)
                    {
                        if (psAI != null)
                        {
                            psAI.SetActive(false);
                        }
                        psAI = ParticleManagerScript.Instance.GetParticle(CurrentAIState.AIPs.PSType);
                        psAI.transform.parent = SpineAnim.transform;
                        psAI.transform.localPosition = Vector3.zero;
                        psAI.SetActive(true);
                    }

                    AICoolDownOffset = 0;
                }

                tempInt_1 = UnityEngine.Random.Range(0, 100);
                nextAttack = null;
                if (target != null)
                {
                    CharOwner.nextAttack = GetRandomAttack();
                }

                if (target != null && tempInt_1 < AttackWillPerc && nextAttack != null && (Time.time - lastAttackTime > nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler))
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
                                pathFound = false;
                                while (!pathFound)
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
                                                    pathFound = true;
                                                    tempVector2Int = path[0] - UMS.CurrentTilePos;
                                                    possiblePos.isTaken = true;
                                                    yield return MoveCharOnDir_Co(tempVector2Int == Vector2Int.right ? InputDirectionType.Down : tempVector2Int == Vector2Int.left ? InputDirectionType.Up : tempVector2Int == Vector2Int.up ? InputDirectionType.Right : InputDirectionType.Left);
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
                                    tempVector2Int = path[0] - UMS.CurrentTilePos;

                                    yield return MoveCharOnDir_Co(tempVector2Int == Vector2Int.right ? InputDirectionType.Down : tempVector2Int == Vector2Int.left ? InputDirectionType.Up : tempVector2Int == Vector2Int.up ? InputDirectionType.Right : InputDirectionType.Left);
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


    public ScriptableObjectAttackBase GetRandomAttack()
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
        }
    }

    public override void SetCharDead()
    {
        CurrentAIState = null;
        previousAIState = null;
    }
}


