using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectModel
{
    public enum PropertyType
    {
        String,
        Number,
        Boolean
    }
    [System.Serializable]
    public class PropertyData
    {
        public string propertyName;
        public PropertyType propertyType;

        public string JSONvalues;
        public List<object> values = new List<object>();

        public List<float> valueWeights = new List<float>();

        public bool multipleValues;
        public int minValues;
        public int maxValues;
        public bool repeatValues;
    }
}