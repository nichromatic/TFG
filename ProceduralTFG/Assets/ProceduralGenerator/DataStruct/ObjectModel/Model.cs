using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectModel
{
    public class Model
    {
        public ModelNode rootNode;
        public List<ModelNode> allNodes;
        public List<ModelLink> allLinks;

        public void BuildModelFromGraph(GraphData graphData)
        {
            allNodes = new List<ModelNode>();
            allLinks = new List<ModelLink>();

            var findRootNode = graphData.nodeList.Where(node => node.rootNode).ToList(); // Find the rootNode
            if (findRootNode.Count > 1)
            {
                Debug.LogError("Couldn't load the model: The graph has more than one rootNode!");
                return;
            }
            else if (findRootNode.Count == 0)
            {
                Debug.LogError("Couldn't load the model: The graph does not have a rootNode!");
                return;
            }

            rootNode = new ModelNode(findRootNode[0]);

            var currentNode = rootNode;
            var openNodes = new List<ModelNode>();
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
                }
                else
                {
                    currentNode = openNodes[0];
                    openNodes.RemoveAt(0);
                }
            }
        }

        public List<ModelNode> GenerateChildren(ModelNode node, GraphData graphData)
        {
            var children = new List<ModelNode>();

            // Find all links that have this node as the parent (output)
            var childLinks = graphData.linkList.Where(link => link.parentNodeID == node.GUID).ToList();
            if (childLinks.Count > 0)
            {
                childLinks.ForEach(childLink =>
                {
                    var findChildNode = graphData.nodeList.Where(child => child.nodeID == childLink.childNodeID).ToList();
                    var childNode = new ModelNode(findChildNode[0]);
                    var newLink = new ModelLink(node, childNode, childLink.chance);
                    node.childLinks.Add(newLink);
                    childNode.parentLink = newLink;
                    children.Add(childNode);
                    allLinks.Add(newLink);
                });
            }

            return children;
        }
    }
}
