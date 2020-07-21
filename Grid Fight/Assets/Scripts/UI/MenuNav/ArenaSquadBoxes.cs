using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class ArenaSquadBoxes : MonoBehaviour
{
    public static ArenaSquadBoxes Instance;

    protected ArenaSquadBox[] boxes;

    private void Awake()
    {
        Instance = this;
        boxes = GetComponentsInChildren<ArenaSquadBox>();
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].Setup(i + 1);
        }
    }

    public void DisplaySelection(int teamNum, SkeletonDataAsset charDisplay, bool hide = false)
    {
        boxes[teamNum - 1].DisplaySelected(charDisplay, hide);
    }
}
