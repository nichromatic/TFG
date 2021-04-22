using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ObjectModelGraphPort : Port
{
    public float chance = 100f;
    public ObjectModelGraphPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, System.Type type, float chance) : base(portOrientation, portDirection, portCapacity, type)
    {
        this.chance = chance;
    }
}
