using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid_UIActivators))]
public class Grid_UICollection : MonoBehaviour
{
    public string CollectionID = "";
    [HideInInspector] public Grid_UIActivators UIActivators;

    private void Awake()
    {
        UIActivators = GetComponent<Grid_UIActivators>();
        DontDestroyOnLoad(this);
    }
}
