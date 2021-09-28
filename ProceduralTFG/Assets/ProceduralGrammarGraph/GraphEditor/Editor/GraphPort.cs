using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ObjectModel
{
    public class GraphPort : Port
    {
        public float chance = 100f;
        public GraphPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, System.Type type, float chance) : base(portOrientation, portDirection, portCapacity, type)
        {
            this.chance = chance;
        }
    }
}