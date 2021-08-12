using UnityEditor;
using UnityEngine;
using ObjectModel;

namespace ProceduralGenerator
{
    [CustomEditor(typeof(Generator))]
    public class GeneratorEditor : Editor
    {
        private Generator generator;

        public override void OnInspectorGUI()
        {
            if (generator == null) generator = (Generator)target;
            //base.OnInspectorGUI();

            // Graph input
            EditorGUILayout.LabelField("Object Model", EditorStyles.boldLabel);
            generator.graphData = (GraphData)EditorGUILayout.ObjectField("Object Model Graph", generator.graphData, typeof(GraphData), false);
            if ((generator.graphData != generator.oldGraph || !generator.HasModelBeenLoaded()) && generator.graphData != null)
            {
                generator.CreateModelFromGraph();
                generator.oldGraph = generator.graphData;
            }

            // Seed input
            EditorGUILayout.LabelField("Generator Seed", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(generator.useRandomSeed); 
            generator.seed = EditorGUILayout.IntField("Seed", generator.seed);
            EditorGUI.EndDisabledGroup();
            generator.useRandomSeed = EditorGUILayout.Toggle("Use random seed", generator.useRandomSeed);

            // Spawn new object each time the Generate function is called
            EditorGUILayout.LabelField("GameObject Options", EditorStyles.boldLabel);
            generator.spawnNewObject = EditorGUILayout.Toggle("Spawn new each time", generator.spawnNewObject);
            generator.parentGameObject = (GameObject)EditorGUILayout.ObjectField("Parent object (optional)", generator.parentGameObject, typeof(GameObject), true);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Last generated object", generator.generatedGameObject, typeof(GameObject), true);
            EditorGUI.EndDisabledGroup();

            // Generate button
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            if (GUILayout.Button("Generate game object in scene"))
            {
                Debug.Log("Generating object");
                generator.GenerateGameObject();
            }
        }
    }
}