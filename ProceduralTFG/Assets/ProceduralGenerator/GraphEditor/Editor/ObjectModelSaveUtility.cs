using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectModelSaveUtility
{
    private ObjectModelGraphView _graphView;
    private ObjectModelGraphData savedGraph;

    private List<Edge> edges => _graphView.edges.ToList();
    private List<ObjectModelGraphNode> nodes => _graphView.nodes.ToList().Cast<ObjectModelGraphNode>().ToList();


    public static ObjectModelSaveUtility GetInstance(ObjectModelGraphView graphView)
    {
        return new ObjectModelSaveUtility
        {
            _graphView = graphView
        };
    }

    public void Save(string filename)
    {
        if (!edges.Any()) return; // Si no hay ningún link, no se guarda nada

        var graphData = ScriptableObject.CreateInstance<ObjectModelGraphData>();

        var connectedPorts = edges.Where(x => x.input.node != null).ToArray();
        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as ObjectModelGraphNode;
            var inputNode = connectedPorts[i].input.node as ObjectModelGraphNode;

            var chance = connectedPorts[i].output.contentContainer.Q<FloatField>("chance").value;
            graphData.linkList.Add(new ObjectModelLinkData {
                parentNodeID = outputNode.GUID,
                portName = connectedPorts[i].output.portName,
                childNodeID = inputNode.GUID,
                chance = chance
            });
        }
        foreach (var graphNode in nodes)
        {
            graphData.nodeList.Add(new ObjectModelNodeData {
                nodeID = graphNode.GUID,
                nodePos = graphNode.GetPosition().position, // GetPosition() returns the entire Rect(pos,size), we only need the position
                nodeName = graphNode.nodeName,
                rootNode = graphNode.rootNode
            });
        }

        // We create the Resources folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        // It needs to be called "Resources" so we can access it when loading
        AssetDatabase.CreateAsset(graphData, $"Assets/Resources/{filename}.asset");
        AssetDatabase.SaveAssets();
    }

    public void Load(string filename)
    {
        // Load asset from Resources folder
        savedGraph = Resources.Load<ObjectModelGraphData>(filename);

        // If we didn't find the asset
        if (savedGraph == null)
        {
            EditorUtility.DisplayDialog("File not found", "No graph asset file with that name was found. Are you sure it's inside a 'Resources' folder?", "OK");
            return;
        } else
        {
            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }
    }

    public void ClearGraph()
    {
        //nodes.Find(x => x.RootNode).GUID = savedGraph.linkList[0].parentNodeID;

        foreach (var node in nodes)
        {
            //if (node.RootNode) continue;
            edges.Where(x => x.input.node == node).ToList().ForEach(edge => _graphView.RemoveElement(edge));

            _graphView.RemoveElement(node);
;       }
    }

    private void CreateNodes()
    {
        foreach (var nodeData in savedGraph.nodeList)
        {
            if (nodeData.rootNode)
            {
                var tempNode = _graphView.GenerateRootNode(nodeData.nodeName, false); // Don't generate default port, we add them from the saved graph
                tempNode.GUID = nodeData.nodeID;
                tempNode.SetPosition(new Rect(nodeData.nodePos, _graphView.defaultNodeSize));

                _graphView.AddElement(tempNode);

                var nodePorts = savedGraph.linkList.Where(x => x.parentNodeID == nodeData.nodeID).ToList();
                for (int i = 0; i < nodePorts.Count; i++)
                {
                    _graphView.GenerateChildPort(tempNode, nodePorts[i].chance);
                }

            } else
            {
                var tempNode = _graphView.CreateNode(nodeData.nodeName, _graphView.defaultNodePos);
                tempNode.GUID = nodeData.nodeID;
                tempNode.SetPosition(new Rect(nodeData.nodePos, _graphView.defaultNodeSize));

                _graphView.AddElement(tempNode);

                var nodePorts = savedGraph.linkList.Where(x => x.parentNodeID == nodeData.nodeID).ToList();
                for (int i = 0; i < nodePorts.Count; i++)
                {
                    _graphView.GenerateChildPort(tempNode, nodePorts[i].chance);
                }
            }
        }
    }

    private void ConnectNodes()
    {
        for (var i = 0; i < nodes.Count; i++)
        {
            var connections = savedGraph.linkList.Where(x => x.parentNodeID == nodes[i].GUID).ToList();
            for (var j = 0; j < connections.Count; j++)
            {
                var childNodeID = connections[j].childNodeID;
                var childNode = nodes.First(x => x.GUID == childNodeID);
                //Debug.Log("Connecting port " + nodes[i].GUID + " to " + childNodeID);
                LinkNodes(nodes[i].outputContainer[j].Q<Port>(), (Port)childNode.inputContainer[0]);
            }
        }
    }

    private void LinkNodes(Port portA, Port portB)
    {
        var tempEdge = new Edge() { output = portA, input = portB };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _graphView.Add(tempEdge);

    }
}
