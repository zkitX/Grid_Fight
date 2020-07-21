using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class ArenaMapSelector : MonoBehaviour
{
    public GameObject mapNodePrefab;
    public float bufferBetweenNodes = 0.1f;
    public float durationOfMoveBetweenNodes = 0.4f;

    List<ArenaMapNode> listedStages = new List<ArenaMapNode>();
    int selectionIndex = 0;

    protected Vector3 startingPos = Vector3.zero;
    public Grid_UIPanel parentPanel;

    private void Awake()
    {
        startingPos = transform.position;
    }

    public void PopulateMaps()
    {
        parentPanel = GetComponentInParent<Grid_UIPanel>();

        foreach (ArenaMapNode stage in listedStages.ToArray())
        {
            Destroy(stage);
        }
        listedStages.Clear();

        Vector2 dimentions = GetComponentInChildren<Image>().rectTransform.sizeDelta;
        GetComponentInChildren<Image>().color = Color.clear;

        StageLoadInformation[] pvpStages = SceneLoadManager.Instance.loadedStages.Where(r => r.stageProfile.type == StageType.Pvp).ToArray();

        for(int i = 0; i < pvpStages.Length; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + (i * (dimentions.x * (1f + bufferBetweenNodes))), transform.position.y, transform.position.z);
            ArenaMapNode curNode = Instantiate(mapNodePrefab, pos, Quaternion.identity, transform).GetComponent<ArenaMapNode>();
            curNode.SetupStageInfo(pvpStages[i], dimentions);
            listedStages.Add(curNode);
        }

        SelectFirstMap();

        InputController.Instance.LeftJoystickUsedEvent -= ChangeMapSelection;
        InputController.Instance.LeftJoystickUsedEvent += ChangeMapSelection;
    }

    protected void SelectFirstMap()
    {
        if (listedStages.Count == 0) return;

        listedStages[0].SelectAction();
        selectionIndex = 0;
    }

    protected void ChangeSelection(int selectionChange)
    {
        if (selectionIndex + selectionChange < 0 || selectionIndex + selectionChange >= listedStages.Count || selectorMoving) return;

        listedStages[selectionIndex].DeselectAction();
        selectionIndex += selectionChange;
        listedStages[selectionIndex].SelectAction();

        SceneLoadManager.Instance.arenaLoadoutInfo.stageSelected = listedStages[selectionIndex].stageProfile;

        if (SelectionMover != null) StopCoroutine(SelectionMover);
        SelectionMover = MoveSelectionToCentre();
        StartCoroutine(SelectionMover);
    }

    public void ChangeMapSelection(int playerIndex, InputDirection dir)
    {
        if (parentPanel.focusState != UI_FocusTypes.Focused) return;

        switch (dir)
        {
            case InputDirection.Left:
                ChangeSelection(-1);
                break;
            case InputDirection.Right:
                ChangeSelection(1);
                break;
            default:
                return;
        }
    }

    bool selectorMoving = false;
    IEnumerator SelectionMover = null;
    IEnumerator MoveSelectionToCentre()
    {
        selectorMoving = true;
        Vector3 endPos = transform.position + new Vector3(startingPos.x - listedStages[selectionIndex].transform.position.x, 0f, 0f);
        float timeRemaining = durationOfMoveBetweenNodes;
        while(timeRemaining != 0f)
        {
            timeRemaining = Mathf.Clamp(timeRemaining - Time.deltaTime, 0f, 99f);
            transform.position = Vector3.Lerp(transform.position, endPos, 1f - (timeRemaining / durationOfMoveBetweenNodes));
            yield return null;
        }
        transform.position = endPos;
        selectorMoving = false;
    }

}
