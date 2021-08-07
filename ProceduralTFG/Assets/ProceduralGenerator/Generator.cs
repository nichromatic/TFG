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
            var objData = generatedGameObject.AddComponent<ObjectData>();
            objData.nodeData = obj.rootNode;
            if (obj.rootNode.nodeSprite != null) {
                generatedGameObject.AddComponent<SpriteRenderer>().sprite = Sprite.Create(obj.rootNode.nodeSprite, new Rect(0.0f, 0.0f, obj.rootNode.nodeSprite.width, obj.rootNode.nodeSprite.height), new Vector2(0.5f, 0.5f), 100.0f);
            }
            GenerateChildGameObjects(generatedGameObject.transform, obj.rootNode);

            return generatedGameObject;
        }

        private void GenerateChildGameObjects(Transform parent, ProceduralObjectNode node, int depth = 1)
        {
            node.childNodes.ForEach(child =>
            {
                var go = new GameObject(child.nodeName);
                go.transform.SetParent(parent);
                var objData = go.AddComponent<ObjectData>();
                objData.nodeData = child;
                if (child.nodeSprite != null) {
                    var spriteRenderer = go.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = Sprite.Create(child.nodeSprite, new Rect(0.0f, 0.0f, child.nodeSprite.width, child.nodeSprite.height), new Vector2(0.5f, 0.5f), 100.0f);
                    spriteRenderer.sortingOrder = depth;
                }
                GenerateChildGameObjects(go.transform, child, depth+1);
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
