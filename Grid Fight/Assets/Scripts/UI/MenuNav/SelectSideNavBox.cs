using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;
using System.Linq;

public class SelectSideNavBox : Grid_UIPlayerNavBox
{
    public int teamSelected = 0;
    public int playerNum = 0;

    public Transform selector;
    protected Animation selectAnim;
    protected TextMeshProUGUI selectorText;



    protected override void Awake()
    {
        selectAnim = selector.GetComponentInChildren<Animation>();
        selectorText = selector.GetComponentInChildren<TextMeshProUGUI>();
        Setup(0);
    }

    public void Setup(int playerIndex)
    {
        playerNum = playerIndex + 1;
        selectorText.text = "P" + (playerIndex + 1).ToString();
        selectorText.color = SceneLoadManager.Instance.playersColor[playerIndex];
        playerNavGroups = new PlayerNavGroup[] { new PlayerNavGroup(false, new int[] { playerIndex }, new Vector2Int(0,0)) };
        selector.position = playerNavGrid.Where(r => r.pos == new Vector2Int(0, 0)).First().button.transform.position;
        SetupInitialSelections();
    }

    public void SetTeam(int team)
    {
        teamSelected = team;
    }

    public void DashSelectorToPos()
    {
        if (SelectorDasher != null) StopCoroutine(SelectorDasher);
        SelectorDasher = DashSelectorToPos_Co(playerNavGrid.Where(r => r.pos == playerNavGroups[0].pos).First().button.transform.position);
        StartCoroutine(SelectorDasher);
    }

    IEnumerator SelectorDasher = null;
    IEnumerator DashSelectorToPos_Co(Vector3 endPos)
    {
        AnimationClip clip = endPos.x >= selector.position.x ? selectAnim.GetClip("SideSelector_DashRight") : selectAnim.GetClip("SideSelector_DashLeft");
        if (selectAnim.isPlaying) selectAnim.Stop();
        selectAnim.clip = clip;
        selectAnim.Play();

        float duration = clip.length;
        float remaining = duration;
        Color endColor = teamSelected == 0 ? SceneLoadManager.Instance.playersColor[playerNum - 1] : Color.white;
        while (remaining != 0)
        {
            remaining = Mathf.Clamp(remaining - Time.deltaTime, 0f, 99f);
            selectorText.color = Color.Lerp(selectorText.color, endColor, 1f - (remaining / duration));
            selector.position = Vector3.Lerp(selector.position, endPos, 1f - (remaining / duration));
            yield return null;
        }
    }
}
