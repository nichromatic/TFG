using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{

    public ObjectModelGraphData graphData;      // Graph data used to build the model
    private ObjectModel generatedModel;         // Built object model

    public ProceduralObject generatedObject;    // Last generated object (abstract)

    public bool generateGameObject = true;          // Determines whether or not a GO is generated in the scene
    public bool regenerateSameGameObject = true;    // Determines if the same object is reset whenever we generate a new one
    public GameObject generatedGameObject;          // Last generated object (gameobject)

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
        if (generateGameObject) GenerateGameObject(generatedObject);
    }

    private void GenerateGameObject(ProceduralObject obj)
    {
        if (regenerateSameGameObject)
        {
            DestroyImmediate(generatedGameObject);
        }
        generatedGameObject = new GameObject(obj.rootNode.nodeName);
        GenerateChildGameObjects(generatedGameObject.transform, obj.rootNode);
    }

    private void GenerateChildGameObjects(Transform parent, ProceduralObjectNode node)
    {
        node.childNodes.ForEach(child =>
        {
            var go = new GameObject(child.nodeName);
            go.transform.SetParent(parent);
            GenerateChildGameObjects(go.transform, child);
        });
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
