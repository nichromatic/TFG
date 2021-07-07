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

        public List<PropertyData> ExportPropertyData()
        {
            List<PropertyData> data = new List<PropertyData>();
            nodePropertyRows.ForEach(p => data.Add(p.ExportPropertyData()));
            return data;
        }
    }
}
