using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapArenaScript : MonoBehaviour
{
    public Button ArenaBtn;
    public int Id;

    public void ActiveBtn()
    {
        ArenaBtn.interactable = true;
    }

    public void GoToArena()
    {
        WorldMapManagerScript.Instance.GoToArena(Id);
    }
}
