using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stage00_BossOctopus_Tentacles : MinionType_Script
{
   
    public bool CanGetDamage = false;
    private List<VFXOffsetToTargetVOL> TargetControllerList = new List<VFXOffsetToTargetVOL>();
    public Stage00_BossOctopus BaseBoss;

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

    private void Flower_CurrentCharIsRebornEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
       
    }

    private void Flower_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
     
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

        Stage04_BossMonster_Script mask = (Stage04_BossMonster_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass((CharacterNameType.Stage04_BossMonster).ToString(), CharacterSelectionType.Up,
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
}
