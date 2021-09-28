using UnityEditor;
using UnityEngine;
using ObjectModel;

namespace ProceduralGenerator
{
    [CustomEditor(typeof(ObjectDataContainer))]
    public class ObjectDataEditor : Editor
    {
        private ObjectDataContainer objectData;

        public override void OnInspectorGUI()
        {
            if (objectData == null) objectData = (ObjectDataContainer)target;
            ObjectDataContainer.ObjectData nodeData = objectData.nodeData;
            //base.OnInspectorGUI();

            // Draw Node name
            GUILayout.Label("Node name: " + nodeData.nodeName);
            
            // Draw node properties
            if (nodeData.properties != null && nodeData.properties.Count > 0)
            {
                GUILayout.Label("---------------------------------------");
                GUILayout.Label("PROPERTIES");
                GUILayout.Label("---------------------------------------");
                foreach (ObjectDataContainer.ObjectPropertyData p in nodeData.properties) {
                    GUILayout.Label("Property name: " + p.propertyName);
                    GUILayout.Label("Property type: " + p.propertyType);
                    if (p.values.Count == 1) {
                        GUILayout.Label("Value: " + p.values[0]);
                    } else {
                        GUILayout.Label("Values:");
                        for (int i = 0; i < p.values.Count; i++) {
                            GUILayout.Label("[" + i + "] " + p.values[i]);
                        }
                    }
                    GUILayout.Label("---------------------------------------");
                }
            }
            

            /* // Graph input
            generator.graphData = (GraphData)EditorGUILayout.ObjectField("Object Model Graph", generator.graphData, typeof(GraphData), false);
            if ((generator.graphData != oldGraph || !generator.HasModelBeenLoaded()) && generator.graphData != null)
            {
                generator.CreateModelFromGraph();
                oldGraph = generator.graphData;
            }

            // Spawn new object each time the Generate function is called
            generator.spawnNewObject = EditorGUILayout.Toggle("Spawn new", generator.spawnNewObject);

            // Generate button
            if (GUILayout.Button("Generate game object in scene"))
            {
                Debug.Log("Generating object");
                generator.GenerateGameObject();
            }
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Last generated object", generator.generatedGameObject, typeof(GameObject), false);
            GUI.enabled = true; */
        }
    }
}