using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowChartVariablesManagerScript : MonoBehaviour
{
    public static FlowChartVariablesManagerScript instance;

    public List<FlowChartVariablesClass> Variables = new List<FlowChartVariablesClass>();
    [HideInInspector]
    public List<BlockStoryClass> BlocksStory = new List<BlockStoryClass>();
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

public class BlockStoryClass
{
    public string BlockName;
    public bool Used;

    public BlockStoryClass()
    {

    }

    public BlockStoryClass(string blockName)
    {
        BlockName = blockName;
    }
}
