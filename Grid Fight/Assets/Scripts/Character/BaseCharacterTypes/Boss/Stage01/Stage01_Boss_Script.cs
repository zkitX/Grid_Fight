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
    public GameObject FaceChangingWarDrums;
    public GameObject FaceChangingLifeDrums;
    public GameObject FaceChangingMoonDrums;


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

    public void AttackedTiles(BattleTileScript bts)
    {
        AttackedTilesList.Add(bts);

    }

    //public override void CastAttackParticles()
    //{
    //    GameObject cast = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Cast, CharInfo.CharacterID, AttackParticlePhaseTypes.Cast,
    //      SpineAnim.FiringPints[(int)nextAttack.AttackAnim].position, UMS.Side, nextAttack.AttackInput);

    //    cast.GetComponent<ParticleHelperScript>().SetSimulationSpeed(CharInfo.BaseSpeed);
    //    if(nextAttack.AttackInput == AttackInputType.Skill1)
    //    {
    //        foreach (VFXOffsetToTargetVOL item in cast.GetComponentsInChildren<VFXOffsetToTargetVOL>())
    //        {
    //            item.Target = AttackedTilesList.First().transform;
    //        }
    //    }
    //}

    public override string GetAttackAnimName()
    {
        return CurrentPhase.ToString() + "_" +  nextAttack.PrefixAnim + (nextAttack.PrefixAnim == AttackAnimPrefixType.Atk1 ? "_Loop" : "_AtkToIdle");
    }
}
