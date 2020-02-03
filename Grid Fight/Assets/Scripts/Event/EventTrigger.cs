using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger : ScriptableObject
{
    public string Name;
    [HideInInspector] public bool hasHappened = false;

}
