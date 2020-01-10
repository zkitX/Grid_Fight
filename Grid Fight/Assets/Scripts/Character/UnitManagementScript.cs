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

    public List<ControllerType> PlayerController = new List<ControllerType>();
    public UnitBehaviourType UnitBehaviour;
    public SideType Side;
    public WalkingSideType WalkingSide;
    public FacingType Facing;

    public bool isAIOn;
    public bool Test = false;
    public BaseCharacter CharOwner;
    public Transform SelectionIndicator;
    public SpriteRenderer SelectionIndicatorSprite;
    public SpriteRenderer SelectionIndicatorPlayerNumberBig;
    public SpriteRenderer SelectionIndicatorPlayerNumberSmall;
    public SpriteRenderer SelectionIndicatorPlayerSmall;
    public Color SelectionIndicatorColorUnselected;
    public Animator IndicatorAnim;
    public GameObject ArrivingParticle;
    public GameObject DeathParticles;
    public AttackType CurrentAttackType;
    //Used to decide the side
    public void SetupCharacterSide()
    {
        MatchType matchType = LoaderManagerScript.Instance != null ? LoaderManagerScript.Instance.MatchInfoType : BattleInfoManagerScript.Instance.MatchInfoType;
        switch (matchType)
        {
            case MatchType.PvE:
                if (PlayerController.Contains(ControllerType.Enemy))
                {
                    SetUnit(FacingType.Left, SideType.RightSide, UnitBehaviourType.NPC, true);
                }
                else
                {
                    SetUnit(FacingType.Right, SideType.LeftSide, UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PvP:
                if (PlayerController.Contains(ControllerType.Player2))
                {
                    SetUnit(FacingType.Left, SideType.RightSide, UnitBehaviourType.ControlledByPlayer);
                }
                else
                {
                    SetUnit(FacingType.Right, SideType.LeftSide, UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PPvE:
                if (PlayerController.Contains(ControllerType.Enemy))
                {
                    SetUnit(FacingType.Left, SideType.RightSide, UnitBehaviourType.NPC, true);
                }
                else
                {
                    SetUnit(FacingType.Right, SideType.LeftSide, UnitBehaviourType.ControlledByPlayer);
                }
                break;
            case MatchType.PPvPP:
                if (PlayerController.Contains(ControllerType.Player3) && PlayerController.Contains(ControllerType.Player4))
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
            CharOwner.StartMoveCo();
        }
        else
        {
            CharOwner.StopMoveCo();
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
