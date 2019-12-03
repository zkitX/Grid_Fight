using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_SWITCH
using UnityEngine.Switch;
#endif
public class FPSscript : MonoBehaviour
{
    public Text FPStxt;
    public Text Modetxt;
    public Text Resolutiontxt;
    public Text Performancestxt;

    // Update is called once per frame
    void Update()
    {
        FPStxt.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
        Modetxt.text = Operation.mode.ToString();
        Resolutiontxt.text = "Width: " + Display.main.renderingWidth + "  Height: " + Display.main.renderingHeight;
        Performancestxt.text = Performance.mode.ToString();
    }
}
