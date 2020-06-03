using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Grid_UIStarRanking : MonoBehaviour
{
    Grid_UIStar[] stars;
    public float basicValue = 0.1f;

    private void Awake()
    {
        stars = GetComponentsInChildren<Grid_UIStar>();
        stars.OrderBy(r => r.transform.position.x);
        SetStarRanking(basicValue);
    }



    public void SetStarRanking(float value)
    {
        float curVal = value;
        float divNum = 1f / stars.Length;
        foreach(Grid_UIStar star in stars)
        {
            star.SetStarValue(curVal > divNum ? 1 : (curVal % divNum) / divNum);
            curVal -= divNum;
        }
    }

    private void OnValidate()
    {
       if(stars != null) SetStarRanking(basicValue);
    }
}
