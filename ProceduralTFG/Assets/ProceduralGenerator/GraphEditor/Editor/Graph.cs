using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;
using UnityEditor.UIElements;

namespace ObjectModel
{
    public class Graph : GraphView
    {
        public readonly Vector2 defaultNodePos = new Vector2(0, 0);
        public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
        public readonly Vector2 defaultRootNodePos = new Vector2(325, 300);

        public Blackboard _blackboard;
        public List<InputProperty> inputProperties = new List<InputProperty>();

        public Graph()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var gridBg = new GridBackground();
            gridBg.AddToClassList("graphGridBg");
            Insert(0, gridBg);
            gridBg.StretchToParentSize();

            InitializeGraph();
        }

        public void InitializeGraph()
        {
            AddElement(GenerateRootNode());
        }

        public GraphNode CreateNode(string name, Vector2 position)
        {
            var newNode = new GraphNode
            {
                GUID = Guid.NewGuid().ToString(),
                rootNode = false,
                nodeName = name
            };

            // Parent (input) port
            var inputPort = GenerateParentPort(newNode);

            // Textfield for the nodeName
            var textField = new TextField()
            {
                name = string.Empty,
                value = name
            };
            textField.RegisterValueChangedCallback(e =>
            {
                newNode.nodeName = e.newValue;
            });
            textField.AddToClassList("titleTextField");
            newNode.titleContainer.Remove(newNode.titleContainer.Q<Label>("title-label"));
            newNode.titleContainer.Insert(0, textField);

            // Btn to add child (output) ports
            var addChildBtn = new Button(() =>
            {
                var newOutputPort = GenerateChildPort(newNode);
            });
            addChildBtn.text = "Add child";
            newNode.titleButtonContainer.Add(addChildBtn);

            var addPropertyBtn = new Button(() =>
            {
                CreateVariableInputRow(newNode);
            });
            addPropertyBtn.text = "Add property";
            newNode.extensionContainer.Add(addPropertyBtn);


            RefreshNode(newNode);
            newNode.SetPosition(new Rect(position, defaultNodeSize));

            return newNode;
        }

        public void CreateVariableInputRow(GraphNode node, PropertyType type = PropertyType.String, Property baseProperty = null)
        {
            if (node.nodePropertiesContainer == null)
            {
                VisualElement propertyContainer = new VisualElement();
                propertyContainer.AddToClassList("property-row-container");
                node.nodePropertiesContainer = propertyContainer;
                node.extensionContainer.Add(propertyContainer);
            }
            node.nodePropertyRows.Add(new GraphProperty(node.nodePropertiesContainer, node, baseProperty));
        }

        public GraphNode GenerateRootNode(string name = "Root node", bool addDefaultPort = true)
        {
            var rootNode = new GraphNode
            {
                GUID = Guid.NewGuid().ToString(),
                rootNode = true,
                nodeName = name
            };

            // Textfield for the nodeName
            var textField = new TextField()
            {
                name = string.Empty,
                value = name
            };
            textField.RegisterValueChangedCallback(e =>
            {
                rootNode.nodeName = e.newValue;
            });
            textField.AddToClassList("titleTextField");
            rootNode.titleContainer.Remove(rootNode.titleContainer.Q<Label>("title-label"));
            rootNode.titleContainer.Insert(0, textField);

            // We add 1 child (output) port by default if needed
            if (addDefaultPort)
            {
                var outputPort = GenerateChildPort(rootNode);
                rootNode.outputContainer.Add(outputPort);
            }

            // Btn to add child (output) ports
            var addChildBtn = new Button(() =>
            {
                var newOutputPort = GenerateChildPort(rootNode);
            });
            addChildBtn.text = "Add child";
            rootNode.titleButtonContainer.Add(addChildBtn);


            // TODO Add global variables to node
            /*var addVarBtn = new Button(() =>
            {
                RefreshNode(rootNode);
            });
            addVarBtn.text = "Add variable";
            rootNode.mainContainer.Add(addVarBtn);*/

            rootNode.capabilities &= ~Capabilities.Deletable; // We disable the ability to delete the root node
            rootNode.SetPosition(new Rect(defaultRootNodePos, defaultNodeSize));

            RefreshNode(rootNode);

            return rootNode;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region PORT FUNCTIONS

        /// <summary>
        /// Method for generating the port that connects to the parent of this node (input).
        /// </summary>
        public Port GenerateParentPort(GraphNode node, bool addToNode = true, String name = "Parent", Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = node.InstantiatePort(Orientation.Horizontal, Direction.Input, capacity, typeof(float));
            port.portName = name;
            if (addToNode)
            {
                node.inputContainer.Add(port);
                RefreshNode(node);
            }
            return port;
        }

        /// <summary>
        /// Method for generating the port that connects to a child of this node (output).
        /// </summary>
        public Port GenerateChildPort(GraphNode node, float chance = 100f, bool addToNode = true, String name = "childPort", Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = node.InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, typeof(float));
            port.portName = name + node.GetNextPortID();

            var chanceLabel = new Label("%");
            chanceLabel.AddToClassList("port-label");
            port.contentContainer.Add(chanceLabel);
            var chanceField = new FloatField()
            {
                name = "chance",
                value = chance
            };
            chanceField.RegisterValueChangedCallback(evt =>
            {
                var clampedValue = Mathf.Clamp(evt.newValue, 0, 100);
                chanceField.value = clampedValue;
            });
            port.contentContainer.Add(chanceField);

            var deleteButton = new Button(() => RemovePort(node, port)) { text = "X" };
            deleteButton.AddToClassList("deleteButton");
            port.contentContainer.Add(deleteButton);

            port.contentContainer.Q<Label>("type").AddToClassList("hidden-label");

            if (addToNode)
            {
                node.outputContainer.Add(port);
                RefreshNode(node);
            }
            return port;
        }

        /// <summary>
        /// Remove port from node, making sure that any edges connected to it are deleted first.
        /// </summary>
        private void RemovePort(GraphNode node, Port port)
        {
            var portEdge = edges.ToList().Where(x => x.output.portName == port.portName && x.output.node == port.node);
            if (portEdge.Any())
            {
                var edge = portEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(portEdge.First());
            }

            node.outputContainer.Remove(port);
            RefreshNode(node);
        }

        /// <summary>
        /// Overrides the List.GetCompatiblePorts() method to add some custom conditions.
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node) compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        #endregion


        public void AddBlackboardProperty(InputProperty input) {
            var newInput = new InputProperty();
            var newInputName = input.propertyName;

            while(inputProperties.Any(i => i.propertyName == newInputName))
                newInputName += "(1)";
            newInput.propertyName = newInputName;

            inputProperties.Add(newInput);

            var container = new VisualElement();
            var field = new BlackboardField{text = newInput.propertyName, typeText = "JSON file"};
            container.Add(field);

            _blackboard.Add(container);
        }
        private void RefreshNode(GraphNode node)
        {
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        public void ClearGraph(bool removeRoot = false, bool removeRootPorts = true)
        {
            var graphNodes = nodes.ToList().Cast<GraphNode>().ToList();
            var graphLinks = edges.ToList();

            foreach (var node in graphNodes)
            {
                // First we delete all edges connected to this node
                graphLinks.Where(x => x.input.node == node).ToList().ForEach(link => RemoveElement(link));

                // If it's the rootNode but we don't want to delete it ...
                if (node.rootNode && !removeRoot)
                {
                    // ... if we don't want to delete its ports either, we skip the rest of this iteration
                    if (!removeRootPorts) continue;
                    // ... if we do want to delete its ports, we remove all of them except the default one
                    else
                    {
                        var numPorts = node.outputContainer.childCount;
                        for (int i = 1; i < numPorts; i++)
                        {
                            node.outputContainer.RemoveAt(0);
                        }
                    }
                }
                // If this isn't the rootNode, or if we want to delete the rootNode, we remove it from the graph
                else
                {
                    RemoveElement(node);
                }
            }

            inputProperties.Clear();
            _blackboard.Clear();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is Graph)
            {
                evt.menu.AppendAction("Create node", (e) =>
                {
                    //Debug.Log("Created node through context menu at position: " + e.eventInfo.mousePosition);
                    AddElement(CreateNode("Node", e.eventInfo.mousePosition));
                });
            }
            base.BuildContextualMenu(evt);
        }
    }
}
