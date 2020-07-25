using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshLayer : MonoBehaviour
{
    public int Layer = 305;
    public bool AllChildren = false;
    private void OnEnable()
    {
        if (AllChildren)
        {
            foreach(MeshRenderer m in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                m.GetComponent<MeshRenderer>().sortingOrder = Layer;
            }
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().sortingOrder = Layer;
        }
    }
}
