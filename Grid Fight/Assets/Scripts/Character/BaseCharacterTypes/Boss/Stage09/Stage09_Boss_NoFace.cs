using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine;

public class Stage09_Boss_NoFace : MinionType_Script
{
    public Stage09_BossInfo bossInfo = null;
    public Stage09_Boss_Geisha baseForme;
    public bool isImmune = true;
    public bool isDead = false;
    public int intensityLevel = -1;

    public void TransformToNoFace()
    {
        StartCoroutine(NoFaceTransformation());
    }

    IEnumerator NoFaceTransformation()
    {
        Attacking = false;
        isImmune = true;
        baseForme.isImmune = true;
        baseForme.CanAttack = false;

        CanAttack = false;
        SetAnimation("Phase1_GettingHit", false, 0.5f);
        while (!baseForme.SpineAnim.CurrentAnim.Contains("Idle"))
        {
            yield return null;
        }
        SetAnimation("Transformation", false, 0.0f);
        SpineAnim.SetAnimationSpeed(bossInfo.demonFormeIntensityLevels[intensityLevel].transformationSpeedMultiplier);
        while (!baseForme.SpineAnim.CurrentAnim.Contains("Idle"))
        {
            yield return null;
        }
        CharInfo.HealthStats.Base = bossInfo.demonFormeIntensityLevels[intensityLevel].healthAmount;
        CharInfo.Health = CharInfo.HealthStats.Base;
        CharInfo.HealthStats.BaseHealthRegeneration = bossInfo.demonFormeIntensityLevels[intensityLevel].healthDrainRate;
        CharInfo.HealthStats.Regeneration = bossInfo.demonFormeIntensityLevels[intensityLevel].healthDrainRate;
        isImmune = false;
        CanAttack = true;

        baseForme.CharInfo.HealthStats.Regeneration = 0f;
        baseForme.BossPhase = Stage09_Boss_Geisha.bossPhasesType.Monster_;
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        SetAnimation(animState.ToString(), loop, transition);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0)
    {
        baseForme.SetAnimation(animState, loop, transition);
    }

    public override void SetCharDead(bool hasToDisappear = true)
    {
        CameraManagerScript.Instance.CameraShake(CameraShakeType.GettingHit);
        InteruptAttack();
        baseForme.SetOniForme(false);

    }

    public void InteruptAttack()
    {
        Attacking = false;
        shotsLeftInAttack = 0;
        currentAttackPhase = AttackPhasesType.End;
    }

    public override IEnumerator AI()
    {
        yield return NoFaceAI();
    }

    public IEnumerator ActiveAI = null;
    public IEnumerator NoFaceAI()
    {
        while (BattleManagerScript.Instance.PlayerControlledCharacters.Length == 0)
        {
            yield return null;
        }
        while (baseForme.BossPhase != Stage09_Boss_Geisha.bossPhasesType.Monster_)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);

        bool val = true;
        while (val)
        {
            yield return null;     
            if (IsOnField && CanAttack && baseForme.BossPhase == Stage09_Boss_Geisha.bossPhasesType.Monster_ && !isImmune)
            {
                List<BaseCharacter> enemys = BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.IsOnField).ToList();
                BaseCharacter targetChar = enemys[Random.Range(0, enemys.Count)];
                if (targetChar != null)
                {
                    Debug.Log("NOFACE ATTACK");
                    yield return AttackSequence();
                    yield return new WaitForSeconds(Random.Range(bossInfo.demonFormeIntensityLevels[intensityLevel].attackRateRange.x,
                        bossInfo.demonFormeIntensityLevels[intensityLevel].attackRateRange.y));
                }
            }
        }
    }

    public override IEnumerator MoveCharOnDir_Co(InputDirection nextDir)
    {
        yield break; //char doesnt move
        yield return baseForme.MoveCharOnDir_Co(nextDir);
    }
    public override IEnumerator AttackSequence(ScriptableObjectAttackBase atk = null)
    {
        Attacking = true;

        if (atk != null)
        {
            nextAttack = atk;
        }
        else
        {
            GetAttack();
            baseForme.nextAttack = nextAttack;
        }

        //CreateTileAttack();

        string animToFire = "bippidi boppidi";
        switch (nextAttack.AttackAnim)
        {
            case AttackAnimType.Rapid_Atk:
                animToFire = "Atk1_IdleToAtk";
                break;
            case AttackAnimType.Powerful_Atk:
                animToFire = "Atk2_IdleToAtk";
                break;
            case AttackAnimType.Boss_Atk3:
                animToFire = "Atk3_IdleToAtk";
                break;
            default:
                Debug.LogError("This attack animation type does not exist in the geisha, only use ATK or RAPIDATK");
                break;
        }

        currentAttackPhase = AttackPhasesType.Start;
        SetAnimation(animToFire, false, 0f);

        while (Attacking)
        {
            yield return null;
        }

    
        yield break;
    }

    public override IEnumerator AttackAction(bool yieldBefore)
    {
        return base.AttackAction(yieldBefore);
    }

    public override bool SetDamage(float damage, ElementalType elemental, bool isCritical, bool isAttackBlocking)
    {
        if (baseForme.BossPhase == Stage09_Boss_Geisha.bossPhasesType.Monster_ && !isImmune)
        {
            bool boolToReturn = base.SetDamage(damage, elemental, isCritical, isAttackBlocking);
            return boolToReturn;
        }
        return false;
    }

    public override void SpineAnimationState_Complete(TrackEntry trackEntry)
    {
        baseForme.SpineAnimationState_Complete(trackEntry);
    }

    public override void StopMoveCo()
    {
        return;
    }

    public override IEnumerator Move()
    {
        yield break;
    }

    public override void SetAttackReady(bool value)
    {
        return;
    }
}
