using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/Move")]
public class ScriptableObjectBaseCharaterBaseMove : ScriptableObjectBaseCharaterAction
{
    public virtual IEnumerator MoveTo(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        yield return null;
    }


    public virtual Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        return new Vector2Int[0];
    }

    protected virtual IEnumerator StartMovement(Vector2Int newPos)
    {
        CharOwner.isMoving = true;
        for (int i = 0; i < CharOwner.UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState(CharOwner.UMS.Pos[i], BattleTileStateType.Empty);
        }
       
        CharOwner.UMS.CurrentTilePos = newPos;
        CharOwner.CharOredrInLayer = 101 + (CharOwner.UMS.CurrentTilePos.x * 10) + (CharOwner.UMS.Facing == FacingType.Right ? CharOwner.UMS.CurrentTilePos.y - 12 : CharOwner.UMS.CurrentTilePos.y);
        if (CharOwner.CharInfo.UseLayeringSystem)
        {
            CharOwner.SpineAnim.SetSkeletonOrderInLayer(CharOwner.CharOredrInLayer);
        }

        for (int i = 0; i < CharOwner.UMS.Pos.Count; i++)
        {
            GridManagerScript.Instance.SetBattleTileState((CharOwner.UMS.Pos[i] - CharOwner.UMS.CurrentTilePos) + newPos, BattleTileStateType.Occupied);
            CharOwner.UMS.Pos[i] = (CharOwner.UMS.Pos[i] - CharOwner.UMS.CurrentTilePos) + newPos;
        }

        yield return MoveByTileSpace(GridManagerScript.Instance.GetBattleTile(newPos).transform.position, CharOwner.curve, CharOwner.CharInfo.SpeedStats.CuttingPerc);//currentmovementSO.MoveByTileSpace
    }
}


