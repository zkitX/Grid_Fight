using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/Move")]
public class ScriptableObjectBaseCharaterMove : ScriptableObjectBaseCharaterBaseMove
{
    public override IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        float timer = 0;
        float spaceTimer = 0;
        bool isMovCheck = false;
        Vector3 offset = CharOwner.spineT.position;
        CharOwner.transform.position = nextPos;
        CharOwner.spineT.position = offset;
        Vector3 localoffset = CharOwner.spineT.localPosition;

        while (timer < 1)
        {
            yield return BattleManagerScript.Instance.WaitFixedUpdate(() => BattleManagerScript.Instance.CurrentBattleState == BattleState.Pause);
            timer += (BattleManagerScript.Instance.FixedDeltaTime / (CharOwner.CharInfo.SpeedStats.TileMovementTime / (CharOwner.CharInfo.SpeedStats.MovementSpeed * CharOwner.CharInfo.SpeedStats.BaseSpeed * BattleManagerScript.Instance.MovementMultiplier)));
            spaceTimer = curve.Evaluate(timer);
            CharOwner.spineT.localPosition = Vector3.Lerp(localoffset, CharOwner.LocalSpinePosoffset, spaceTimer);

            if (timer > animPerc && !isMovCheck)
            {
                isMovCheck = true;
                CharOwner.isMoving = false;
                CharOwner.Invoke_TileMovementCompleteEvent();
            }

            if (CharOwner.SpineAnim.CurrentAnim == CharacterAnimationStateType.Reverse_Arriving.ToString())
            {
                CharOwner.isMoving = false;
                CharOwner.Invoke_TileMovementCompleteEvent();
                CharOwner.spineT.localPosition = CharOwner.LocalSpinePosoffset;
                yield break;
            }
        }
        CharOwner.spineT.localPosition = CharOwner.LocalSpinePosoffset;
    }


    public override Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        Vector2Int[] path;
        for (int i = 0; i < poses.Length; i++)
        {
            path = GridManagerScript.Pathfinding.GetPathTo(poses[i], CharOwner.UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(CharOwner.UMS.WalkingSide));
            if(path.Length > 0)
            {
                return path;
            }
        }
        return GridManagerScript.Pathfinding.GetPathTo(poses[0], CharOwner.UMS.Pos, GridManagerScript.Instance.GetWalkableTilesLayout(CharOwner.UMS.WalkingSide));
    }


    public override List<BattleTileScript> CheckTileAvailabilityUsingDir(Vector2Int dir)
    {
        tempList_Vector2int = CalculateNextPosUsingDir(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(CharOwner.UMS.Pos, tempList_Vector2int, CharOwner.UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(tempList_Vector2int, CharOwner.UMS.WalkingSide);
        }
        return base.CheckTileAvailabilityUsingDir(dir);
    }

}


