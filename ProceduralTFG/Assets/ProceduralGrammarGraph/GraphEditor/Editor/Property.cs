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

        public bool roundValue;

        public Property(string name, PropertyType type, GraphProperty element)
        {
            propertyName = name;
            propertyType = type;
            graphElement = element;
        }

        public Property(string data, GraphProperty element)
        {
            Property p = UnityEngine.JsonUtility.FromJson<Property>(data);
            propertyName = p.propertyName;
            propertyType = p.propertyType;
            graphElement = element;
            multipleValues = p.multipleValues;
            minMultiple = p.minMultiple;
            maxMultiple = p.maxMultiple;
            repeatValues = p.repeatValues;
        }

        public virtual List<Object> GetValues()
        {
            return null;
        }

        public virtual List<float> GetWeights()
        {
            return null;
        }

        public virtual string GetAsJSON() {
            return (UnityEngine.JsonUtility.ToJson(this));
        }

        public virtual void SetValues(string newValues, List<float> newWeights) { }

        public virtual void InitializeRow(VisualElement parent) { }

        public virtual void InitializeRowButton(Button button) { }

        public virtual void InitializeRowValues() { }
    }
}