using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MaskButton : MonoBehaviour
{
    protected Image maskImg;
    public Sprite maskLockImg;
    public MaskTypes maskType = MaskTypes.None;
    public bool showSelectable = false;
    [HideInInspector] public MaskSelectionBox maskSelectionBox = null;
    protected MaskLoadInformation maskInfo;
    protected bool displayed = false;

    protected Color BGColor = Color.magenta;

    private void Awake()
    {
        maskImg = GetComponentsInChildren<Image>()[1];
        DisplayCurrentMask();
        ShowMaskSelectable();
    }

    public void DisplayCurrentMask()
    {
        if (displayed || maskType == MaskTypes.None) return;

        displayed = true;

        maskInfo = SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == maskType).FirstOrDefault();
        maskImg.sprite = maskInfo.collected ? maskInfo.maskImage : maskLockImg;
        maskImg.rectTransform.sizeDelta *= (maskInfo.collected ? 1f : 0.3f);
    }

    public void ShowMaskSelectable()
    {
        if (!maskInfo.collected || !showSelectable) return;
        if (maskInfo.maskHolder == CharacterNameType.None)
        {
            maskImg.color = Color.white;
        }
        else
        {
            maskImg.color = new Color(1f,1f,1f,0.3f);
        }
    }

    public void Attempt_SelectMaskInSelectionBox()
    {
        if (!showSelectable || !maskInfo.collected || maskInfo.maskHolder != CharacterNameType.None) return;

        maskSelectionBox.SelectMask(maskType);
    }

    public void UpdateMaskInfoBox()
    {
        MaskInfoBox.Instance?.UpdateMaskInfo(maskInfo.collected ? maskType : MaskTypes.None);
    }

    private void OnEnable()
    {
        if (BGColor == Color.magenta) BGColor = GetComponent<Image>().color;
    }

    private void OnDisable()
    {
        if (BGColor != Color.magenta) GetComponent<Image>().color = BGColor;
    }
}
