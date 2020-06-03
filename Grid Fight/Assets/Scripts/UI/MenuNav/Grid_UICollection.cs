using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid_UIInputActivators))]
public class Grid_UICollection : MonoBehaviour
{
    public string CollectionID = "";
    [HideInInspector] public Grid_UIInputActivators UIActivators;

    private void Awake()
    {
        UIActivators = GetComponent<Grid_UIInputActivators>();
        DontDestroyOnLoad(this);
    }
}
