using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSwappableBase : ScriptableObject
{

    public BaseCharacter CharOwner;

    public SwappableActionType SwappableType;

    public virtual bool SpineAnimationState_Complete(string completedAnim)
    {
        return false;
    }
    public virtual bool SetAnimation(string animState, bool loop = false, float transition = 0, bool _pauseOnLastFrame = false)
    {
        if (CharOwner.isMoving && (animState.ToString() != CharacterAnimationStateType.Reverse_Arriving.ToString() && animState.ToString() != CharacterAnimationStateType.Defeat_ReverseArrive.ToString()) && (!animState.ToString().Contains("Dash")))
        {
            return true;
        }

        return false;
    }


    public virtual void Reset()
    {

    }

    public virtual void SetUpEnteringOnBattle()
    {

    }

    public virtual void SetUpLeavingBattle()
    {

    }

    public virtual void SetAttackReady(bool value)
    {

    }

    public virtual void SetCharDead()
    {

    }

    public virtual void CharArrivedOnBattleField()
    {

    }


    public virtual void SetFinalDamage(BaseCharacter attacker,ref float damage, HitInfoClass hic = null)
    {
    }

    public virtual bool SetDamage(BaseCharacter attacker, ElementalType elemental, bool isCritical, bool isAttackBlocking, ref float damage)
    {
        return true;
    }

    public virtual void SetupCharacterSide()
    {
    }

    public virtual void UpdateVitalities()
    {

    }

    public virtual void OnDestroy()
    {
    }

  
}