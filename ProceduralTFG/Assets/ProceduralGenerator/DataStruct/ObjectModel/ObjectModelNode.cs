using System.Collections.Generic;

public class ObjectModelNode
{
    public ObjectModelLink parentLink;
    public List<ObjectModelLink> childLinks = new List<ObjectModelLink>();
    public bool rootNode = false;
    public string GUID;
    public string nodeName;

    public ObjectModelNode(ObjectModelNodeData nodeData)
    {
        rootNode = nodeData.rootNode;
        GUID = nodeData.nodeID;
        nodeName = nodeData.nodeName;
    }
}
