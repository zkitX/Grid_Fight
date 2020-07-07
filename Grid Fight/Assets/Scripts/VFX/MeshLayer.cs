using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshLayer : MonoBehaviour
{
    public int Layer = 305;

    private void OnEnable()
    {
        gameObject.GetComponent<MeshRenderer>().sortingOrder = Layer;
    }
}
