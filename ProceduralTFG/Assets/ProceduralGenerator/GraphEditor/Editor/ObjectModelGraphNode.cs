using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class ObjectModelGraphNode : Node
{
    public string GUID;
    public bool rootNode = false;
    public string nodeName;

    private int nextPortID = -1;

    public string GetNextPortID()
    {
        nextPortID++;
        return nextPortID.ToString();
    }
}
