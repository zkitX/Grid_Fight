using Fungus;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CommandInfo("Scripting",
                "Call CallCameraUpdate",
                "CallCameraUpdate")]
[AddComponentMenu("")]
public class CallCameraUpdate : Command
{


    public CameraMovementType CamMovementType;
    public float Zoom;
    public AnimationCurve ZoomCurve;
    public float Duration;
    public AnimationCurve MovementCurve;

    [HideInInspector] public Vector3 NextCamPos;

    [HideInInspector] public CharacterNameType CharacterId;

    [HideInInspector] public ControllerType PlayerController;

    [HideInInspector] public Vector2Int TilePos;

    public bool WaitForCameraMovement = false;
    #region Public members

    public override void OnEnter()
    {

        StartCoroutine(MoveCam());

    }

    private IEnumerator MoveCam()
    {
        Vector3 nextPos = Vector3.zero;
        switch (CamMovementType)
        {
            case CameraMovementType.OnWorldPosition:
                nextPos = NextCamPos;
                break;
            case CameraMovementType.OnCharacter:
                nextPos = GridManagerScript.Instance.GetBattleTile(BattleManagerScript.Instance.AllCharactersOnField.Where(r => r.CharInfo.CharacterID == CharacterId).First().UMS.CurrentTilePos).transform.position;    
                break;
            case CameraMovementType.OnPlayer:
                nextPos = GridManagerScript.Instance.GetBattleTile(BattleManagerScript.Instance.CurrentSelectedCharacters[PlayerController].Character.UMS.CurrentTilePos).transform.position;
                break;
            case CameraMovementType.OnTile:
                nextPos = GridManagerScript.Instance.GetBattleTile(TilePos).transform.position;
                break;
        }
        BattleState currentState = BattleManagerScript.Instance.CurrentBattleState;

        CameraManagerScript.Instance.CameraFocusSequence(Duration, Zoom, ZoomCurve, MovementCurve, nextPos);

        if (WaitForCameraMovement)
        {
            yield return BattleManagerScript.Instance.WaitFor(Duration, () => BattleManagerScript.Instance.CurrentBattleState != currentState);
        }
        Continue();
    }

    
    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}