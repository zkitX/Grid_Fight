using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStartupScript : MonoBehaviour
{
    public List<GameObject> ObjectsToEnable;
    private void Awake()
    {
        foreach(GameObject g in ObjectsToEnable)
        {
            g.SetActive(true);
        }
    }
}
