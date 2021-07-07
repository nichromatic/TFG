using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectModel;

namespace ProceduralGenerator
{
    public class ProceduralObjectNode
    {
        public ProceduralObject parentObject;
        private ProceduralObjectNode parentNode;
        public List<ProceduralObjectNode> childNodes = new List<ProceduralObjectNode>();
        public bool rootNode = false;
        public string nodeName = "";

        public ProceduralObjectNode(ProceduralObjectNode parent, ModelNode data, ProceduralObject PO)
        {
            parentNode = parent;
            parentObject = PO;

            LoadData(data);
        }

        private void LoadData(ModelNode data)
        {
            rootNode = data.rootNode;
            nodeName = data.nodeName;
        }

        public void GenerateChildren(ModelNode modelNode)
        {
            modelNode.childLinks.ForEach(link =>
            {
                var coinToss = UnityEngine.Random.Range(0f, 100f);
                if (coinToss > link.chance) return;

                var childNode = new ProceduralObjectNode(this, link.child, parentObject);
                childNodes.Add(childNode);
                childNode.GenerateChildren(link.child);
            });
        }

        public void DebugPrintNode()
        {
            if (rootNode) Debug.Log("Root node with " + childNodes.Count + " children.");
            else Debug.Log("Node " + nodeName + " with parent " + parentNode.nodeName + " and " + childNodes.Count + " children");
            childNodes.ForEach(node => node.DebugPrintNode());
        }
    }
}
