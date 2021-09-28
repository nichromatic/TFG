using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectModel
{
    public class ModelBaseNode
    {
        public ModelLink parentLink;
        public List<ModelLink> childLinks = new List<ModelLink>();
        public bool rootNode = false;
        public string GUID;

        public ModelBaseNode(NodeData nodeData)
        {
            rootNode = nodeData.rootNode;
            GUID = nodeData.nodeID;
        }
    }
}