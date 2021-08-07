using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectModel
{
    public class ModelConstraintNode : ModelBaseNode
    {
        public ConstraintType type;

        public ModelConstraintNode(NodeData nodeData) : base(nodeData)
        {
            this.type = nodeData.type;
        }
    }
}