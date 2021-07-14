using System;
using System.Collections;
using System.Collections.Generic;

namespace ObjectModel
{
    public enum PropertyType
    {
        String,
        Number,
        Boolean,
        Range
    }
    [System.Serializable]
    public class PropertyData
    {
        public string propertyName;
        public PropertyType propertyType;

        public bool multipleValues;
        public bool repeatValues;
        public int minMultiple;
        public int maxMultiple;

        public bool roundValue;

        public List<string> values = new List<string>();
        public List<float> valueWeights = new List<float>();
    }
}