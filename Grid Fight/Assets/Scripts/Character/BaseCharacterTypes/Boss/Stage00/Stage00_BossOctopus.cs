using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage00_BossOctopus : MinionType_Script
{
    public bool CanGetDamage = false;
    private List<MinionType_Script> Pieces = new List<MinionType_Script>();
    public bool IsCharArrived = false;
    public bool DialogueComplete = false;

    private List<CharacterNameType> piecesType = new List<CharacterNameType>()
    {
        CharacterNameType.Stage00_BossOctopus_Head,
        CharacterNameType.Stage00_BossOctopus_Tentacles,
        CharacterNameType.Stage00_BossOctopus_Girl
    };



    public override void SetUpEnteringOnBattle()
    {
        StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    public override IEnumerator Move()
    {
        yield return null;
    }

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        BattleManagerScript.Instance.CurrentBattleState = BattleState.Event;




        foreach (CharacterNameType piece in piecesType)
        {
            Pieces.Add(CreatePiece(piece));
            
        }


        SetAnimation(CharacterAnimationStateType.Arriving);

        while(!IsCharArrived)
        {
            yield return null;
        }


        WaveManagerScript.Instance.BossArrived(this);

        while (!DialogueComplete)
        {
            yield return null;
        }

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


        foreach (MinionType_Script piece in Pieces)
        {
            piece.StartAttakCo();
        }

        timer = 0;
        while (timer <= 3)
        {
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }

        BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
    }

    private MinionType_Script CreatePiece(CharacterNameType pieceType)
    {
        MinionType_Script piece = (MinionType_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(pieceType.ToString(), CharacterSelectionType.A,
             CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, pieceType, WalkingSideType.RightSide, AttackType.Tile), transform);
        piece.UMS.Pos = UMS.Pos;
        piece.UMS.CurrentTilePos = UMS.CurrentTilePos;
        piece.SetValueFromVariableName("BaseBoss", this);
        return piece;
    }





    private void Flower_CurrentCharIsRebornEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
        
    }

    private void Flower_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
    }

    

    public override void SetCharDead()
    {
        if(SpineAnim.CurrentAnim != CharacterAnimationStateType.Death)
        {
            CameraManagerScript.Instance.CameraShake();
            BattleManagerScript.Instance.CurrentBattleState = BattleState.Event;
            ParticleManagerScript.Instance.AttackParticlesFired.ForEach(r => r.PS.SetActive(false));
            ParticleManagerScript.Instance.ParticlesFired.ForEach(r => r.PS.SetActive(false));
            StartCoroutine(DeathStasy());
        }
    }

    private IEnumerator DeathStasy()
    {
        float timer = 0;
        SetAnimation(CharacterAnimationStateType.Death);
        while (timer < 7f)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event)
            {
                yield return new WaitForFixedUpdate();
            }

            timer += Time.fixedDeltaTime;
        }

        Stage04_BossMonster_Script mask = (Stage04_BossMonster_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass((CharacterNameType.Stage04_BossMonster).ToString(), CharacterSelectionType.A,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage04_BossMonster, WalkingSideType.RightSide, AttackType.Tile), WaveManagerScript.Instance.transform);
        BattleManagerScript.Instance.AllCharactersOnField.Add(mask);
        mask.UMS.Pos = UMS.Pos;
        mask.UMS.CurrentTilePos = UMS.CurrentTilePos;
        mask.transform.position = transform.position;
        mask.SetUpEnteringOnBattle();
        timer = 0;
        while (timer < 3)
        {
            yield return new WaitForFixedUpdate();
            while (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event)
            {
                yield return new WaitForFixedUpdate();
            }

            timer += Time.fixedDeltaTime;
        }
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        base.SetCharDead();
    }

    public override IEnumerator AttackAction(bool yieldBefore)
    {
        yield break;
    }

    public override bool SetDamage(float damage, ElementalType elemental)
    {
        if(CanGetDamage)
        {
            return base.SetDamage(damage, elemental);
        }
        return false;
    }


    public override void SetAnimation(CharacterAnimationStateType animState, bool loop = false, float transition = 0)
    {
        foreach (MinionType_Script piece in Pieces)
        {
            piece.SetAnimation(animState, loop, transition);
        }
    }


    private void SetAnimForSinglePiece()
    {

    }
}
