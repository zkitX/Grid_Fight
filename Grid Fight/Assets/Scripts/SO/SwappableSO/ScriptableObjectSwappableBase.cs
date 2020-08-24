using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectSwappableBase : ScriptableObject
{

    public BaseCharacter CharOwner;

    public SwappableActionType SwappableType;

    public virtual void SpineAnimationState_Complete(string completedAnim)
    {

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
}
