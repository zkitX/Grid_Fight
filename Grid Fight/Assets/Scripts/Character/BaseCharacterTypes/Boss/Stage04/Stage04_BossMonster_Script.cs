using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

public class Stage04_BossMonster_Script : MinionType_Script
{

    public GameObject Flower1;

    private IEnumerator CanGetDamageCo;

    private List<Vector2Int> FlowersPos = new List<Vector2Int>()
    {
        new Vector2Int(0,7),
        new Vector2Int(1,10),
        new Vector2Int(3,10),
        new Vector2Int(4,6)
    };
    private List<Stage04_BossMonster_Flower_Script> Flowers = new List<Stage04_BossMonster_Flower_Script>();
    private List<Transform> TargetControllerList = new List<Transform>();
    public bool CanGetDamage = false;


    private Dictionary<CharacterNameType, bool> AreChildrenAlive = new Dictionary<CharacterNameType, bool>()
    {
        { CharacterNameType.Stage04_BossMonster_Minion0, true },
        { CharacterNameType.Stage04_BossMonster_Minion1, true },
        { CharacterNameType.Stage04_BossMonster_Minion2, true },
        { CharacterNameType.Stage04_BossMonster_Minion3, true }
    };

    public override void SetUpEnteringOnBattle()
    {
        StartCoroutine(SetUpEnteringOnBattle_Co());
    }

    private IEnumerator SetUpEnteringOnBattle_Co()
    {
        foreach (FabrikSolver2D item in GetComponentsInChildren<FabrikSolver2D>())
        {
            TargetControllerList.Add(item.transform.GetChild(0));
        }

        SetAnimation(CharacterAnimationStateType.Arriving);

        float timer = 0;
        while (timer <= 9)
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
            Stage04_BossMonster_Flower_Script flower = (Stage04_BossMonster_Flower_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass(CharacterNameType.Stage04_BossMonster_Minion.ToString(), CharacterSelectionType.Up,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage04_BossMonster_Minion, WalkingSideType.RightSide, AttackType.Tile), transform);
            BattleManagerScript.Instance.AllCharactersOnField.Add(flower);
            flower.mfType = (MonsterFlowerType)i;
            flower.UMS.Pos = FlowersPos.GetRange(i, 1);
            flower.BasePos = FlowersPos[i];
            flower.UMS.CurrentTilePos = FlowersPos[i];
            flower.transform.position = transform.position;
            flower.SetUpEnteringOnBattle();
            flower.CurrentCharIsDeadEvent += Flower_CurrentCharIsDeadEvent;
            Flowers.Add(flower);
            Transform t = flower.GetComponentsInChildren<Transform>().Where(r => r.name == "Stage04_BossMonster_Minion_Target").First();
            TargetControllerList[i].parent = t;
            TargetControllerList[i].localPosition = Vector3.zero;
        }
        timer = 0;
        while (timer <= 4)
        {
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState != BattleState.Event))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }
        GetComponentInChildren<LayerParticleSelection>(true).gameObject.SetActive(true);
        BattleManagerScript.Instance.CurrentBattleState = BattleState.Battle;
    }


    private void Flower_CurrentCharIsDeadEvent(CharacterNameType cName, List<ControllerType> playerController, SideType side)
    {
        if (CanGetDamageCo != null)
        {
            StopCoroutine(CanGetDamageCo);
        }
        CanGetDamageCo = CanGetDamage_Co();
        StartCoroutine(CanGetDamageCo);
    }

    public IEnumerator CanGetDamage_Co()
    {
        CanGetDamage = true;
        GetComponentInChildren<LayerParticleSelection>(true).gameObject.SetActive(false);
        float timer = 0;
        while (timer <= 20)
        {
            yield return new WaitForFixedUpdate();
            while (!VFXTestMode && (BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause))
            {
                yield return new WaitForEndOfFrame();
            }
            timer += Time.fixedDeltaTime;
        }
        CanGetDamage = false;
        GetComponentInChildren<LayerParticleSelection>(true).gameObject.SetActive(true);
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
        if (CanGetDamage)
        {
            return base.SetDamage(damage, elemental);
        }

        return false;
    }

    public override IEnumerator Move()
    {
        yield return null;
    }

    public override void SetCharDead()
    {
        CameraManagerScript.Instance.CameraShake();
        UIBattleManager.Instance.Win.gameObject.SetActive(true);
        Instantiate(UMS.DeathParticles, transform.position, Quaternion.identity);
        base.SetCharDead();
    }
}
