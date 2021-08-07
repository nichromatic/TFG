using System.Collections.Generic;
using UnityEngine;

namespace ObjectModel
{
    public class ModelNode : ModelBaseNode
    {
        public string nodeName;
        public List<PropertyData> nodeProperties = new List<PropertyData>();
        public Texture2D nodeSprite;

        public ModelNode(NodeData nodeData) : base(nodeData)
        {
            nodeName = nodeData.nodeName;
            nodeSprite = nodeData.nodeSprite;
            foreach(string s in nodeData.JSONProperties) {
                nodeProperties.Add(UnityEngine.JsonUtility.FromJson<PropertyData>(s));
            }
        }
    }
}