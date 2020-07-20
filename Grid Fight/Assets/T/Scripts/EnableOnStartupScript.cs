using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnStartupScript : MonoBehaviour
{
    public List<GameObject> ObjectsToEnable;
    public List<GameObject> ObjectsToDisable;
    private void Awake()
    {
        foreach (GameObject g in ObjectsToEnable)
        {
            g.SetActive(true);
        }
        foreach (GameObject g in ObjectsToDisable)
        {
            g.SetActive(false);
        }
    }
}
