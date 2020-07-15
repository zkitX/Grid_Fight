using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class ArenaSquadBox : MonoBehaviour
{
    [SerializeField] protected Image[] teammateDisplays = new Image[4];
    protected List<Image> teammateBackGrounds = new List<Image>();

    public Image selectedImg = null;
    public Image frame = null;

    protected Color SelectionColor = Color.magenta;
    protected Color backgroundGenericColor = Color.white;

    public Dictionary<int, CharacterLoadInformation> squad = null;

    public void Setup(int squadNum)
    {
        squad = SceneLoadManager.Instance.GetSquadToCheck(squadNum);

        foreach (Image display in teammateDisplays)
        {
            teammateBackGrounds.Add(display.gameObject.GetComponentsInParent<Image>()[1]);
        }

        SelectionColor = SceneLoadManager.Instance.teamsColor[squadNum - 1];
        frame.color = SelectionColor;

        backgroundGenericColor = teammateBackGrounds[0].color;

        DisplaySquad();

        SceneLoadManager.Instance.SquadChangeEvent += DisplaySquad;
    }

    private void DisplaySquad()
    {
        for (int i = 0; i < squad.Count; i++)
        {
            if(squad[i].characterID != CharacterNameType.None)
            {
                teammateDisplays[i].sprite = squad[i].charPortrait;
                teammateDisplays[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                teammateDisplays[i].color = new Color(1f, 1f, 1f, 0f);
            }
            if (i >= 0)
            {
                teammateBackGrounds[i].color = backgroundGenericColor;
            }
        }

        DisplaySelected(selectedImg.sprite);
    }

    public void DisplaySelected(Sprite charPortrait)
    {
        Color displayColor = new Color(1f, 1f, 1f, 1f);
        if (charPortrait == null)
        {
            displayColor = new Color(1f, 1f, 1f, 0f);
        }

        selectedImg.sprite = charPortrait;
        selectedImg.color = displayColor;

        if (squad.Values.Where(r => r.characterID == CharacterNameType.None).FirstOrDefault() == null) return;

        int key = squad.Where(r => r.Value.characterID == CharacterNameType.None).First().Key;
        teammateBackGrounds[key].color = SelectionColor;
    }
}
