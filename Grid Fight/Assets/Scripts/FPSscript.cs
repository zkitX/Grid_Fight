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


    float deltaTime = 0.0f;
    // Update is called once per frame
    void Update()
    {
        FPStxt.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        //  Modetxt.text = Operation.mode.ToString();
        Resolutiontxt.text = "Width: " + Display.main.renderingWidth + "  Height: " + Display.main.renderingHeight;
      //  Performancestxt.text = Performance.mode.ToString();
    }

    

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
