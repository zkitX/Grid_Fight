using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharaqcterInput/AIInput")]
public class ScriptableObjectAIInput : ScriptableObjectBaseCharaterInput
{
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
                    GetAttack();
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
                    tempInt_2 = UnityEngine.Random.Range(0, 100);
                    if (AreTileNearEmpty() && tempInt_2 < MoveWillPerc)
                    {
                        if (possiblePos == null)
                        {
                            tempInt_1 = UnityEngine.Random.Range(0, (TowardMovementPerc + AwayMovementPerc));
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
}


