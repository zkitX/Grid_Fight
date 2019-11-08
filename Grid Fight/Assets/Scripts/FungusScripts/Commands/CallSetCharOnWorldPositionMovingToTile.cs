﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

/// <summary>
/// Calls a named method on a GameObject using the GameObject.SendMessage() system.
/// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
/// a Send Message command for sending messages to trigger block execution.
/// </summary>
[CommandInfo("Scripting",
             "Call SetCharOnWorldPositionMovingToTile",
             "Calls SetCharOnWorldPositionMovingToTile")]
[AddComponentMenu("")]
public class CallSetCharOnWorldPositionMovingToTile : Command
{

    public ControllerType PlayerController;
    public CharacterType Ct;
    public Transform WorldPos;
    public Vector2Int TilePos;
    public float Duration;

    protected virtual void CallTheMethod()
    {
        BattleManagerScript.Instance.SetCharOnWorldPositionMovingToTile(PlayerController, Ct, WorldPos.position, TilePos, Duration);
    }

    #region Public members

    public override void OnEnter()
    {
        CallTheMethod();
        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(235, 191, 217, 255);
    }

    #endregion
}


