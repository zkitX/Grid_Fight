using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

public class Stage04_BossMonster_Script : BaseCharacter
{

    public GameObject Flower1;
    private List<Vector2Int> FlowersPos = new List<Vector2Int>()
    {
        new Vector2Int(0,6),
        new Vector2Int(0,9),
        new Vector2Int(4,7),
        new Vector2Int(5,10)
    };
    private List<Stage04_BossMonster_Flower_Script> Flowers = new List<Stage04_BossMonster_Flower_Script>();
    private List<Transform> TargetControllerList = new List<Transform>();

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

        yield return new WaitForSecondsRealtime(3);

        for (int i = 0; i < 4; i++)
        {
            Stage04_BossMonster_Flower_Script flower = (Stage04_BossMonster_Flower_Script)BattleManagerScript.Instance.CreateChar(new CharacterBaseInfoClass((CharacterNameType.Stage04_BossGirl_Minion0 + i).ToString(), CharacterSelectionType.A,
                CharacterLevelType.Novice, new List<ControllerType> { ControllerType.Enemy }, CharacterNameType.Stage04_BossGirl_Minion0 + i, WalkingSideType.RightSide), transform);
            BattleManagerScript.Instance.AllCharactersOnField.Add(flower);
            flower.UMS.Pos = FlowersPos.GetRange(i, 1);
            flower.BasePos = FlowersPos[i];
            flower.UMS.CurrentTilePos = FlowersPos[i];
            flower.transform.position = transform.position;
            flower.SetUpEnteringOnBattle();
            Flowers.Add(flower);
            Transform t = flower.GetComponentsInChildren<Transform>().Where(r => r.name == "Stage04_GirlBoss_Minion_Target").First();
            TargetControllerList[i].parent = t;
        }
    }

}
