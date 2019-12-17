﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{
  

    #region Unity Life Cycles
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    #endregion

    #region Setup Character
    public override void SetupCharacterSide()
    {
        base.SetupCharacterSide();
        transform.GetChild(0).eulerAngles = new Vector3(0, 0, CharInfo.CharacterSelection == CharacterSelectionType.Up ? 90 :
            CharInfo.CharacterSelection == CharacterSelectionType.Down ? -90 :
            CharInfo.CharacterSelection == CharacterSelectionType.Left ? 180 : 0);
    }


    public override void SetCharDead()
    {
        base.SetCharDead();
      
    }

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
    }
    #endregion

    #region Attack
   

    //Load the special attack and fire it if the load is complete
    public IEnumerator LoadSpecialAttack()
    {
        if (CharInfo.StaminaStats.Stamina - CharInfo.StaminaStats.Stamina_Cost_S_Atk01 >= 0)
        {
            isSpecialLoading = true;
            float timer = 0;
            while (isSpecialLoading && !VFXTestMode)
            {
                while (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause)
                {
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
            }
            if (IsOnField || VFXTestMode)
            {
                while (isMoving)
                {
                    yield return new WaitForEndOfFrame();
                }
                
                SpecialAttack(CharacterLevelType.Defiant);
            }
        }
    }

    //Set ste special attack
    public void SpecialAttack(CharacterLevelType attackLevel)
    {
        NextAttackLevel = attackLevel;
        SetAnimation(CharacterAnimationStateType.Atk1);
    }

    #endregion

    #region Move

    #endregion

    //Used to indicate the character that is selected in the battlefield
    public void SetCharSelected(bool isSelected, Color selectionIndicatorColorSelected)
    {
        UMS.SelectionIndicatorSprite.color = isSelected ? selectionIndicatorColorSelected : UMS.SelectionIndicatorColorUnselected;
    }

}
