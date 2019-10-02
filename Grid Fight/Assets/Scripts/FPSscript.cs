using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSscript : MonoBehaviour
{
    public Text txt;
   

    // Update is called once per frame
    void Update()
    {
        txt.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
    }
}
