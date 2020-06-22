using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid_UIStar : MonoBehaviour
{
    public Image[] starFills = new Image[1];


    public void SetStarValue(float percentage)
    {
        foreach (Image starFill in starFills)
        {
            starFill.fillAmount = percentage;
        }
    }
}
