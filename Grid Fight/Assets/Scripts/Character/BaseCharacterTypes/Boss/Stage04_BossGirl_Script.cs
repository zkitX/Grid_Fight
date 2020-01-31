using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.U2D.IK;

public class Stage04_BossGirl_Script : BaseCharacter
{
    public GameObject Flower1;
    public GameObject Flower2;
    public GameObject Flower3;
    public GameObject Flower4;
    public bool CanGetDamage = false;
    private List<Stage04_BossGirl_Flower_Script> Flowers = new List<Stage04_BossGirl_Flower_Script>();
    private List<VFXOffsetToTargetVOL> TargetControllerList = new List<VFXOffsetToTargetVOL>();


    private List<Vector2Int> FlowersPos = new List<Vector2Int>()
    {
        new Vector2Int(0,7),
        new Vector2Int(1,11),
        new Vector2Int(3,11),
        new Vector2Int(4,6)
    };

    private Dictionary<CharacterNameType, bool> AreChildrenAlive = new Dictionary<CharacterNameType, bool>()
    {
        { CharacterNameType.Stage04_BossGirl_Minion0, true },
        { CharacterNameType.Stage04_BossGirl_Minion1, true },
        { CharacterNameType.Stage04_BossGirl_Minion2, true },
        { CharacterNameType.Stage04_BossGirl_Minion3, true }
    };

    public override void SetUpEnteringOnBattle()
    {
        UIBattleFieldManager.Instance.SetUIBattleField(this);
        StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        BattleManagerScript.Instance.CurrentBattleState = BattleState.Event;

        foreach (VFXOffsetToTargetVOL item in GetComponentsInChildren<VFXOffsetToTargetVOL>())
        {
            TargetControllerList.Add(item);
        }

        SetAnimation(CharacterAnimationStateType.Arriving);
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

        for (int i = 0; i < 4; i++)
        {
            Stage04_BossGirl_Flower_Script flower = (Stage04_BossGirl_Flower_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass((CharacterNameType.Stage04_BossGirl_Minion0 + i).ToString(), CharacterSelectionType.A,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage04_BossGirl_Minion0 + i, WalkingSideType.RightSide, AttackType.Tile), transform);
            BattleManagerScript.Instance.AllCharactersOnField.Add(flower);
            flower.UMS.Pos = FlowersPos.GetRange(i, 1);
            flower.BasePos = FlowersPos[i];
            flower.UMS.CurrentTilePos = FlowersPos[i];
            flower.transform.position = transform.position;
            flower.SetUpEnteringOnBattle();
            Flowers.Add(flower);
            flower.CurrentCharIsDeadEvent += Flower_CurrentCharIsDeadEvent;
            flower.CurrentCharIsRebirthEvent += Flower_CurrentCharIsRebornEvent;
            Transform t = flower.GetComponentsInChildren<Transform>().Where(r => r.name == "Stage04_GirlBoss_Minion_Target").First();
            TargetControllerList[i].Target = t;
            TargetControllerList[i].transform.localPosition = Vector3.zero;
        }
        StartAttakCo();
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

    private void Flower_CurrentCharIsRebornEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
        AreChildrenAlive[cName] = true;
        CanGetDamage = false;
        
    }

    private void Flower_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
        AreChildrenAlive[cName] = false;
        if(AreChildrenAlive.Where(r=> r.Value).ToList().Count == 0)
        {
            foreach (Stage04_BossGirl_Flower_Script item in Flowers)
            {
                item.CanRebirth = false;
                CanGetDamage = true;
                GetComponentInChildren<LayerParticleSelection>(true).gameObject.SetActive(false);

            }
        }
    }

    public void SetCharInPos(BaseCharacter currentCharacter, Vector2Int pos)
    {
        BattleTileScript bts = GridManagerScript.Instance.GetBattleTile(pos);
        currentCharacter.UMS.CurrentTilePos = bts.Pos;
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

   /* public override IEnumerator AttackAction()
    {
        while (true)
        {
            while (!CanAttack && !VFXTestMode)
            {
                yield return null;
            }

            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Battle || !CanGetDamage))
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
    }*/

    public override bool SetDamage(float damage, ElementalType elemental)
    {
        if(CanGetDamage)
        {
            return base.SetDamage(damage, elemental);
        }
        return false;
    }
}
