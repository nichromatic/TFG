using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ObjectModel
{
    public class GraphProperty
    {
        public PropertyString property;
        private GraphNode graphNode;

        public GraphProperty(VisualElement parent, GraphNode node, Property savedProperty = null)
        {
            property = (PropertyString)savedProperty;
            graphNode = node;
            InitializeRow(parent, PropertyType.String);
        }

        public GraphProperty(VisualElement parent, GraphNode node, PropertyData savedData = null)
        {
            if (savedData != null)
            {
                UnityEngine.Debug.Log(savedData.values.Count);
                property = new PropertyString(savedData, this);
                property.SetValues(savedData.values, savedData.valueWeights);
            }
            graphNode = node;
            InitializeRow(parent, PropertyType.String);
        }

        private void InitializeRow(VisualElement parent, PropertyType type = PropertyType.String)
        {
            bool loadProperty = false;
            if (property != null)
            {
                if (type != property.propertyType)
                {
                    property = null;
                }
                else
                {
                    loadProperty = true;
                }
            }

            switch (type)
            {
                case PropertyType.String:

                    if (!loadProperty)
                    {
                        property = new PropertyString("Property Name", this);
                    }

                    property.InitializeRow(parent, property.GetValues(), property.GetWeights());

                    break;
                case PropertyType.Number:                 

                    break;
                case PropertyType.Boolean:

                    break;
            }
        }
        
        public void DeleteProperty()
        {
            graphNode.nodePropertyRows.Remove(this);
        }

        public PropertyData ExportPropertyData()
        {
            PropertyData data = new PropertyData
            {
                propertyName = property.propertyName,
                propertyType = property.propertyType,
                //values = property.GetValues(),
                JSONvalues = UnityEngine.JsonUtility.ToJson(property.GetValues()),
                //valueWeights = property.GetWeights(),
                multipleValues = property.multipleValues,
                minValues = property.minMultiple,
                maxValues = property.maxMultiple,
                repeatValues = property.repeatValues
            };
            return data;
        }
    }
}