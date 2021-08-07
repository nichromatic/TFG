using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ObjectModel
{
    public class GraphConstraintNode : BaseNode
    {
        public bool rootNode = false;
        public string nodeName;

        public ConstraintType type = ConstraintType.AND;

        private int nextPortID = -1;

        public string GetNextPortID()
        {
            nextPortID++;
            return nextPortID.ToString();
        }

        public string GetConstraintDescription() {
            switch (this.type) {
                case ConstraintType.AND: return "If one of these nodes is generated, the rest will be generated too.";
                case ConstraintType.OR: return "Only one of these nodes will be generated, the rest will be ignored.";
            }
            return "";
        }
    }
}