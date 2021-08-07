using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ObjectModel
{
    public class GraphProperty
    {
        public Property property;
        private GraphNode graphNode;

        public GraphProperty(VisualElement parent, GraphNode node, Property savedProperty = null)
        {
            property = savedProperty;
            graphNode = node;
            var loadProperty = savedProperty!=null;
            InitializeRow(parent, PropertyType.String, loadProperty);
        }

        public GraphProperty(VisualElement parent, GraphNode node, string savedData = null)
        {
            graphNode = node;
            if (savedData != null)
            {
                property = new Property(savedData, this);
                /* switch (property.propertyType) {
                    case PropertyType.String:
                        property = new PropertyString("",savedData, this);
                    break;
                    case PropertyType.Number:
                        property = new PropertyNumber("",savedData, this);
                    break;
                    case PropertyType.Boolean:
                        property = new Property(savedData, this);
                    break;
                } */
                InitializeRow(parent, property.propertyType, true, savedData);
            } else {
                InitializeRow(parent, PropertyType.String);
            }
        }

        public void InitializeRow(VisualElement parent, PropertyType type = PropertyType.String, bool loadProperty = false, string savedData = null, bool loadValues = true)
        {
            switch (type)
            {
                case PropertyType.String:

                    if (!loadProperty)
                    {
                        property = new PropertyString("Property Name", this);
                    } else if (loadValues) {
                        property = new PropertyString("",savedData, this);
                    } else if (!loadValues) {
                        property = new PropertyString("",savedData, this, false);
                    } 
                    var propString = (PropertyString)property;
                    propString.InitializeRow(parent);
                    break;

                case PropertyType.Number: 
                         
                    if (!loadProperty)
                    {
                        property = new PropertyNumber("Property Name", this);
                    } else if (loadValues) {
                        property = new PropertyNumber("",savedData, this);
                    } else if (!loadValues) {
                        property = new PropertyNumber("",savedData, this, false);
                    } 
                    var propNumber = (PropertyNumber)property;
                    propNumber.InitializeRow(parent);
                    break;

                case PropertyType.Boolean:

                    if (!loadProperty)
                    {
                        property = new PropertyBool("Property Name", this);
                    } else if (loadValues) {
                        property = new PropertyBool("",savedData, this);
                    } else if (!loadValues) {
                        property = new PropertyBool("",savedData, this, false);
                    } 
                    var propBool = (PropertyBool)property;
                    propBool.InitializeRow(parent);
                    break;
                case PropertyType.Range:

                    if (!loadProperty)
                    {
                        property = new PropertyRange("Property Name", this);
                    } else if (loadValues) {
                        property = new PropertyRange("",savedData, this);
                    } else if (!loadValues) {
                        property = new PropertyRange("",savedData, this, false);
                    } 
                    var propRange = (PropertyRange)property;
                    propRange.InitializeRow(parent);
                    break;
                case PropertyType.Color:

                    if (!loadProperty)
                    {
                        property = new PropertyColor("Property Name", this);
                    } else if (loadValues) {
                        property = new PropertyColor("",savedData, this);
                    } else if (!loadValues) {
                        property = new PropertyColor("",savedData, this, false);
                    } 
                    var propColor = (PropertyColor)property;
                    propColor.InitializeRow(parent);
                    break;
            }
        }
        
        public void DeleteProperty()
        {
            graphNode.nodePropertyRows.Remove(this);
        }

        public string ExportPropertyData()
        {
            return property.GetAsJSON();
        }
    }
}