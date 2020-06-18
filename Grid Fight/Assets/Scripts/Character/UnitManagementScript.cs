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

    public Vector2Int CurrentTilePos
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

    public bool Test = false;
    public BaseCharacter CharOwner;
    public Transform SelectionIndicator;
    public SpriteRenderer SelectionIndicatorSprite;
    public SpriteRenderer SelectionIndicatorPlayerNumberBig;
    public SpriteRenderer SelectionIndicatorPlayerNumberSmall;
    public SpriteRenderer SelectionIndicatorPlayerSmall;
    public SpriteRenderer SelectionIndicatorPlayerBig;//Potentially can be removed
    public Color SelectionIndicatorColorUnselected;
    public Animator IndicatorAnim;
    public GameObject ArrivingParticle;
    public GameObject DeathParticles;

    public Transform HPBar;
    public Transform StaminaBar;

    public Transform IndicatorContainer;
    public Transform HpBarContainer;
    public Transform StaminaBarContainer;
    //Used to decide the side
   

    public void SetBattleUISelection(ControllerType playerController)
    {
        SelectionIndicatorPlayerNumberSmall.color = BattleManagerScript.Instance.playersColor[(int)playerController];
        SelectionIndicatorPlayerSmall.color = BattleManagerScript.Instance.playersColor[(int)playerController];
        SelectionIndicatorPlayerNumberBig.color = BattleManagerScript.Instance.playersColor[(int)playerController];
        SelectionIndicatorPlayerBig.color = BattleManagerScript.Instance.playersColor[(int)playerController];

        SelectionIndicatorPlayerNumberSmall.sprite = BattleManagerScript.Instance.playersNumberSmall[(int)playerController];
        SelectionIndicatorPlayerNumberBig.sprite = BattleManagerScript.Instance.playersNumberBig[(int)playerController];
    }


    //Used to set the unit info for the facing,side,unitbehaviour, tag and AI
    public void SetUnit(UnitBehaviourType ubt)
    {
        if(Facing == FacingType.Right)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
        gameObject.tag = Side.ToString();
        if (CharOwner.SpineAnim == null)
        {
            CharOwner.SpineAnimatorsetup();
        }
        CharOwner.SpineAnim.gameObject.tag = Side.ToString();
        UnitBehaviour = ubt;
    }


    private void Update()
    {
        if(Test)
        {
            Test = false;
            SetUnit(UnitBehaviour);
        }
    }

    public void EnableBattleBars(bool state)
    {
        foreach(SpriteRenderer sprenderer in HPBar.parent.GetComponentsInChildren<SpriteRenderer>())
        {
            sprenderer.enabled = state;
        }
        foreach (SpriteRenderer sprenderer in StaminaBar.parent.GetComponentsInChildren<SpriteRenderer>())
        {
            sprenderer.enabled = state;
        }
    }


    public bool IsCharControllableByPlayers(List<ControllerType> controllers)
    {
        foreach (ControllerType item in controllers)
        {
            if(!PlayerController.Contains(item))
            {
                return false;
            }
        }

        return true;
    }
}
