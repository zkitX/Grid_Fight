using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowChartVariablesManagerScript : MonoBehaviour
{
    public static FlowChartVariablesManagerScript instance;

    public List<FlowChartVariablesClass> Variables = new List<FlowChartVariablesClass>();


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

   
}

[System.Serializable]
public class FlowChartVariablesClass
{
    public string name;
    public string Value;
}