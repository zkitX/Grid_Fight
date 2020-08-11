using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using UnityEngine;

public class Stage01_Boss_Script : MinionType_Script
{
    public enum Stage01_Boss_MaskType
    {
        WarDrums,
        LifeDrums,
        MoonDrums,
        CrystalTomb
    }


    public Stage01_Boss_MaskType CurrentPhase;
    public List<BattleTileScript> AttackedTilesList = new List<BattleTileScript>();
    public GameObject MovementPsIn;
    public GameObject MovementPsOut;
    public GameObject FaceChangingWarDrums;
    public GameObject FaceChangingLifeDrums;
    public GameObject FaceChangingMoonDrums;



    BaseCharacter target = null;
    public override IEnumerator AI()
    {
        bool val = true;
        while (val)
        {
            yield return null;
            if (IsOnField)
            {

                while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle)
                {
                    yield return null;
                }
                ScriptableObjectAI prev = CurrentAIState;

                CurrentAIState = CharInfo.GetCurrentAI(AggroInfoList, UMS.CurrentTilePos, this, ref target);

                if (prev == null || prev.AI_Type != CurrentAIState.AI_Type)
                {
                    SetCurrentAIValues();
                    if (prev != null)
                    {
                        prev.ResetStats(CharInfo);

                    }
                    CurrentAIState.ModifyStats(CharInfo);
                 /*   if (psAI != null)
                    {
                        psAI.SetActive(false);
                    }
                    psAI = ParticleManagerScript.Instance.GetParticle(CurrentAIState.AIPs.PSType);
                    psAI.transform.parent = SpineAnim.transform;
                    psAI.transform.localPosition = Vector3.zero;
                    psAI.SetActive(true);*/
                    AICoolDownOffset = 0;
                }

                int atkChances = Random.Range(0, 100);
                nextAttack = null;
                GetAttack();

                if (target != null && atkChances < AttackWillPerc && nextAttack != null && (Time.time - lastAttackTime > nextAttack.CoolDown * UniversalGameBalancer.Instance.difficulty.enemyAttackCooldownScaler))
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
                    int movementChances = Random.Range(0, (TowardMovementPerc + AwayMovementPerc));
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
                    if (possiblePositions.Count > 0 && target != null)
                    {
                        yield return Teleport_Co(possiblePositions);
                    }
                }
                yield return null;
            }
        }
    }

    public IEnumerator Teleport_Co(List<BattleTileScript> positions)
    {
        if ((CharInfo.Health > 0 && !isMoving && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving.ToString() && CharActionlist.Contains(CharacterActionType.Move)) || BattleManagerScript.Instance.VFXScene)
        {
            List<BattleTileScript> prevBattleTile = CurrentBattleTiles;
            List<BattleTileScript> CurrentBattleTilesToCheck = new List<BattleTileScript>();
            Vector2Int nextPos = Vector2Int.left;
            for (int i = 0; i < positions.Count; i++)
            {
                CurrentBattleTilesToCheck = CheckTileAvailabilityUsingPos(positions[i].Pos);
                if(CurrentBattleTilesToCheck.Count > 0)
                {
                    nextPos = positions[i].Pos;
                    break;
                }
            }

            if (CurrentAIState.IdleMovement)
            {
                bool samepos = true;
                for (int i = 0; i < CurrentBattleTilesToCheck.Count; i++)
                {
                    if(!prevBattleTile.Contains(CurrentBattleTilesToCheck[i]))
                    {
                        samepos = false;
                        break;
                    }
                }
                if(samepos)
                {
                    yield break;
                }
            }
            if (CurrentBattleTilesToCheck.Count > 0 &&
                CurrentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
                CurrentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos)).ToList().Count && GridManagerScript.Instance.isPosOnField(nextPos))
            {
                isMoving = true;
                foreach (BattleTileScript item in prevBattleTile)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Empty);
                }
                UMS.CurrentTilePos = nextPos;
                CharOredrInLayer = 101 + (nextPos.x * 10) + (nextPos.y - 12);
                if (CharInfo.UseLayeringSystem)
                {
                    SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
                }

                CurrentBattleTiles = CurrentBattleTilesToCheck;
                UMS.Pos = new List<Vector2Int>();
                foreach (BattleTileScript item in CurrentBattleTilesToCheck)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Occupied);
                    UMS.Pos.Add(item.Pos);
                }

                BattleTileScript resbts = CurrentBattleTiles.Where(r => r.Pos == UMS.CurrentTilePos).First();

                if (resbts != null)
                {
                    foreach (BattleTileScript item in prevBattleTile)
                    {
                        BattleManagerScript.Instance.OccupiedBattleTiles.Remove(item);
                    }
                    BattleManagerScript.Instance.OccupiedBattleTiles.AddRange(CurrentBattleTiles);

                    yield return MoveByTileSpace(resbts.transform.position, new AnimationCurve(), 0);
                }
                else
                {
                    yield break;
                }
            }
        }
    }



    public override IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        if(MovementPsOut == null)
        {
            MovementPsOut = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter01_TohoraSea_Boss_TeleportationOut);
            
        }
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.Footsteps, AudioBus.LowPrio, SpineAnim.transform);
        MovementPsOut.transform.position = transform.position;
        MovementPsOut.SetActive(true);
        float timer = 0;
        bool inOut = false;
        while (MovementPsOut.activeInHierarchy)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
            if(timer > 0.2f && !inOut)
            {
                inOut = true;
                transform.position = new Vector3(100, 100, 100);
            }
        }
        timer = 0;
        if (MovementPsIn == null)
        {
            MovementPsIn = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter01_TohoraSea_Boss_TeleportationIn);
        }
        MovementPsIn.transform.position = nextPos;
        MovementPsIn.SetActive(true);

        while (MovementPsIn.activeInHierarchy)
        {
            yield return null;
            timer += BattleManagerScript.Instance.DeltaTime;
            if (timer > 0.2f && inOut)
            {
                inOut = false;
                transform.position = nextPos;
            }
        }
        isMoving = false;
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if(animState.Contains("Dash"))
        {
            return;
        }

        if(animState.Contains("Atk") || animState.Contains("Charging") || animState.Contains("Loop"))
        {
            switch (nextAttack.AttackInput)
            {
                case AttackInputType.Weak:
                    if (CurrentPhase != Stage01_Boss_MaskType.WarDrums)
                    {
                        if(FaceChangingWarDrums == null)
                        {
                            FaceChangingWarDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter01_TohoraSea_Boss_FaceChanging_WarDrums);
                            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.Skill3.Cast, AudioBus.MidPrio, transform);
                        }
                        FaceChangingWarDrums.transform.parent = SpineAnim.transform;
                        FaceChangingWarDrums.transform.localPosition = Vector3.zero;
                        FaceChangingWarDrums.SetActive(true);
                        CurrentPhase = Stage01_Boss_MaskType.WarDrums;
                    } 
                    break;
                case AttackInputType.Strong:
                    if (CurrentPhase != Stage01_Boss_MaskType.CrystalTomb)
                    {
                      /*  if (FaceChangingWarDrums == null)
                        {
                            FaceChangingWarDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_CrystalTomb_Effect);
                            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.Skill3.Cast, AudioBus.MidPrio, transform);
                        }
                        FaceChangingWarDrums.transform.parent = SpineAnim.transform;
                        FaceChangingWarDrums.transform.localPosition = Vector3.zero;
                        FaceChangingWarDrums.SetActive(true);*/
                        CurrentPhase = Stage01_Boss_MaskType.CrystalTomb;
                    }
                    base.SetAnimation(Stage01_Boss_MaskType.CrystalTomb.ToString() + "_" + animState, loop, transition, _pauseOnLastFrame);
                    return;
                case AttackInputType.Skill1:
                    if (CurrentPhase != Stage01_Boss_MaskType.LifeDrums)
                    {
                        if (FaceChangingLifeDrums == null)
                        {
                            FaceChangingLifeDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter01_TohoraSea_Boss_FaceChanging_LifeDrums);
                            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.Skill3.Cast, AudioBus.MidPrio, transform);
                        }
                        FaceChangingLifeDrums.transform.parent = SpineAnim.transform;
                        FaceChangingLifeDrums.transform.localPosition = Vector3.zero;
                        FaceChangingLifeDrums.SetActive(true);
                        CurrentPhase = Stage01_Boss_MaskType.LifeDrums;
                    }
                    break;
                case AttackInputType.Skill2:
                    if (CurrentPhase != Stage01_Boss_MaskType.MoonDrums)
                    {
                        if (FaceChangingMoonDrums == null)
                        {
                            FaceChangingMoonDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter01_TohoraSea_Boss_FaceChanging_MoonDrums);
                            AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.Skill3.Cast, AudioBus.MidPrio, transform);
                        }
                        FaceChangingMoonDrums.transform.parent = SpineAnim.transform;
                        FaceChangingMoonDrums.transform.localPosition = Vector3.zero;
                        FaceChangingMoonDrums.SetActive(true);
                        CurrentPhase = Stage01_Boss_MaskType.MoonDrums;
                    }
                    break;
            }
        }

        base.SetAnimation(CurrentPhase.ToString() + "_" + animState, loop, transition, _pauseOnLastFrame);
    }

    public override void SpineAnimationState_Complete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "<empty>" || SpineAnim.CurrentAnim == CharacterAnimationStateType.Idle.ToString()
       || SpineAnim.CurrentAnim == CharacterAnimationStateType.Death.ToString())
        {
            return;
        }
        string completedAnim = trackEntry.Animation.Name;

        if (completedAnim.Contains("IdleToAtk") && SpineAnim.CurrentAnim.Contains("IdleToAtk"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            return;
        }

        if (completedAnim.Contains("_Loop") && SpineAnim.CurrentAnim.Contains("_Loop"))
        {
            SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
            currentAttackPhase = AttackPhasesType.End;
            return;
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            Attacking = false;
            if(completedAnim.Contains("Atk2_AtkToIdle"))
            {
                if (FaceChangingWarDrums == null)
                {
                    FaceChangingWarDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Chapter01_TohoraSea_Boss_FaceChanging_WarDrums);
                    AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.Skill3.Cast, AudioBus.MidPrio, transform);
                }
                FaceChangingWarDrums.transform.parent = SpineAnim.transform;
                FaceChangingWarDrums.transform.localPosition = Vector3.zero;
                FaceChangingWarDrums.SetActive(true);
                CurrentPhase = Stage01_Boss_MaskType.WarDrums;
            }
        }

        if (completedAnim.Contains(CharacterAnimationStateType.Arriving.ToString()))
        {
            CharArrivedOnBattleField();
        }


        if (completedAnim != CharacterAnimationStateType.Idle.ToString() && !SpineAnim.Loop)
        {
            SpineAnim.CurrentAnim = CharacterAnimationStateType.Idle.ToString();
            SetAnimation(CharacterAnimationStateType.Idle.ToString(), true);
        }
    }

    public override void AttackedTiles(BattleTileScript bts)
    {
        AttackedTilesList.Add(bts);

    }

    public override void CastAttackParticles()
    {
        GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Cast, CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
          SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position, UMS.Side, nextAttack.AttackInput);

        cast.GetComponent<ParticleHelperScript>().SetSimulationSpeed(CharInfo.BaseSpeed);
        if(nextAttack.AttackInput == AttackInputType.Skill1)
        {
            foreach (VFXOffsetToTargetVOL item in cast.GetComponentsInChildren<VFXOffsetToTargetVOL>())
            {
                item.Target = AttackedTilesList.First().transform;
            }
        }
    }

    public override string GetAttackAnimName()
    {
        return CurrentPhase.ToString() + "_" +  nextAttack.PrefixAnim + (nextAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? "_Loop" : "_AtkToIdle");
    }
}
