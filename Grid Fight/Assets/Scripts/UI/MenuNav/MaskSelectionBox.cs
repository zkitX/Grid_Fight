using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using System.Linq;

public class MaskSelectionBox : MonoBehaviour
{
    protected MaskButton[] maskBtns = null;


    private void Awake()
    {
        maskBtns = GetComponentsInChildren<MaskButton>();
    }

    public void SetupMaskSelect()
    {
        transform.position -= new Vector3(10000, 0, 0);
        foreach(MaskButton btn in maskBtns)
        {
            btn.maskSelectionBox = this;
            btn.ShowMaskSelectable();
        }
    }

    public void SelectMask(MaskTypes maskName)
    {
        MaskLoadInformation maskInfo =  SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == maskName).FirstOrDefault();
        if (maskInfo == null)
        {
            Debug.LogError("No corresponding mask in load info: " + maskName.ToString());
            return;
        }

        maskInfo.maskHolder = CharInfoBox.Instance.curCharID;
        CharInfoBox.Instance.UpdateCurCharInfo();
        maskBtns.Where(r => r.maskType == maskName).First().GetComponent<Grid_UIButton>().DeselectAction(true);

        CloseMaskMenu();
    }

    public void CloseMaskMenu()
    {
        Grid_UINavigator.Instance.TriggerUIActivator("MaskUnitsTransition");
    }
}
