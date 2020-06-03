using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_UIStar : MonoBehaviour
{
    public Image starFill = null;


    public void SetStarValue(float percentage)
    {
        starFill.fillAmount = percentage;
    }
}
