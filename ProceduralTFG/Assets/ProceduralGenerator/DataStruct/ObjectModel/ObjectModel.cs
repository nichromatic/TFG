using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectModel
{
    public ObjectModelNode rootNode;
    public List<ObjectModelNode> allNodes;
    public List<ObjectModelLink> allLinks;

    public void BuildModelFromGraph (ObjectModelGraphData graphData)
    {
        allNodes = new List<ObjectModelNode>();
        allLinks = new List<ObjectModelLink>();

        var findRootNode = graphData.nodeList.Where(node => node.rootNode).ToList(); // Find the rootNode
        if (findRootNode.Count > 1)
        {
            Debug.LogError("Couldn't load the model: The graph has more than one rootNode!");
            return;
        } else if (findRootNode.Count == 0)
        {
            Debug.LogError("Couldn't load the model: The graph does not have a rootNode!");
            return;
        }

        rootNode = new ObjectModelNode(findRootNode[0]);

        var currentNode = rootNode;
        var openNodes = new List<ObjectModelNode>();
        var finished = false;
        while (!finished)
        {
            var generatedChildren = GenerateChildren(currentNode, graphData);
            generatedChildren.ForEach(child =>
            {
                openNodes.Add(child);
                allNodes.Add(child);
            });

            if (openNodes.Count == 0)
            {
                finished = true;
                Debug.Log("Model loaded successfully.");
            } else
            {
                currentNode = openNodes[0];
                openNodes.RemoveAt(0);
            }
        }
    }

    public List<ObjectModelNode> GenerateChildren(ObjectModelNode node, ObjectModelGraphData graphData)
    {
        var children = new List<ObjectModelNode>();

        // Find all links that have this node as the parent (output)
        var childLinks = graphData.linkList.Where(link => link.parentNodeID == node.GUID).ToList();
        if (childLinks.Count > 0)
        {
            childLinks.ForEach(childLink =>
            {
                var findChildNode = graphData.nodeList.Where(child => child.nodeID == childLink.childNodeID).ToList();
                var childNode = new ObjectModelNode(findChildNode[0]);
                var newLink = new ObjectModelLink(node, childNode);
                node.childLinks.Add(newLink);
                childNode.parentLink = newLink;
                children.Add(childNode);
                allLinks.Add(newLink);
            });
        }

        return children;
    }
}
