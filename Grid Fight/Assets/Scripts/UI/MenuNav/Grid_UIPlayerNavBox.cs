using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine.UI;

public class Grid_UIPlayerNavBox : MonoBehaviour
{
    public bool activeState = false;
    public void SetActiveState(bool state)
    {
        activeState = state;
    }

    public PlayerNavButton[] playerNavGrid = new PlayerNavButton[0]; 
    public PlayerNavGroup[] playerNavGroups = new PlayerNavGroup[0];

    #region Input events subscription handling
    protected virtual void OnEnable()
    {
        InputController.Instance.ButtonAUpEvent += Instance_ButtonAUpEvent;
        InputController.Instance.LeftJoystickUsedEvent += Instance_LeftJoyStickUsedEvent;
    }

    protected virtual void OnDisable()
    {
        InputController.Instance.ButtonAUpEvent -= Instance_ButtonAUpEvent;
        InputController.Instance.LeftJoystickUsedEvent -= Instance_LeftJoyStickUsedEvent;
    }

    protected void Instance_ButtonAUpEvent(int player)
    {
        if (!Grid_UINavigator.Instance.CanNavigate(MenuNavigationType.PlayerNavBox) || !activeState) return;
        PressForPlayer(player);
    }
    protected float offset = 0f;
    protected void Instance_LeftJoyStickUsedEvent(int player, InputDirection dir)
    {
        if (!Grid_UINavigator.Instance.CanNavigate(MenuNavigationType.PlayerNavBox) || !activeState) return;
        if (offset + 0.2f < Time.time)
        {
            MoveForPlayer(player, dir);
            offset = Time.time;
        }
    }
    #endregion

    protected virtual void Awake()
    {
        SetupInitialSelections();
    }

    public void SetupInitialSelections()
    {
        //update the selection based on the settings in the navGroup
        foreach (PlayerNavGroup navGroup in playerNavGroups)
        {
            Grid_UIButton btnToSelect = null;
            if (!navGroup.startInRandomPos && playerNavGrid.Where(r => r.pos == navGroup.pos).FirstOrDefault() != null)
            {
                btnToSelect = playerNavGrid.Where(r => r.pos == navGroup.pos).First().button;
            }
            else
            {
                btnToSelect = playerNavGrid[Random.Range(0, playerNavGrid.Length)].button;
            }
            btnToSelect.SelectAction();
        }
    }

    protected virtual void PressForPlayer(int player)
    {
        Debug.LogError("LOGGED PRESS FOR PLAYER " + (player + 1).ToString());
    }

    protected virtual void MoveForPlayer(int player, InputDirection dir)
    {
        PlayerNavGroup playNav = playerNavGroups.Where(r => r.ContainsPlayer(player)).FirstOrDefault();

        if (playNav == null) return;

        Vector2Int destPos = GetClosestPosInDirection(playNav.pos, dir);

        if (destPos == playNav.pos) return;

        if(playerNavGrid.Where(r => r.pos == playNav.pos).FirstOrDefault() != null)
        {
            playerNavGrid.Where(r => r.pos == playNav.pos).First().button.DeselectAction(true);
        }

        playerNavGrid.Where(r => r.pos == destPos).First().button.SelectAction();

        playNav.pos = destPos;
    }

    protected Vector2Int GetClosestPosInDirection(Vector2Int start, InputDirection dir)
    {
        PlayerNavButton closestButton = null;

        switch (dir)
        {
            case InputDirection.Up:
                closestButton = playerNavGrid.Where(r => r.pos == start + new Vector2Int(0,1)).FirstOrDefault();
                if (closestButton != null) break;
                closestButton = playerNavGrid.Where(r => r.pos.y > start.y).OrderBy(e => (start + e.pos).magnitude).FirstOrDefault();
                break;
            case InputDirection.Down:
                closestButton = playerNavGrid.Where(r => r.pos == start + new Vector2Int(0, -1)).FirstOrDefault();
                if (closestButton != null) break;
                closestButton = playerNavGrid.Where(r => r.pos.y < start.y).OrderBy(e => (start + e.pos).magnitude).FirstOrDefault();
                break;
            case InputDirection.Left:
                closestButton = playerNavGrid.Where(r => r.pos == start + new Vector2Int(-1, 0)).FirstOrDefault();
                if (closestButton != null) break;
                closestButton = playerNavGrid.Where(r => r.pos.x < start.x).OrderBy(e => (start + e.pos).magnitude).FirstOrDefault();
                break;
            case InputDirection.Right:
                closestButton = playerNavGrid.Where(r => r.pos == start + new Vector2Int(1, 0)).FirstOrDefault();
                if (closestButton != null) break;
                closestButton = playerNavGrid.Where(r => r.pos.x > start.x).OrderBy(e => (start + e.pos).magnitude).FirstOrDefault();
                break;
        }

        if(closestButton == null)
        {
            return start;
        }

        return closestButton.pos;
    }

    protected void OnValidate()
    {
        foreach(PlayerNavGroup png in playerNavGroups)
        {
            png.Name = (png.players.Length > 1 ? png.players.Length.ToString() + " Players" : (png.players.Length == 1 ? ("Player " + (png.players[0] + 1).ToString()) : "No players"))
                + " starting on " + (png.startInRandomPos ? "a random tile" : "tile pos " + png.pos.ToString());
        }

        foreach(PlayerNavButton pnb in playerNavGrid)
        {
            pnb.Name = (pnb.button != null ? "Button: " + pnb.button.name : "Unassigned Button") + " in pos " + pnb.pos.ToString();
        }
    }
}

[System.Serializable]
public class PlayerNavGroup
{
    [HideInInspector] public string Name;
    public bool startInRandomPos = false;
    [ConditionalField("startInRandomPos", true)] public Vector2Int pos;
    public int[] players = new int[0];
    public bool ContainsPlayer(int playerIndex)
    {
        return players.Contains(playerIndex);
    }

    public PlayerNavGroup(bool _startRandomPos, int[] _players, Vector2Int startingPos = new Vector2Int())
    {
        startInRandomPos = _startRandomPos;
        players = _players;
        pos = startingPos;
    }
}

[System.Serializable]
public class PlayerNavButton
{
    [HideInInspector] public string Name;
    public Vector2Int pos;
    public Grid_UIButton button;
}