using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine.UI;

public class ArenaCharSelectBox : Grid_UIPlayerNavBox
{
    public static ArenaCharSelectBox Instance;

    public GameObject charSelectButtonPrefab = null;
    public int[] rowLengths = new int[] { 14, 13, 14 };
    public float btnSpacingPerc = 0.2f;
    public bool startAtRandomPoints = true;



    protected override void Awake()
    {
        Instance = this;
        return;
    }

    public void Setup()
    {
        SetupGrid();

        playerNavGroups = new PlayerNavGroup[] 
        { 
            new PlayerNavGroup(startAtRandomPoints, SceneLoadManager.Instance.arenaLoadoutInfo.T1Players.ToArray(), _name: "T1"),
            new PlayerNavGroup(startAtRandomPoints, SceneLoadManager.Instance.arenaLoadoutInfo.T2Players.ToArray(), _name: "T2")
        };
        SetupInitialSelections();
    }

    protected void SetupGrid()
    {
        foreach (PlayerNavButton btn in playerNavGrid)
        {
            Destroy(btn.button.gameObject);
        }

        float btnDims = charSelectButtonPrefab.GetComponent<Image>().rectTransform.sizeDelta.x;
        List<PlayerNavButton> pnBtns = new List<PlayerNavButton>();
        int i = 0;
        for (int y = 0; y < rowLengths.Length; y++)
        {
            for (int x = 0; x < rowLengths[y]; x++)
            {
                pnBtns.Add(new PlayerNavButton(new Vector2Int(x, -y), Instantiate(charSelectButtonPrefab, transform).GetComponent<Grid_UIButton>()));
                pnBtns[i].button.transform.localPosition =
                        new Vector3(
                            (btnDims * btnSpacingPerc * x) + (btnDims * x) + (y % 2 == 1 ? btnDims * (1f + btnSpacingPerc) * 0.5f : 0f),
                            -((btnDims * btnSpacingPerc * y) + (btnDims * y)),
                            transform.position.z
                            );
                pnBtns[i].button.GetComponent<ArenaCharSelectButton>().pos = new Vector2Int(x, -y);
                pnBtns[i].button.GetComponent<ArenaCharSelectButton>().DisplayChar(i + 1 > SceneLoadManager.Instance.loadedCharacters.Length ? null : SceneLoadManager.Instance.loadedCharacters[i]);
                pnBtns[i].button.parentPanel = GetComponentInParent<Grid_UIPanel>();
                i++;
            }
        }
        playerNavGrid = pnBtns.ToArray();
    }


    public override void PressForPlayer(int player)
    {
        PlayerNavGroup playerGroup = playerNavGroups.Where(r => r.ContainsPlayer(player)).FirstOrDefault();

        if (playerGroup == null) return;

        PlayerNavButton button = playerNavGrid.Where(r => r.pos == playerGroup.pos).FirstOrDefault();

        if (button == null) return;

        button.button.GetComponent<ArenaCharSelectButton>().SelectChar(playerGroup.Name == "T1" ? 1 : 2);
    }
}
