﻿using UnityEditor;
using UnityEngine;
using ObjectModel;

namespace ProceduralGenerator
{
    [CustomEditor(typeof(Generator))]
    public class GeneratorEditor : Editor
    {

        private GraphData oldGraph;
        private Generator generator;

        public override void OnInspectorGUI()
        {
            if (generator == null) generator = (Generator)target;
            //base.OnInspectorGUI();

            // Graph input
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
            GUI.enabled = true;
        }
    }
}