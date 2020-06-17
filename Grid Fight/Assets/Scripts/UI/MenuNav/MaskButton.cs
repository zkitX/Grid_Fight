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
    protected MaskLoadInformation maskInfo;

    private void Awake()
    {
        maskImg = GetComponentsInChildren<Image>()[1];
        maskInfo = SceneLoadManager.Instance.loadedMasks.Where(r => r.maskType == maskType).FirstOrDefault();
        maskImg.sprite = maskInfo.collected ? maskInfo.maskImage : maskLockImg;
        maskImg.rectTransform.sizeDelta *= (maskInfo.collected ? 1f : 0.3f);
    }

    public void UpdateMaskInfoBox()
    {
        MaskInfoBox.Instance?.UpdateMaskInfo(maskInfo.collected ? maskType : MaskTypes.None);
    }
}
