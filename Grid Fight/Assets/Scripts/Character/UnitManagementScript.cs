using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManagementScript : MonoBehaviour
{

    public List<Vector2Int> Pos
    {
        get
        {
            return _Pos;
        }
        set
        {
            _Pos = value;
        }
    }
    public List<Vector2Int> _Pos = new List<Vector2Int>();

    public

        Vector2Int CurrentTilePos
    {
        get
        {
            return _CurrentTilePos;
        }
        set
        {
            _CurrentTilePos = value;
        }
    }
    public Vector2Int _CurrentTilePos;

    public ControllerType PlayerController;
    public UnitBehaviourType UnitBehaviour;
    public SideType Side;
    public FacingType Facing;
    public bool isAIOn;
    public AIScript AI;
    public bool Test = false;
    public CharacterBase CharOwner;


    //Used to decide the side
    public void SetupCharacterSide()
    {
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        switch (matchType)
        {
            case MatchType.PvE:
                if (PlayerController == ControllerType.Enemy)
                {
                    SetUnit(FacingType.Left, SideType.RightSide, UnitBehaviourType.NPC, true);
                }
                else
                {
                    SetUnit(FacingType.Right, SideType.LeftSide, UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PvP:
                if (PlayerController == ControllerType.Player2)
                {
                    SetUnit(FacingType.Left, SideType.RightSide, UnitBehaviourType.ControlledByPlayer);
                }
                else
                {
                    SetUnit(FacingType.Right, SideType.LeftSide, UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PPvE:
                if (PlayerController == ControllerType.Enemy)
                {
                    SetUnit(FacingType.Left, SideType.RightSide, UnitBehaviourType.NPC, true);
                }
                else
                {
                    SetUnit(FacingType.Right, SideType.LeftSide, UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PPvPP:
                if (PlayerController == ControllerType.Player3 || PlayerController == ControllerType.Player4)
                {
                    SetUnit(FacingType.Left, SideType.RightSide, UnitBehaviourType.ControlledByPlayer);
                }
                else
                {
                    SetUnit(FacingType.Right, SideType.LeftSide, UnitBehaviourType.ControlledByPlayer);
                }
                break;
        }
    }

    //Used to set the unit info for the facing,side,unitbehaviour, tag and AI
    public void SetUnit(FacingType facing, SideType side, UnitBehaviourType ubt, bool ai = false)
    {
        Facing = facing;
        if(facing == FacingType.Right)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
        Side = side;
        gameObject.tag = Side.ToString();
        CharOwner.SpineAnim.gameObject.tag = Side.ToString();
        UnitBehaviour = ubt;
        if (ai)
        {
            AI.StartMoveCo();
        }
        else
        {
            AI.StopMoveCo();
        }
    }


    private void Update()
    {
        if(Test)
        {
            Test = false;
            SetUnit(Facing, Side, UnitBehaviour, isAIOn);
        }
    }

}
