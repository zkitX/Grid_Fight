using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterType_Script : BaseCharacter
{
    public Transform SelectionIndicator;
    public SpriteRenderer SelectionIndicatorSprite;
    public Color SelectionIndicatorColorUnselected;

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


    protected override void SetCharDead()
    {
        base.SetCharDead();
      
    }

    public override void SetUpEnteringOnBattle()
    {
        SetAnimation(CharacterAnimationStateType.Arriving);
    }
    #endregion

    #region Attack
    //Basic attack Action that will start the attack anim every x seconds
    public override IEnumerator AttackAction()
    {
        while (true)
        {
            while (!CanAttack && !VFXTestMode)
            {
                yield return null;
            }

            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || isMoving || isSpecialLoading))
            {
                yield return null;
            }

            isAttackStarted = false;
            isAttackCompletetd = false;
            isAttackGoing = false;
            while (!isAttackCompletetd)
            {
                if (!isAttackStarted)
                {
                    isAttackStarted = true;
                    isAttackGoing = true;
                    SetAnimation(CharacterAnimationStateType.Atk);
                }

                if (isAttackStarted && !isAttackGoing && !isMoving)
                {
                    isAttackGoing = true;
                    SetAnimation(CharacterAnimationStateType.Atk);
                }
                yield return null;
            }


            float timer = 0;
            while (timer <= CharInfo.AttackSpeedRatio)
            {
                yield return new WaitForFixedUpdate();
                while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause))
                {
                    yield return new WaitForEndOfFrame();
                }

                while (isSpecialLoading)
                {
                    yield return new WaitForEndOfFrame();
                    timer = 0;
                }

                timer += Time.fixedDeltaTime;
            }
        }
    }

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
    //used to set the movement of the character on a determinated direction
    public override void MoveCharOnDirection(InputDirection nextDir)
    {
        if (CharInfo.Health > 0 && !isMoving && CanAttack && IsOnField && SpineAnim.CurrentAnim != CharacterAnimationStateType.Atk1)
        {
            List<BattleTileScript> prevBattleTile = CurrentBattleTiles;
            List<BattleTileScript>  CurrentBattleTilesToCheck = new List<BattleTileScript>();
            CharacterAnimationStateType AnimState = CharacterAnimationStateType.Idle;
            AnimationCurve curve = new AnimationCurve();
            List<Vector2Int> nextPos;
            Vector2Int dir = Vector2Int.zero;
            switch (nextDir)
            {
                case InputDirection.Up:
                    dir = new Vector2Int(-1, 0);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.UpMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashUp;
                    break;
                case InputDirection.Down:
                    dir = new Vector2Int(1, 0);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.DownMovementSpeed;
                    AnimState = CharacterAnimationStateType.DashDown;
                    break;
                case InputDirection.Right:
                    dir = new Vector2Int(0, 1);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.RightMovementSpeed;
                    AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                    break;
                case InputDirection.Left:
                    dir = new Vector2Int(0, -1);
                    nextPos = CalculateNextPos(dir);
                    if (GridManagerScript.Instance.AreBattleTilesInControllerArea(nextPos, UMS.WalkingSide))
                    {
                        CurrentBattleTilesToCheck = GridManagerScript.Instance.GetBattleTiles(nextPos, UMS.WalkingSide);
                    }
                    curve = SpineAnim.LeftMovementSpeed;
                    AnimState = UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                    break;
            }

            if (CurrentBattleTilesToCheck.Count > 0 && CurrentBattleTilesToCheck.Where(r=>!UMS.Pos.Contains(r.Pos) && r.BattleTileState == BattleTileStateType.Empty).ToList().Count ==
                CurrentBattleTilesToCheck.Where(r => !UMS.Pos.Contains(r.Pos)).ToList().Count)
            {
                foreach (BattleTileScript item in prevBattleTile)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Empty);
                }
                UMS.CurrentTilePos += dir;
                CurrentBattleTiles = CurrentBattleTilesToCheck;
                UMS.Pos = new List<Vector2Int>();
                foreach (BattleTileScript item in CurrentBattleTilesToCheck)
                {
                    GridManagerScript.Instance.SetBattleTileState(item.Pos, BattleTileStateType.Occupied);
                    UMS.Pos.Add(item.Pos);
                }
                
                if (MoveCo != null)
                {
                    StopCoroutine(MoveCo);
                }
                MoveCo = MoveByTile(CurrentBattleTiles.Where(r=> r.Pos == UMS.CurrentTilePos).First().transform.position, AnimState, curve);
                StartCoroutine(MoveCo);
            }


            if (CurrentBattleTiles.Count > 0)
            {
                foreach (BattleTileScript item in prevBattleTile)
                {
                    BattleManagerScript.Instance.OccupiedBattleTiles.Remove(item);
                }
                BattleManagerScript.Instance.OccupiedBattleTiles.AddRange(CurrentBattleTiles);
            }
        }

    }

    #endregion

    //Used to indicate the character that is selected in the battlefield
    public void SetCharSelected(bool isSelected, Color selectionIndicatorColorSelected)
    {
        transform.GetComponentInChildren<SpriteRenderer>().color = isSelected ? selectionIndicatorColorSelected : SelectionIndicatorColorUnselected;
    }

}

