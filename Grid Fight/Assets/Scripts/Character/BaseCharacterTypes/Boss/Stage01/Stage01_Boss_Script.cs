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

                AttackedTilesList.Clear();
                List<BaseCharacter> enemys = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToList();
                if (enemys.Count > 0)
                {
                    BaseCharacter targetChar = null;
                    List<BaseCharacter> possibleTargets = enemys.Where(r => Mathf.Abs(r.UMS.CurrentTilePos.x - UMS.CurrentTilePos.x) <= 1).ToList();
                    if(possibleTargets.Count > 0)
                    {
                        targetChar = possibleTargets[Random.Range(0, possibleTargets.Count)];
                    }

                    if (targetChar != null)
                    {
                        nextAttackPos = targetChar.UMS.CurrentTilePos;
                        yield return AttackSequence();
                    }
                    else
                    {
                        targetChar = GetTargetChar(enemys);
                        yield return Teleport_Co(targetChar);
                        yield return new WaitForSeconds(1);
                    }
                }
                yield return null;
            }
        }
    }


    public IEnumerator Teleport_Co(BaseCharacter targetChar)
    {
        if ((CharInfo.Health > 0 && !isMoving && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Arriving.ToString() && CharActionlist.Contains(CharacterActionType.Move)) || BattleManagerScript.Instance.VFXScene)
        {
            List<BattleTileScript> prevBattleTile = CurrentBattleTiles;
            List<BattleTileScript> CurrentBattleTilesToCheck = new List<BattleTileScript>();
            Vector2Int nextPos = Vector2Int.left;

            while (CurrentBattleTilesToCheck.Count == 0)
            {
                nextPos = new Vector2Int(Random.Range(targetChar.UMS.CurrentTilePos.x - 1, targetChar.UMS.CurrentTilePos.x + 2), Random.Range(0, 12));
                CurrentBattleTilesToCheck = CheckTileAvailabilityUsingPos(nextPos);
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

                    MoveCo = MoveByTileSpace(resbts.transform.position, new AnimationCurve(), 0);
                    yield return MoveCo;
                }
                else
                {
                    yield break;
                }
            }
        }
    }



    public override IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animLength)
    {
        if(MovementPsOut == null)
        {
            MovementPsOut = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_TeleportationOut);
            
        }
        AudioManagerMk2.Instance.PlaySound(AudioSourceType.Game, CharInfo.AudioProfile.Footsteps, AudioBus.LowPrio, SpineAnim.transform);
        MovementPsOut.transform.position = transform.position;
        MovementPsOut.SetActive(true);
        float timer = 0;
        bool inOut = false;
        while (MovementPsOut.activeInHierarchy)
        {
            yield return null;
            timer += Time.deltaTime;
            if(timer > 0.2f && !inOut)
            {
                inOut = true;
                transform.position = new Vector3(100, 100, 100);
            }
        }
        timer = 0;
        if (MovementPsIn == null)
        {
            MovementPsIn = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_TeleportationIn);
        }
        MovementPsIn.transform.position = nextPos;
        MovementPsIn.SetActive(true);

        while (MovementPsIn.activeInHierarchy)
        {
            yield return null;
            timer += Time.deltaTime;
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
                            FaceChangingWarDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_FaceChanging_WarDrums);
                           
                        }
                        FaceChangingWarDrums.transform.parent = SpineAnim.transform;
                        FaceChangingWarDrums.transform.localPosition = Vector3.zero;
                        FaceChangingWarDrums.SetActive(true);
                        CurrentPhase = Stage01_Boss_MaskType.WarDrums;
                    } 
                    break;
                case AttackInputType.Strong:
                    if (CurrentPhase != Stage01_Boss_MaskType.WarDrums)
                    {
                        if (FaceChangingWarDrums == null)
                        {
                            FaceChangingWarDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_FaceChanging_WarDrums);
                          
                        }
                        FaceChangingWarDrums.transform.parent = SpineAnim.transform;
                        FaceChangingWarDrums.transform.localPosition = Vector3.zero;
                        FaceChangingWarDrums.SetActive(true);
                        CurrentPhase = Stage01_Boss_MaskType.WarDrums;
                    }
                    base.SetAnimation(Stage01_Boss_MaskType.CrystalTomb.ToString() + "_" + animState, loop, transition, _pauseOnLastFrame);
                    return;
                case AttackInputType.Skill1:
                    if (CurrentPhase != Stage01_Boss_MaskType.LifeDrums)
                    {
                        if (FaceChangingLifeDrums == null)
                        {
                            FaceChangingLifeDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_FaceChanging_LifeDrums);
                          
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
                            FaceChangingMoonDrums = ParticleManagerScript.Instance.GetParticle(ParticlesType.Stage01_Boss_FaceChanging_MoonDrums);
                         
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

            //If they can still attack, keep them in the charging loop
            if (shotsLeftInAttack > 0)
            {
                SetAnimation(nextAttack.PrefixAnim + "_Charging", true, 0);
            }
            //otherwise revert them to the idle postion
            else
            {
                SetAnimation(nextAttack.PrefixAnim + "_AtkToIdle");
                currentAttackPhase = AttackPhasesType.End;
            }
            return;
        }

        if (completedAnim.Contains("AtkToIdle") || completedAnim == CharacterAnimationStateType.Atk.ToString() || completedAnim == CharacterAnimationStateType.Atk1.ToString())
        {
            currentAttackPhase = AttackPhasesType.End;
            if (shotsLeftInAttack == 0)
            {
                Attacking = false;
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

        cast.GetComponent<DisableParticleScript>().SetSimulationSpeed(CharInfo.BaseSpeed);
        if(nextAttack.AttackInput == AttackInputType.Skill1)
        {
            foreach (VFXOffsetToTargetVOL item in cast.GetComponentsInChildren<VFXOffsetToTargetVOL>())
            {
                item.Target = AttackedTilesList.First().transform;
            }
        }
    }

}
