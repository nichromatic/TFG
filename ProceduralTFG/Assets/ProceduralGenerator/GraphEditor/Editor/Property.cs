using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ObjectModel {
    [Serializable]
    public class Property
    {
        public string propertyName;
        public PropertyType propertyType;

        public VisualElement parentElement;
        public VisualElement varRow;
        public VisualElement valueContainer;
        public GraphProperty graphElement;

        public bool multipleValues;
        public bool repeatValues;
        public int minMultiple;
        public int maxMultiple;

        public Property(string name, PropertyType type, GraphProperty element)
        {
            propertyName = name;
            propertyType = type;
            graphElement = element;
        }

        public Property(PropertyData data, GraphProperty element)
        {
            propertyName = data.propertyName;
            propertyType = data.propertyType;
            graphElement = element;
            multipleValues = data.multipleValues;
            minMultiple = data.minValues;
            maxMultiple = data.maxValues;
            repeatValues = data.repeatValues;
        }

        public virtual List<Object> GetValues()
        {
            return null;
        }

        public virtual List<float> GetWeights()
        {
            return null;
        }

        public virtual void SetValues(List<Object> newValues, List<float> newWeights) { }

        public virtual void InitializeRow(VisualElement parent, List<Object> values = null, List<float> weights = null) { }

        public virtual void InitializeRowButton(Button button) { }

        public virtual void InitializeRowValues(List<Object> values = null, List<float> weights = null) { }
    }
}