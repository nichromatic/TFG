using System.Collections.Generic;

namespace ObjectModel
{
    public class ModelNode
    {
        public ModelLink parentLink;
        public List<ModelLink> childLinks = new List<ModelLink>();
        public bool rootNode = false;
        public string GUID;
        public string nodeName;
        //public List<Property> nodeProperties = new List<Property>();

        public ModelNode(NodeData nodeData)
        {
            rootNode = nodeData.rootNode;
            GUID = nodeData.nodeID;
            nodeName = nodeData.nodeName;
        }
    }
}