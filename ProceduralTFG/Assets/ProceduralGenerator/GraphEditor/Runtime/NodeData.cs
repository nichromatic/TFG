using System;
using System.Collections.Generic;
using UnityEngine;


namespace ObjectModel
{
    public enum ConstraintType {
        AND,
        OR
    }
    [Serializable]
    public class NodeData
    {
        public string nodeID;
        public Vector2 nodePos;

        public string nodeName;
        public bool rootNode;
        public List<string> JSONProperties = new List<string>();

        public Texture2D nodeSprite;

        public ConstraintType type;
        public bool constraintNode;
    }
}
