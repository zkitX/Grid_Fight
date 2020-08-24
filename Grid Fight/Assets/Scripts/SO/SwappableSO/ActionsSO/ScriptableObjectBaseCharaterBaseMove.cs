using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableObjectBaseCharaterAction/Move")]
public class ScriptableObjectBaseCharaterBaseMove : ScriptableObjectBaseCharaterAction
{
    protected Vector2Int dir;
    protected AnimationCurve curve;
    protected CharacterAnimationStateType animState;

    protected List<Vector2Int> tempList_Vector2int = new List<Vector2Int>();

    public virtual IEnumerator MoveTo(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        yield return null;
    }


    public virtual Vector2Int[] GetMovesTo(Vector2Int[] poses)
    {
        return new Vector2Int[0];
    }



    //                if (animState.ToString() == SpineAnim.CurrentAnim)
    //                { 
    //                    tempFloat_1 = ((SpineAnim.GetAnimLenght(animState) * CharInfo.SpeedStats.LoopPerc) / CharInfo.SpeedStats.TileMovementTime) * CharInfo.SpeedStats.MovementSpeed* CharInfo.BaseSpeed;
    //    SpineAnim.SetAnimationSpeed(tempFloat_1);
    //                    SpineAnim.skeletonAnimation.state.GetCurrent(0).TrackTime = SpineAnim.GetAnimLenght(animState) * CharInfo.SpeedStats.IntroPerc;
    //}
    //                else
    //                {
    //                    SetAnimation(animState.ToString());
    //SpineAnim.SetAnimationSpeed((SpineAnim.GetAnimLenght(animState) * (CharInfo.SpeedStats.IntroPerc + CharInfo.SpeedStats.LoopPerc) / CharInfo.SpeedStats.TileMovementTime) * CharInfo.SpeedStats.MovementSpeed * CharInfo.BaseSpeed);
    //                }

    //FireActionEvent(CharacterActionType.Move);
    //                switch (nextDir)
    //                {
    //                    case InputDirectionType.Up:
    //                        FireActionEvent(CharacterActionType.MoveUp);
    //                        break;
    //                    case InputDirectionType.Down:
    //                        FireActionEvent(CharacterActionType.MoveDown);
    //                        break;
    //                    case InputDirectionType.Left:
    //                        FireActionEvent(CharacterActionType.MoveLeft);
    //                        break;
    //                    case InputDirectionType.Right:
    //                        FireActionEvent(CharacterActionType.MoveRight);
    //                        break;
    //                }

    public virtual IEnumerator StartMovement(InputDirectionType inDir)
    {
        GetDirectionVectorAndAnimationCurve(inDir);
        yield return StartMovement(dir);
    }

    public virtual IEnumerator StartMovement(Vector2Int newPos)
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
        
        yield return MoveByTileSpace(GridManagerScript.Instance.GetBattleTile(newPos).transform.position, curve, CharOwner.CharInfo.SpeedStats.CuttingPerc);//currentmovementSO.MoveByTileSpace
    }

    public virtual IEnumerator MoveByTileSpace(Vector3 nextPos, AnimationCurve curve, float animPerc)
    {
        yield return null;
    }

    public void GetDirectionVectorAndAnimationCurve(InputDirectionType nextDir)
    {
        animState = CharacterAnimationStateType.Idle;
        curve = new AnimationCurve();
        dir = Vector2Int.zero;
        switch (nextDir)
        {
            case InputDirectionType.Up:
                dir = new Vector2Int(-1, 0);
                curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.UpMovement : CharOwner.SpineAnim.Speed_Time_Curves.UpMovement;
                animState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirectionType.Down:
                dir = new Vector2Int(1, 0);
                curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.DownMovement : CharOwner.SpineAnim.Speed_Time_Curves.DownMovement;
                animState = CharacterAnimationStateType.DashDown;
                break;
            case InputDirectionType.Right:
                dir = new Vector2Int(0, 1);
                animState = CharOwner.UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashRight : CharacterAnimationStateType.DashLeft;
                if (animState == CharacterAnimationStateType.DashLeft)
                {
                    curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.BackwardMovement : CharOwner.SpineAnim.Speed_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.ForwardMovement : CharOwner.SpineAnim.Speed_Time_Curves.ForwardMovement;
                }
                break;
            case InputDirectionType.Left:
                dir = new Vector2Int(0, -1);
                animState = CharOwner.UMS.Facing == FacingType.Left ? CharacterAnimationStateType.DashLeft : CharacterAnimationStateType.DashRight;
                if (animState == CharacterAnimationStateType.DashLeft)
                {
                    curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.BackwardMovement : CharOwner.SpineAnim.Speed_Time_Curves.BackwardMovement;
                }
                else
                {
                    curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.ForwardMovement : CharOwner.SpineAnim.Speed_Time_Curves.ForwardMovement;
                }
                break;
            case InputDirectionType.UpLeft:
                dir = new Vector2Int(-1, -1);
                curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.UpMovement : CharOwner.SpineAnim.Speed_Time_Curves.UpMovement;
                animState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirectionType.UpRight:
                dir = new Vector2Int(-1, 1);
                curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.UpMovement : CharOwner.SpineAnim.Speed_Time_Curves.UpMovement;
                animState = CharacterAnimationStateType.DashUp;
                break;
            case InputDirectionType.DownLeft:
                dir = new Vector2Int(1, -1);
                curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.DownMovement : CharOwner.SpineAnim.Speed_Time_Curves.DownMovement;
                animState = CharacterAnimationStateType.DashDown;
                break;
            case InputDirectionType.DownRight:
                dir = new Vector2Int(1, 1);
                curve = CharOwner.SpineAnim.CurveType == MovementCurveType.Space_Time ? CharOwner.SpineAnim.Space_Time_Curves.DownMovement : CharOwner.SpineAnim.Speed_Time_Curves.DownMovement;
                animState = CharacterAnimationStateType.DashDown;
                break;
        }
    }

    protected List<BattleTileScript> CheckTileAvailabilityUsingDir(Vector2Int dir)
    {
        tempList_Vector2int = CalculateNextPosUsingDir(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(CharOwner.UMS.Pos, tempList_Vector2int, CharOwner.UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(tempList_Vector2int, CharOwner.UMS.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    protected List<BattleTileScript> CheckTileAvailabilityUsingPos(Vector2Int dir)
    {
        tempList_Vector2int = CalculateNextPosUsinPos(dir);
        if (GridManagerScript.Instance.AreBattleTilesInControllerArea(CharOwner.UMS.Pos, tempList_Vector2int, CharOwner.UMS.WalkingSide))
        {
            return GridManagerScript.Instance.GetBattleTiles(tempList_Vector2int, CharOwner.UMS.WalkingSide);
        }
        return new List<BattleTileScript>();
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsingDir(Vector2Int direction)
    {
        tempList_Vector2int.Clear();
        CharOwner.UMS.Pos.ForEach(r => tempList_Vector2int.Add(r + direction));
        return tempList_Vector2int;
    }

    //Calculate the next position fro the actual 
    public List<Vector2Int> CalculateNextPosUsinPos(Vector2Int direction)
    {
        tempList_Vector2int.Clear();
        CharOwner.UMS.Pos.ForEach(r => tempList_Vector2int.Add((r - CharOwner.UMS.CurrentTilePos) + direction));
        return tempList_Vector2int;
    }

}


