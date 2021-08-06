using System.Collections.Generic;
using UnityEngine;

namespace ObjectModel
{
    public class ModelNode
    {
        public ModelLink parentLink;
        public List<ModelLink> childLinks = new List<ModelLink>();
        public bool rootNode = false;
        public string GUID;
        public string nodeName;
        public List<PropertyData> nodeProperties = new List<PropertyData>();
        public Texture2D nodeSprite;

        public ModelNode(NodeData nodeData)
        {
            rootNode = nodeData.rootNode;
            GUID = nodeData.nodeID;
            nodeName = nodeData.nodeName;
            nodeSprite = nodeData.nodeSprite;
            foreach(string s in nodeData.JSONProperties) {
                nodeProperties.Add(UnityEngine.JsonUtility.FromJson<PropertyData>(s));
            }
        }
    }
}