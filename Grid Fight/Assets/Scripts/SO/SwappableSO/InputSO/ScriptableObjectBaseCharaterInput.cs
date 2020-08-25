using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ScriptableObjectBaseCharaterInput : ScriptableObjectSwappableBase
{
    public delegate void CurrentCharSkillCompleted(AttackInputType inputSkill, float duration);
    public event CurrentCharSkillCompleted CurrentCharSkillCompletedEvent;


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


    //TEMP
    protected bool temp_Bool;
    protected GameObject tempGameObject = null;
    protected float tempFloat_1;
    protected int tempInt_1, tempInt_2, tempInt_3;
    protected Vector2Int tempVector2Int;
    protected Vector3 tempVector3;
    protected string tempString;
    protected List<Vector2Int> tempList_Vector2int = new List<Vector2Int>();
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

    public virtual ScriptableObjectAttackBase GetRandomAttack()
    {
        return null;
    }
    public virtual void SetCharSelected(bool isSelected, ControllerType player)
    {
    }

    public override void Reset()
    {
        isDefending = false;
        isDefendingStop = false;
        base.Reset();
    }

    public void CallCurrentCharSkillCompleted(AttackInputType inputSkill, float duration)
    {
        CurrentCharSkillCompletedEvent(inputSkill, duration);
    }

    public void RemoveCurrentCharSkillCompleted()
    {
        CurrentCharSkillCompletedEvent = null;
    }
}

