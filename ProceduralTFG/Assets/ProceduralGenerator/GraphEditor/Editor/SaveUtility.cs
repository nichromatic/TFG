using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ObjectModel
{
    public class SaveUtility
    {
        private Graph _graphView;
        private GraphData savedGraph;

        private List<Edge> edges => _graphView.edges.ToList();
        private List<BaseNode> nodes => _graphView.nodes.ToList().Cast<BaseNode>().ToList();


        public static SaveUtility GetInstance(Graph graphView)
        {
            return new SaveUtility
            {
                _graphView = graphView
            };
        }

        public void Save(string filename)
        {
            //if (!edges.Any()) return; // Si no hay ningún link, no se guarda nada

            var graphData = ScriptableObject.CreateInstance<GraphData>();

            // PORTS
            var connectedPorts = edges.Where(x => x.input.node != null).ToArray();
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as BaseNode;
                var inputNode = connectedPorts[i].input.node as BaseNode;

                var chanceField = connectedPorts[i].output.contentContainer.Q<FloatField>("chance");
                var chanceValue = 100f;
                if (chanceField != null) chanceValue = chanceField.value;
                graphData.linkList.Add(new LinkData
                {
                    parentNodeID = outputNode.GUID,
                    portName = connectedPorts[i].output.portName,
                    childNodeID = inputNode.GUID,
                    chance = chanceValue
                });
            }

            // NODES
            foreach (Node graphNode in nodes)
            {
                if (graphNode is GraphNode) {
                    GraphNode n = (GraphNode)graphNode;
                    graphData.nodeList.Add(new NodeData
                    {
                        nodeID = n.GUID,
                        nodePos = n.GetPosition().position,
                        nodeName = n.nodeName,
                        rootNode = n.rootNode,
                        JSONProperties = n.ExportPropertyData(),
                        nodeSprite = n.nodeSprite
                    });
                } else if (graphNode is GraphConstraintNode) {
                    GraphConstraintNode c = (GraphConstraintNode)graphNode;
                    graphData.nodeList.Add(new NodeData
                    {
                        nodeID = c.GUID,
                        nodePos = c.GetPosition().position,
                        type = c.type,
                        constraintValue = c.constraintValue,
                        constraintNode = true
                    });
                } 
            }

            // INPUT PROPERTIES
            graphData.inputProperties.AddRange(_graphView.inputProperties);

            // SAVE FILE
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            EditorUtility.SetDirty(graphData);
            // It needs to be called "Resources" so we can access it when loading
            AssetDatabase.CreateAsset(graphData, $"Assets/Resources/{filename}.asset");
            AssetDatabase.SaveAssets();
        }

        public void Load(string filename)
        {
            // Load asset from Resources folder
            savedGraph = Resources.Load<GraphData>(filename);

            // If we didn't find the asset
            if (savedGraph == null)
            {
                EditorUtility.DisplayDialog("File not found", "No graph asset file with that name was found. Are you sure it's inside a 'Resources' folder?", "OK");
                return;
            }
            else
            {
                ClearGraph();
                CreateNodes();
                ConnectNodes();
                CreateInputProperties();
            }
        }

        public void ClearGraph()
        {
            //nodes.Find(x => x.RootNode).GUID = savedGraph.linkList[0].parentNodeID;

            /* foreach (var node in nodes)
            {
                //if (node.RootNode) continue;
                edges.Where(x => x.input.node == node).ToList().ForEach(edge => _graphView.RemoveElement(edge));

                _graphView.RemoveElement(node);;
            } */
            _graphView.ClearGraph(true);
        }

        private void CreateNodes()
        {
            foreach (var nodeData in savedGraph.nodeList)
            {
                if (nodeData.constraintNode) {
                    var tempNode = _graphView.CreateConstraintNode(nodeData, _graphView.defaultNodePos);
                    tempNode.GUID = nodeData.nodeID;
                    tempNode.SetPosition(new Rect(nodeData.nodePos, _graphView.defaultNodeSize));

                    _graphView.AddElement(tempNode);

                    var nodePorts = savedGraph.linkList.Where(x => x.parentNodeID == nodeData.nodeID).ToList();
                    for (int i = 0; i < nodePorts.Count; i++)
                    {
                        _graphView.GenerateChildPort(tempNode, false);
                    }
                } else {
                    if (nodeData.rootNode)
                    {
                        var tempNode = _graphView.GenerateRootNode(nodeData, false); // Don't generate default port, we add them from the saved graph
                        tempNode.GUID = nodeData.nodeID;
                        tempNode.SetPosition(new Rect(nodeData.nodePos, _graphView.defaultNodeSize));

                        _graphView.AddElement(tempNode);

                        var nodePorts = savedGraph.linkList.Where(x => x.parentNodeID == nodeData.nodeID).ToList();
                        for (int i = 0; i < nodePorts.Count; i++)
                        {
                            _graphView.GenerateChildPort(tempNode, true, nodePorts[i].chance);
                        }
                        for (int i = 0; i < nodeData.JSONProperties.Count; i++)
                        {
                            if (tempNode.nodePropertiesContainer == null)
                            {
                                VisualElement propertyContainer = new VisualElement();
                                propertyContainer.AddToClassList("property-row-container");
                                tempNode.nodePropertiesContainer = propertyContainer;
                                //node.extensionContainer.Add(propertyContainer);
                                tempNode.nodePropertiesFoldout.Add(propertyContainer);
                            }
                            tempNode.nodePropertyRows.Add(new GraphProperty(tempNode.nodePropertiesContainer, tempNode, nodeData.JSONProperties[i]));
                        }

                    }
                    else if (!nodeData.constraintNode)
                    {
                        var tempNode = _graphView.CreateNode(nodeData, _graphView.defaultNodePos);
                        tempNode.GUID = nodeData.nodeID;
                        tempNode.SetPosition(new Rect(nodeData.nodePos, _graphView.defaultNodeSize));

                        _graphView.AddElement(tempNode);

                        var nodePorts = savedGraph.linkList.Where(x => x.parentNodeID == nodeData.nodeID).ToList();
                        for (int i = 0; i < nodePorts.Count; i++)
                        {
                            _graphView.GenerateChildPort(tempNode, true, nodePorts[i].chance);
                        }
                        for (int i = 0; i < nodeData.JSONProperties.Count; i++)
                        {
                            if (tempNode.nodePropertiesContainer == null)
                            {
                                VisualElement propertyContainer = new VisualElement();
                                propertyContainer.AddToClassList("property-row-container");
                                tempNode.nodePropertiesContainer = propertyContainer;
                                //node.extensionContainer.Add(propertyContainer);
                                tempNode.nodePropertiesFoldout.Add(propertyContainer);
                            }
                            tempNode.nodePropertyRows.Add(new GraphProperty(tempNode.nodePropertiesContainer, tempNode, nodeData.JSONProperties[i]));
                        }
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

        private void CreateInputProperties() {
            foreach(var property in savedGraph.inputProperties) {
                _graphView.AddBlackboardProperty(property);
            }
        }
    }
}