using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class SquadBox : MonoBehaviour
{
    public static SquadBox Instance;

    [SerializeField] protected Image[] squadMateDisplays = new Image[3];
    [SerializeField] protected TextMeshProUGUI[] bonusDisplays = new TextMeshProUGUI[3];
    protected List<Image> squadMateBackGrounds = new List<Image>();

    protected Sprite selectedImg = null;

    public Color SelectionColor = Color.magenta;
    protected Color backgroundGenericColor = Color.white;

    private void Awake()
    {
        Instance = this;

        backgroundGenericColor = GetComponent<Image>().color;

        foreach (Image display in squadMateDisplays)
        {
            squadMateBackGrounds.Add(display.gameObject.GetComponentsInParent<Image>()[1]);
        }

        DisplaySquad();

        SceneLoadManager.Instance.SquadChangeEvent += DisplaySquad;
    }

    private void DisplaySquad()
    {
        for (int i = 0; i < SceneLoadManager.Instance.squad.Count; i++)
        {
            if(SceneLoadManager.Instance.squad[i].characterID != CharacterNameType.None)
            {
                squadMateDisplays[i].sprite = SceneLoadManager.Instance.squad[i].charPortrait;
                squadMateDisplays[i].color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                squadMateDisplays[i].color = new Color(1f, 1f, 1f, 0f);
            }
            if (i > 0)
            {
                squadMateBackGrounds[i].color = backgroundGenericColor;
                bonusDisplays[i - 1].text = SceneLoadManager.Instance.squad[i].squadBonusDetails;
            }
        }

        DisplaySelected(selectedImg);
    }

    public void DisplaySelected(Sprite charPortrait)
    {
        if (SceneLoadManager.Instance.squad.Values.Where(r => r.characterID == CharacterNameType.None).FirstOrDefault() == null) return;

        Color displayColor = new Color(1f, 1f, 1f, 0.6f);
        if (charPortrait == null)
        {
            displayColor = new Color(1f, 1f, 1f, 0f);
        }

        selectedImg = charPortrait;

        int key = SceneLoadManager.Instance.squad.Where(r => r.Value.characterID == CharacterNameType.None).First().Key;
        squadMateDisplays[key].sprite = selectedImg;
        squadMateDisplays[key].color = displayColor;
        squadMateBackGrounds[key].color = SelectionColor;
    }
}
