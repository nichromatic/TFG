using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectModelGraphData : ScriptableObject
{
    public List<ObjectModelNodeData> nodeList = new List<ObjectModelNodeData>();
    public List<ObjectModelLinkData> linkList = new List<ObjectModelLinkData>();
}
