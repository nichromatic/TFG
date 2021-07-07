using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ObjectModel;

namespace ProceduralGenerator
{
    public class Generator : MonoBehaviour
    {

        public GraphData graphData;      // Graph data used to build the model
        private Model objectModel;         // Built object model

        public ProceduralObject generatedObject;    // Last generated object (abstract)

        public bool spawnNewObject = true;    // Determines if the same object is reset whenever we generate a new one
        public GameObject generatedGameObject;          // Last generated object (gameobject)

        public ProceduralObject Generate()
        {
            if (graphData == null)
            {
                Debug.LogError("There is no ObjectModel associated with this Generator.");
                return null;
            }
            else if (objectModel == null)
            {
                Debug.LogError("The Generator didn't build the model properly. Please try selecting the ObjectModel again.");
                return null;
            }

            generatedObject = new ProceduralObject(objectModel);

            return generatedObject;
        }

        public GameObject GenerateGameObject(ProceduralObject obj = null)
        {
            if (obj == null) obj = Generate();

            if (!spawnNewObject)
            {
                DestroyImmediate(generatedGameObject);
            }
            generatedGameObject = new GameObject(obj.rootNode.nodeName);
            GenerateChildGameObjects(generatedGameObject.transform, obj.rootNode);

            return generatedGameObject;
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

            objectModel = new Model();
            objectModel.BuildModelFromGraph(graphData);
        }


        public bool HasModelBeenLoaded()
        {
            return objectModel != null;
        }
    }
}
