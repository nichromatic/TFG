using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ObjectModel
{
    public class GraphNode : Node
    {
        public string GUID;
        public bool rootNode = false;
        public string nodeName;
        //public List<Property> nodeProperties = new List<Property>();
        public List<GraphProperty> nodePropertyRows = new List<GraphProperty>();
        public VisualElement nodePropertiesContainer;

        private int nextPortID = -1;

        public string GetNextPortID()
        {
            nextPortID++;
            return nextPortID.ToString();
        }

        public List<string> ExportPropertyData()
        {
            List<string> data = new List<string>();
            nodePropertyRows.ForEach(p => data.Add(p.ExportPropertyData()));
            return data;
        }
    }
}
