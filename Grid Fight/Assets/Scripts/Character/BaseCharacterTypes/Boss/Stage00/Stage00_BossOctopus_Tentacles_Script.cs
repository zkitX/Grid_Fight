using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage00_BossOctopus_Tentacles_Script : MinionType_Script
{
    public Stage00_BossOctopus_Script bossParent;
    public bool CanGetDamage = true;
    private List<VFXOffsetToTargetVOL> TargetControllerList = new List<VFXOffsetToTargetVOL>();
    public Stage00_BossOctopus_Script BaseBoss;
    public bool disabled = false;

    public override void SetUpEnteringOnBattle()
    {
       // StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    public override IEnumerator Move()
    {
        yield return null;
    }


    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        yield return new WaitForFixedUpdate();
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
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);
        Debug.Log("Tentacles Disabled");
        disabled = true;
        CanGetDamage = false;
        InteruptAttack();
        SetAnimation(CharacterAnimationStateType.Idle_Disable_Loop, true);
        bossParent.SetCharDead();
    }

    void InteruptAttack()
    {
        shotsLeftInAttack = 0;
        currentAttackPhase = AttackPhasesType.End;
    }

    public override void SpecialAttackImpactEffects(Vector3 tilePos)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.Octopus_Tentacle);
        GameObject effect = ParticleManagerScript.Instance.FireParticlesInPosition(nextAttack.Particles.Right.Hit, CharInfo.CharacterID, AttackParticlePhaseTypes.Hit, tilePos, UMS.Side, nextAttack.AttackInput);
    }

    private IEnumerator DeathStasy()
    {
        yield return null;
    }

    public override IEnumerator AttackSequence(ScriptableObjectAttackBase atk = null)
    {
        yield return base.AttackSequence();
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {

        Debug.Log(animState.ToString());
        SpineAnim.AnimationTransition = 1;
        switch (animState)
        {
            case (CharacterAnimationStateType.Idle):
                transition = 0.5f;
                break;
            case (CharacterAnimationStateType.Atk1_IdleToAtk):
                transition = 0f;
                break;
            case (CharacterAnimationStateType.Idle_Disable_Loop):
                transition = 1f;
                break;
            case (CharacterAnimationStateType.Death_Prep):
                transition = 1f;
                break;
            case (CharacterAnimationStateType.Atk1_AtkToIdle):
                transition = 0.5f;
                break;
            case (CharacterAnimationStateType.Atk1_Charging):
                transition = 0.4f;
                break;
            default:
                break;
        }
        base.SetAnimation(animState, loop, transition);
    }

    public override bool SetDamage(BaseCharacter attacker, float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (CanGetDamage && CharInfo.Health > 0)
        {
            return base.SetDamage(attacker, damage, elemental, isCritical, isAttackBlocking);
        }
        return false;

    }


    public override bool GeneralTestAI()
    {
        return true;
    }
}
