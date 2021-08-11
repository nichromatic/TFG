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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region NODE FUNCTIONS

        public GraphNode CreateNode(NodeData data, Vector2 position)
        {
            var name = "Node";
            if (data != null) name = data.nodeName;
            Texture2D sprite = null;
            if (data != null) sprite = data.nodeSprite;

            var newNode = new GraphNode
            {
                GUID = Guid.NewGuid().ToString(),
                rootNode = false,
                nodeName = name,
                nodeSprite = sprite
            };

            // Parent (input) port
            var inputPort = GenerateParentPort(newNode);

            // Textfield for the nodeName
            newNode.InitializeNameField();

            // Btn to add child (output) ports
            var addChildBtn = new Button(() =>
            {
                var newOutputPort = GenerateChildPort(newNode);
            });
            addChildBtn.text = "Add child";
            newNode.titleButtonContainer.Add(addChildBtn);
            //newNode.outputContainer.Insert(0,addChildBtn);

            var propertyFoldout = new Foldout();
            var addPropertyBtn = new Button(() =>
            {
                CreateVariableInputRow(newNode);
            });
            addPropertyBtn.text = "Add property";
            addPropertyBtn.AddToClassList("addBtn");
            propertyFoldout.Add(addPropertyBtn);
            propertyFoldout.value = false;
            propertyFoldout.text = "Node properties";
            propertyFoldout.AddToClassList("nodeFoldout");
            propertyFoldout.AddToClassList("propertyFoldout");
            newNode.extensionContainer.Add(propertyFoldout);
            newNode.nodePropertiesFoldout = propertyFoldout;

            var graphicFoldout = new Foldout();
            /* var addGraphicBtn = new Button(() =>
            {
                newNode.InitializeSpriteField(graphicFoldout);
            });
            addGraphicBtn.text = "Add sprite";
            addGraphicBtn.AddToClassList("addBtn");
            graphicFoldout.Add(addGraphicBtn); */
            newNode.InitializeSpriteField(graphicFoldout, newNode.nodeSprite);
            if (newNode.nodeSprite == null) graphicFoldout.value = false;
            graphicFoldout.text = "Visual representation";
            graphicFoldout.AddToClassList("nodeFoldout");
            graphicFoldout.AddToClassList("graphicFoldout");
            newNode.extensionContainer.Add(graphicFoldout);

            RefreshNode(newNode);
            newNode.SetPosition(new Rect(position, defaultNodeSize));

            return newNode;
        }

        public GraphConstraintNode CreateConstraintNode(NodeData data, Vector2 position)
        {

            var newNode = new GraphConstraintNode
            {
                GUID = Guid.NewGuid().ToString(),
            };

            if (data != null)  {
                newNode.type = data.type;
                newNode.constraintValue = data.constraintValue;
            }

            // Parent (input) port
            var inputPort = GenerateParentPort(newNode);

            // Btn to add child (output) ports
            var addChildBtn = new Button(() =>
            {
                var newOutputPort = GenerateChildPort(newNode, false);
            });
            addChildBtn.text = "Add child";
            newNode.titleButtonContainer.Add(addChildBtn);

            var constraintValue = new IntegerField() {
                label = "Repeat times",
                value = newNode.constraintValue
            };
            constraintValue.RegisterValueChangedCallback(e =>
            {
                newNode.constraintValue = e.newValue;
            });
            constraintValue.AddToClassList("constraintNodeValue");
            if (newNode.type != ConstraintType.MULTIPLY) constraintValue.AddToClassList("display-none");

            var constraintDescription = new TextElement();
            constraintDescription.text = newNode.GetConstraintDescription();
            var constraintDescriptionContainer = new VisualElement();
            constraintDescriptionContainer.AddToClassList("constraintNodeDescription");
            constraintDescriptionContainer.Add(constraintValue);
            constraintDescriptionContainer.Add(constraintDescription);
            newNode.extensionContainer.Add(constraintDescriptionContainer);

            var constraintTypeSelect = new EnumField(newNode.type);
            constraintTypeSelect.RegisterValueChangedCallback(e =>
            {
                newNode.type = (ConstraintType)e.newValue;
                constraintDescription.text = newNode.GetConstraintDescription();
                if (newNode.type == ConstraintType.MULTIPLY) {
                    constraintValue.RemoveFromClassList("display-none");
                } else if (!constraintValue.ClassListContains("display-none")) {
                    constraintValue.AddToClassList("display-none");
                }
            });
            constraintTypeSelect.AddToClassList("titleEnumField");
            newNode.titleContainer.Insert(0, constraintTypeSelect);

            //newNode.outputContainer.Insert(0,addChildBtn);

            newNode.mainContainer.AddToClassList("constraintNode");
            RefreshNode(newNode);
            newNode.SetPosition(new Rect(position, defaultNodeSize));

            return newNode;
        }

        public GraphNode GenerateRootNode(NodeData data = null, bool addDefaultPort = true)
        {
            var name = "Root node";
            if (data != null) name = data.nodeName;
            Texture2D sprite = null;
            if (data != null) sprite = data.nodeSprite;

            var rootNode = new GraphNode
            {
                GUID = Guid.NewGuid().ToString(),
                rootNode = true,
                nodeName = name,
                nodeSprite = sprite
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
            rootNode.mainContainer.AddToClassList("rootNode");
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


            var propertyFoldout = new Foldout();
            var addPropertyBtn = new Button(() =>
            {
                CreateVariableInputRow(rootNode);
            });
            addPropertyBtn.text = "Add property";
            addPropertyBtn.AddToClassList("addBtn");
            propertyFoldout.Add(addPropertyBtn);
            propertyFoldout.value = false;
            propertyFoldout.text = "Node properties";
            propertyFoldout.AddToClassList("nodeFoldout");
            propertyFoldout.AddToClassList("propertyFoldout");
            rootNode.extensionContainer.Add(propertyFoldout);
            rootNode.nodePropertiesFoldout = propertyFoldout;

            var graphicFoldout = new Foldout();
            /* var addGraphicBtn = new Button(() =>
            {
                rootNode.InitializeSpriteField(graphicFoldout);
            });
            addGraphicBtn.text = "Add sprite";
            addGraphicBtn.AddToClassList("addBtn");
            graphicFoldout.Add(addGraphicBtn); */
            rootNode.InitializeSpriteField(graphicFoldout, rootNode.nodeSprite);
            if (rootNode.nodeSprite == null) graphicFoldout.value = false;
            graphicFoldout.text = "Visual representation";
            graphicFoldout.AddToClassList("nodeFoldout");
            graphicFoldout.AddToClassList("graphicFoldout");
            rootNode.extensionContainer.Add(graphicFoldout);
            rootNode.capabilities &= ~Capabilities.Deletable; // We disable the ability to delete the root node
            rootNode.SetPosition(new Rect(defaultRootNodePos, defaultNodeSize));

            RefreshNode(rootNode);

            return rootNode;
        }
        
        public void CreateVariableInputRow(GraphNode node, PropertyType type = PropertyType.String, Property baseProperty = null)
        {
            if (node.nodePropertiesContainer == null)
            {
                VisualElement propertyContainer = new VisualElement();
                propertyContainer.AddToClassList("property-row-container");
                node.nodePropertiesContainer = propertyContainer;
                //node.extensionContainer.Add(propertyContainer);
                node.nodePropertiesFoldout.Add(propertyContainer);
            }
            node.nodePropertyRows.Add(new GraphProperty(node.nodePropertiesContainer, node, baseProperty));
        }

        #endregion
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region PORT FUNCTIONS

        public Port GenerateParentPort(Node node, bool addToNode = true, String name = "Parent", Port.Capacity capacity = Port.Capacity.Single)
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

        public Port GenerateChildPort(Node node, bool showChance = true, float chance = 100f, bool addToNode = true, String name = "childPort", Port.Capacity capacity = Port.Capacity.Single)
        {
            var port = node.InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, typeof(float));
            if (node is GraphNode) port.portName = name + ((GraphNode)node).GetNextPortID();
            else if (node is GraphConstraintNode) port.portName = name + ((GraphConstraintNode)node).GetNextPortID();

            if (showChance) {
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
            }

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

        private void RemovePort(Node node, Port port)
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region BLACKBOARD

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

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region GRAPH INTERFACE

        private void RefreshNode(Node node)
        {
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        public void ClearGraph(bool removeRoot = false, bool removeRootPorts = true, bool generateRoot = false)
        {
            var graphNodes = nodes.ToList().Cast<BaseNode>().ToList();
            var graphLinks = edges.ToList();

            foreach (var node in graphNodes)
            {
                // First we delete all edges connected to this node
                graphLinks.Where(x => x.input.node == node).ToList().ForEach(link => RemoveElement(link));

                var isRootNode = false;  
                if (node is GraphNode && ((GraphNode)node).rootNode) isRootNode = true;
                // If it's the rootNode but we don't want to delete it ...
                if (isRootNode && !removeRoot)
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

            if (generateRoot) InitializeGraph();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is Graph)
            {
                evt.menu.AppendAction("Create node", (e) =>
                {
                    //Debug.Log("Created node through context menu at position: " + e.eventInfo.mousePosition);
                    AddElement(CreateNode(null, e.eventInfo.mousePosition));
                });
                evt.menu.AppendAction("Create constraint node", (e) =>
                {
                    //Debug.Log("Created node through context menu at position: " + e.eventInfo.mousePosition);
                    AddElement(CreateConstraintNode(null, e.eventInfo.mousePosition));
                });
            }
            base.BuildContextualMenu(evt);
        }

        #endregion
    }
}
