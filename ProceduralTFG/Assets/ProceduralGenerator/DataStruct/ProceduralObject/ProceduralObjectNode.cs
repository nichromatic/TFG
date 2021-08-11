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
        public List<ProceduralObjectProperty> properties = new List<ProceduralObjectProperty>();
        public bool rootNode = false;
        public string nodeName = "";
        public Texture2D nodeSprite;

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
            nodeSprite = data.nodeSprite;
        }

        public void GenerateProperties(ModelNode modelNode) {
            foreach(PropertyData p in modelNode.nodeProperties) {
                properties.Add(new ProceduralObjectProperty(p));
            }
        }

        public void GenerateChildren(ModelBaseNode modelNode)
        {
            GenerateProperties((ModelNode)modelNode);
            modelNode.childLinks.ForEach(link =>
            {
                if (link.child is ModelNode) {
                    var coinToss = UnityEngine.Random.Range(0f, 100f);
                    if (coinToss > link.chance) return;

                    var childNode = new ProceduralObjectNode(this, (ModelNode)link.child, parentObject);
                    childNodes.Add(childNode);
                    childNode.GenerateChildren(link.child);
                } else if (link.child is ModelConstraintNode) {
                    var coinToss = UnityEngine.Random.Range(0f, 100f);
                    if (coinToss > link.chance) return;

                    HandleConstraint((ModelConstraintNode)link.child);
                }
            });
        }

        public void HandleConstraint(ModelConstraintNode constraint) {
            switch (constraint.type) {
                case ConstraintType.AND: // All child nodes are generated
                    constraint.childLinks.ForEach(link =>
                    {
                        if (link.child is ModelNode) {
                            var childNode = new ProceduralObjectNode(this, (ModelNode)link.child, parentObject);
                            childNodes.Add(childNode);
                            childNode.GenerateChildren(link.child);
                        } else if (link.child is ModelConstraintNode) {
                            HandleConstraint((ModelConstraintNode)link.child);
                        }
                    });
                break;
                case ConstraintType.OR: // We choose 1 child to generate: All others are ignored
                    var coinToss = UnityEngine.Random.Range(0, constraint.childLinks.Count);
                    var chosenChild = constraint.childLinks[coinToss].child;
                    if (chosenChild is ModelNode) {
                            var childNode = new ProceduralObjectNode(this, (ModelNode)chosenChild, parentObject);
                            childNodes.Add(childNode);
                            childNode.GenerateChildren(chosenChild);
                        } else if (chosenChild is ModelConstraintNode) {
                            HandleConstraint((ModelConstraintNode)chosenChild);
                        }
                break;
                case ConstraintType.MULTIPLY:
                    for(int i = 0; i < constraint.constraintValue; i++) {
                        constraint.childLinks.ForEach(link =>
                        {
                            if (link.child is ModelNode) {
                                var childNode = new ProceduralObjectNode(this, (ModelNode)link.child, parentObject);
                                childNodes.Add(childNode);
                                childNode.GenerateChildren(link.child);
                            } else if (link.child is ModelConstraintNode) {
                                HandleConstraint((ModelConstraintNode)link.child);
                            }
                        });
                    }
                break;
            }
        }

        public void DebugPrintNode()
        {
            if (rootNode) Debug.Log("Root node with " + childNodes.Count + " children.");
            else Debug.Log("Node " + nodeName + " with parent " + parentNode.nodeName + " and " + childNodes.Count + " children");
            childNodes.ForEach(node => node.DebugPrintNode());
        }
    }
}
