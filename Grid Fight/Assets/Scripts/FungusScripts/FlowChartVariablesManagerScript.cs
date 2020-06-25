using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowChartVariablesManagerScript : MonoBehaviour
{
    public static FlowChartVariablesManagerScript instance;

    public List<FlowChartVariablesClass> Variables = new List<FlowChartVariablesClass>();
    void Awake()
    {
        instance = this;
    }
}

[System.Serializable]
public class FlowChartVariablesClass
{
    public string Name;
    public string Value;

    public FlowChartVariablesClass()
    {

    }

    public FlowChartVariablesClass(string name, string value)
    {
        Name = name;
        Value = value;
    }    
}