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

        // Generate button
        if (GUILayout.Button("Generate object"))
        {
            Debug.Log("Generating object");
            generator.Generate();
        }
    }
}
