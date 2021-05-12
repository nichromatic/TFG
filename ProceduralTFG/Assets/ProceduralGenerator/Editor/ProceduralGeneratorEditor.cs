using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralGenerator))]
public class ProceduralGeneratorEditor : Editor
{

    private ObjectModelGraphData oldGraph;
    private ProceduralGenerator generator;

    public override void OnInspectorGUI()
    {
        if (generator == null) generator = (ProceduralGenerator)target;
        //base.OnInspectorGUI();

        // Graph input
        generator.graphData = (ObjectModelGraphData)EditorGUILayout.ObjectField("Object Model Graph",generator.graphData, typeof(ObjectModelGraphData), false);
        if ((generator.graphData != oldGraph || !generator.HasModelBeenLoaded()) && generator.graphData != null)
        {
            generator.CreateModelFromGraph();
            oldGraph = generator.graphData;
        }

        // Spawn new object each time the Generate function is called
        generator.spawnNewObject = EditorGUILayout.Toggle("Spawn new",generator.spawnNewObject);

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
