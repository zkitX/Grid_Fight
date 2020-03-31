using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage09_Boss_Geisha : MinionType_Script
{
    bool IsCharArrived = false;
    bool oniFormeActive = false;
    Collider hitbox;
    Stage09_Boss_NoFace oniForme;

    #region Initial Setup

    public override void Start()
    {
        GenerateBoss();

        StartCoroutine(DelayedSetupSequence());
    }

    void GenerateBoss()
    {
        hitbox = GetComponent<BoxCollider>();
        oniForme = (Stage09_Boss_NoFace)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(CharacterNameType.Stage09_Boss_NoFace.ToString(), CharacterSelectionType.Up,
        CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage09_Boss_NoFace, WalkingSideType.RightSide, AttackType.Tile, BaseCharType.None), transform);
        oniForme.UMS.Pos = UMS.Pos;
        oniForme.UMS.EnableBattleBars(false);
        oniForme.UMS.CurrentTilePos = UMS.CurrentTilePos;
        oniForme.SetValueFromVariableName("BaseBoss", this);
        SetOniForme(false);
    }

    IEnumerator DelayedSetupSequence()
    {
        while (UIBattleManager.Instance == null || !IsOnField)
        {
            yield return null;
        }
        SetupBossHealthBar();
    }

    protected void SetupBossHealthBar()
    {
        if (!UIBattleManager.Instance.UIBoss.gameObject.activeInHierarchy)
        {
            UIBattleManager.Instance.UIBoss.gameObject.SetActive(true);
        }
        UIBattleManager.Instance.UIBoss.UpdateHp((100f * CharInfo.HealthStats.Health) / CharInfo.HealthStats.Base);
    }

    #endregion

    #region Enquiries


    #endregion

    #region Entering and Exiting

    public override void SetUpEnteringOnBattle()
    {
        StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        UMS.EnableBattleBars(false);

        //SetAnimation(CharacterAnimationStateType.Arriving);
        //DO THE ARRIVING ANIMATION FOR THE 

        while (!IsCharArrived)
        {
            yield return null;
        }

        WaveManagerScript.Instance.BossArrived(this);

        oniForme.UMS.Pos = UMS.Pos;
        oniForme.UMS.CurrentTilePos = UMS.CurrentTilePos;

        SetAttackReady(true);
        float timer = 0;
        while (timer <= 3)
        {
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }
    }

    public override void SetUpLeavingBattle()
    {

    }

    #endregion

    #region Functionality

    public void SetOniForme(bool state)
    {
        oniForme.gameObject.SetActive(true);
        oniFormeActive = state;
        //SetFormeAttackReady(!state);
        if (state)
        {
            oniForme.CharInfo.Health = oniForme.CharInfo.HealthStats.Base;
            SetAnimation("Transformation", true);
        }
        else
        {
            SetAnimation(CharacterAnimationStateType.Death, true);
        }
    }

    protected void SetFormeAttackReady(MinionType_Script forme, bool value)
    {
        if (value)
        {
            StartCoroutine(AI());
        }
        CharInfo.DefenceStats.BaseDefence = Random.Range(0.7f, 1);
        if (CharBoxCollider != null)
        {
            forme.CharBoxCollider.enabled = value;
        }
        forme.CanAttack = value;
        currentAttackPhase = AttackPhasesType.End;
        CharOredrInLayer = 101 + (UMS.CurrentTilePos.x * 10) + (UMS.Facing == FacingType.Right ? UMS.CurrentTilePos.y - 12 : UMS.CurrentTilePos.y);
        if (CharInfo.UseLayeringSystem)
        {
            SpineAnim.SetSkeletonOrderInLayer(CharOredrInLayer);
        }
    }

    protected override IEnumerator MoveCharOnDir_Co(InputDirection nextDir)
    {
        yield return base.MoveCharOnDir_Co(nextDir);
        oniForme.UMS.Pos = UMS.Pos;
        oniForme.UMS.CurrentTilePos = UMS.CurrentTilePos;
    }

    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        base.SetAnimation(animState, loop, transition);
    }

    public override void SetAnimation(string animState, bool loop = false, float transition = 0)
    {
        base.SetAnimation(oniFormeActive && animState != "Transformation" ? "Monster_" : "Phase1_" + animState.ToString(), loop, transition);
    }

    #endregion

    //NULLIFIED FIELDS

    protected override void Update()
    {

    }
}