using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ScriptableObjectBaseCharaterInput : ScriptableObjectSwappableBase
{


    #region Defence Variables
    public bool isDefending
    {
        get
        {
            return isDefendingStop ? false : _isDefending;
        }
        set
        {
            _isDefending = value;
        }
    }
    protected bool _isDefending = false;
    public bool isDefendingStop = false;

    protected float DefendingHoldingTimer = 0;

    protected bool canDefend = true;
    protected float defenceAnimSpeedMultiplier = 5f;
    #endregion

    public override void SetCharDead()
    {
        isDefendingStop = true;
        isDefending = false;
        base.SetCharDead();
    }

    public virtual void StartInput()
    {
    }

    public virtual void EndInput()
    {
    }

    public virtual IEnumerator AttackSequence()
    {
        yield return null;
    }

    public virtual void CharacterInputHandler(InputActionType action)
    {
    }
}

