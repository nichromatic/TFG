using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectModel;

namespace ProceduralGenerator {
    public class ObjectDataContainer : MonoBehaviour
    {
        //public ProceduralObjectNode nodeData;
        public ObjectData nodeData;

        public void LoadNodeData(ProceduralObjectNode node) {
            this.nodeData = new ObjectData() {
                nodeName = node.nodeName
            };
            foreach (ProceduralObjectProperty property in node.properties) {
                nodeData.properties.Add(new ObjectPropertyData(property));
            }
        }

        public static List<ObjectDataContainer> GetAllObjectData(GameObject generatedObject) {
            return new List<ObjectDataContainer>(generatedObject.GetComponentsInChildren<ObjectDataContainer>());
        }

        public static ObjectDataContainer FindNode(string nodeName, List<ObjectDataContainer> allNodes) {
            return allNodes.Find(data => data.GetObjectData().nodeName == nodeName);
        }

        public List<string> GetValuesFromProperty(int propertyIndex) {
            return nodeData.properties[propertyIndex].values;
        }

        public List<string> GetValuesFromProperty(string propertyName) {
            return nodeData.properties.Find(p => p.propertyName == propertyName).values;
        }

        public string GetValueFromProperty(int propertyIndex, int valueIndex) {
            return nodeData.properties[propertyIndex].values[valueIndex];
        }

        public PropertyType GetPropertyType(int propertyIndex) {
            return nodeData.properties[propertyIndex].propertyType;
        }

        public PropertyType GetPropertyType(string propertyName) {
            return nodeData.properties.Find(p => p.propertyName == propertyName).propertyType;
        }

        public PropertyType GetPropertyType(ObjectPropertyData property) {
            return property.propertyType;
        }

        public int CountProperties() {
            return nodeData.properties.Count;
        }

        public int CountValuesInProperty(int propertyIndex) {
            return nodeData.properties[propertyIndex].values.Count;
        }

        public int CountValuesInProperty(string propertyName) {
            return nodeData.properties.Find(p => p.propertyName == propertyName).values.Count;
        }

        public ObjectData GetObjectData() {
            return nodeData;
        }

        public ObjectPropertyData GetProperty(int index) {
            return nodeData.properties[index];
        }

        public ObjectPropertyData GetProperty(string propertyName) {
            return nodeData.properties.Find(p => p.propertyName == propertyName);
        }

        [System.Serializable]
        public class ObjectData {
            public string nodeName;

            public List<ObjectPropertyData> properties = new List<ObjectPropertyData>();
        }

        [System.Serializable]
        public class ObjectPropertyData {
            public PropertyType propertyType;
            public string propertyName;

            public List<string> values = new List<string>();

            public ObjectPropertyData(ProceduralObjectProperty property) {
                this.propertyName = property.propertyName;
                this.propertyType = property.propertyType;
                this.values = CastToString(property.values);
            }

            public List<string> CastToString(List<object> oldValues) {
                var castList = new List<string>();
                foreach (object val in oldValues) {
                    castList.Add(val.ToString());
                }
                return castList;
            }

            public string GetValueAt(int index) {
                if (values.Count <= index) {
                    Debug.LogError("Index " + index + " is out of bounds. Property only has " + values.Count + " values.");
                    return null;
                } else {
                    return values[index];
                }
            }
        }
    }
}