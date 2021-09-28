using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectModel
{
    [Serializable]
    public class GraphData : ScriptableObject
    {
        public List<NodeData> nodeList = new List<NodeData>();
        public List<LinkData> linkList = new List<LinkData>();
        public List<InputProperty> inputProperties = new List<InputProperty>();
    }
}