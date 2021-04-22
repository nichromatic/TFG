using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{

    public ObjectModelGraphData graphData;      // Graph data used to build the model
    private ObjectModel generatedModel;         // Built object model

    public ProceduralObject generatedObject;    // Generated object (abstract)

    public bool generateGameObject;             // Determines whether or not a GO is generated in the scene
    public GameObject generatedGameObject;      // Generated object (gameobject)

    [ContextMenu("Generate Object")]
    public void Generate()
    {
        if (graphData == null)
        {
            Debug.LogError("There is no ObjectModel associated with this Generator.");
            return;
        } else if (generatedModel == null)
        {
            Debug.LogError("The Generator didn't build the model properly. Please try selecting the ObjectModel again.");
            return;
        }

        generatedObject = new ProceduralObject(generatedModel);
        
    }

    public void CreateModelFromGraph()
    {
        if (graphData == null)
        {
            Debug.LogError("There is no ObjectModel associated with this Generator.");
            return;
        }

        generatedModel = new ObjectModel();
        generatedModel.BuildModelFromGraph(graphData);
    }


    public bool HasModelBeenLoaded()
    {
        return generatedModel != null;
    }
}
