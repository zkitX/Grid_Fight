﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage00_BossOctopus_Head : MinionType_Script
{

    public Stage00_BossOctopus bossParent;
    public bool CanGetDamage = true;
    private List<VFXOffsetToTargetVOL> TargetControllerList = new List<VFXOffsetToTargetVOL>();
    public Stage00_BossOctopus BaseBoss;
    public List<Vector3> eyeAttackTarget = new List<Vector3>();
    public bool disabled = false;

    public override void SetUpEnteringOnBattle()
    {
        //StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    public override IEnumerator Move()
    {
        yield return null;
    }

    public override IEnumerator AttackSequence()
    {
        yield return base.AttackSequence();
    }

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        yield return new WaitForFixedUpdate();
    }

    public override void fireAttackAnimation(Vector3 pos)
    {
        eyeAttackTarget.Add(pos);
        base.fireAttackAnimation(pos);
    }

    public override void CastAttackParticles()
    {
        GameObject cast;
        GameObject GOTarget;
        //Debug.Log("Cast");

        for (int i = 0; i < eyeAttackTarget.Count; i++)
        {
            cast = ParticleManagerScript.Instance.FireParticlesInPosition(CharInfo.ParticleID, UMS.CurrentAttackType == AttackType.Particles ? AttackParticlePhaseTypes.CastLeft : AttackParticlePhaseTypes.CastRight,
                NextAttackLevel == CharacterLevelType.Novice ? SpineAnim.FiringPoint.position : SpineAnim.SpecialFiringPoint.position, UMS.Side);
            GOTarget = cast.GetComponentInChildren<ParticleLaserAiming>().Target.transform.gameObject;
            GOTarget.transform.position = eyeAttackTarget[i];
            cast.GetComponent<DisableParticleScript>().SetSimulationSpeed(CharInfo.BaseSpeed);
            LayerParticleSelection lps = cast.GetComponent<LayerParticleSelection>();

            if (lps != null)
            {
                lps.Shot = NextAttackLevel;
                lps.SelectShotLevel();
            }
        }

        if (UMS.CurrentAttackType == AttackType.Particles)
        {
            if (SpineAnim.CurrentAnim.ToString().Contains("Atk1"))
            {
                CharInfo.Stamina -= CharInfo.RapidAttack.Stamina_Cost_Atk;
            }
            else if (SpineAnim.CurrentAnim.ToString().Contains("Atk2"))
            {
                CharInfo.Stamina -= CharInfo.PowerfulAttac.Stamina_Cost_Atk;
            }
        }

        eyeAttackTarget.Clear();
    }

    public override void CharArrivedOnBattleField()
    {
        base.CharArrivedOnBattleField();
    }

    public void SetCharInPos(BaseCharacter currentCharacter, Vector2Int pos)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
        currentCharacter.CurrentBattleTiles = new List<BattleTileScript>();
        for (int i = 0; i < currentCharacter.UMS.Pos.Count; i++)
        {
            currentCharacter.UMS.Pos[i] += bts.Pos;
            BattleTileScript cbts = GridManagerScript.Instance.GetBattleTile(currentCharacter.UMS.Pos[i]);
            currentCharacter.CurrentBattleTiles.Add(cbts);
        }

        foreach (Vector2Int item in currentCharacter.UMS.Pos)
        {
            GridManagerScript.Instance.SetBattleTileState(item, BattleTileStateType.Occupied);
        }
        currentCharacter.SetUpEnteringOnBattle();
        StartCoroutine(BattleManagerScript.Instance.MoveCharToBoardWithDelay(0.2f, currentCharacter, bts.transform.position));
    }

    public override void SetCharDead()
    {
        if (disabled) return;
        CameraManagerScript.Instance.CameraShake();
        Debug.Log("Head Disabled");
        disabled = true;
        CanGetDamage = false;
        SetAnimation(CharacterAnimationStateType.Idle_Disable_Loop, true);
        bossParent.SetCharDead();
    }

    private IEnumerator DeathStasy()
    {
        yield return null;
    }

    public override IEnumerator AttackAction(bool yieldBefore)
    {
        yield break;
    }

    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical)
    {
        if (CanGetDamage)
        {
            return base.SetDamage(damage, elemental, isCritical);
        }
        return false;
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        switch (animState)
        {
            case (CharacterAnimationStateType.Idle):
                transition = 1f;
                break;
            case (CharacterAnimationStateType.Atk1_IdleToAtk):
                transition = 1f;
                break;
            case (CharacterAnimationStateType.Idle_Disable_Loop):
                transition = 1f;
                break;
            case (CharacterAnimationStateType.Death_Prep):
                transition = 1f;
                break;
            case (CharacterAnimationStateType.Atk1_AtkToIdle):
                transition = 10f;
                break;
            case (CharacterAnimationStateType.Atk1_Charging):
                transition = 1f;
                break;
            default:
                break;
        }
        base.SetAnimation(animState, loop, transition);
    }

}
