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

        public GraphData graphData;                 // Graph data used to build the model
        private Model objectModel;                  // Built object model
        public GraphData oldGraph;                  // Duplicate of loaded graph for checking when it's changed

        public int seed = 0;                        // Seed for the generation process
        public bool useRandomSeed = true;           // If true, ignore seed and use a random one each time

        public bool spawnNewObject = true;          // Determines if the same object is reset whenever we generate a new one
        public GameObject generatedGameObject;      // Last generated object (gameobject)
        public ProceduralObject generatedObject;    // Last generated object (abstract)
        public GameObject parentGameObject;         // Set this as the parent for the generated object(s)

        void Awake() {
            CreateModelFromGraph();
        }

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

            if (useRandomSeed) Random.InitState((int)System.DateTime.Now.Ticks);
            else Random.InitState(seed);

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
            var objData = generatedGameObject.AddComponent<ObjectDataContainer>();
            objData.LoadNodeData(obj.rootNode);
            if (obj.rootNode.nodeSprite != null) {
                var spriteRenderer = generatedGameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Sprite.Create(obj.rootNode.nodeSprite, new Rect(0.0f, 0.0f, obj.rootNode.nodeSprite.width, obj.rootNode.nodeSprite.height), new Vector2(0.5f, 0.5f), 100.0f);
                for (int i = 0; i < obj.rootNode.spriteModifiers.Count; i++) {
                    string propname = obj.rootNode.spriteModifiers[i];
                    //Debug.Log("Modifiers: " + propname);
                    if (propname != null && propname != "") {
                        var property = obj.rootNode.properties.Find(p => p.propertyName == propname);
                        //Debug.Log("Property found: " + property);
                        if (property != null && property.values.Count > 0) {
                            switch (i) {
                                case 0: // Color tint
                                    Color color;
                                    ColorUtility.TryParseHtmlString((string)property.values[0], out color);
                                    if (color != null) spriteRenderer.color = color;
                                break;
                                case 1: // X Scale
                                    float x = (float)property.values[0];
                                    generatedGameObject.transform.localScale += new Vector3(x,0,0);
                                break;
                                case 2: // Y Scale
                                    float y = (float)property.values[0];
                                    generatedGameObject.transform.localScale += new Vector3(0,y,0);
                                break;
                                case 3: // X Offset
                                    float xOff = (float)property.values[0];
                                    generatedGameObject.transform.localPosition += new Vector3(xOff, 0, 0);
                                break;
                                case 4: // Y Offset
                                    float yOff = (float)property.values[0];
                                    generatedGameObject.transform.localPosition += new Vector3(0, yOff, 0);
                                break;
                            }
                        }
                    }
                }
            }
            GenerateChildGameObjects(generatedGameObject.transform, obj.rootNode);

            if (parentGameObject != null) {
                generatedGameObject.transform.SetParent(parentGameObject.transform);
                generatedGameObject.transform.localPosition = new Vector3(0,0,0);
                generatedGameObject.transform.localScale = new Vector3(1,1,1);
            }

            return generatedGameObject;
        }

        public void SetSeed(int newSeed, bool useRandom = false) {
            this.seed = newSeed;
            this.useRandomSeed = useRandom;
        }

        public void SetRandomSeed(bool useRandom = true) {
            this.useRandomSeed = useRandom;
        }

        private void GenerateChildGameObjects(Transform parent, ProceduralObjectNode node, int depth = 1)
        {
            node.childNodes.ForEach(child =>
            {
                var go = new GameObject(child.nodeName);
                go.transform.SetParent(parent);
                go.transform.localPosition = new Vector3(0,0,0);
                go.transform.localScale = new Vector3(1,1,1);
                var objData = go.AddComponent<ObjectDataContainer>();
                objData.LoadNodeData(child);
                if (child.nodeSprite != null) {
                    var spriteRenderer = go.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = Sprite.Create(child.nodeSprite, new Rect(0.0f, 0.0f, child.nodeSprite.width, child.nodeSprite.height), new Vector2(0.5f, 0.5f), 100.0f);
                    spriteRenderer.sortingOrder = depth;
                    for (int i = 0; i < child.spriteModifiers.Count; i++) {
                        var propname = child.spriteModifiers[i];
                        if (propname != null && propname != "") {
                            var property = child.properties.Find(p => p.propertyName == propname);
                            //Debug.Log("Property found: " + property);
                            if (property != null && property.values.Count > 0) {
                                switch (i) {
                                    case 0: // Color tint
                                        Color color;
                                        ColorUtility.TryParseHtmlString((string)property.values[0], out color);
                                        if (color != null) spriteRenderer.color = color;
                                    break;
                                    case 1: // X Scale
                                        float x = (float)property.values[0];
                                        go.transform.localScale  = Vector3.Scale(go.transform.localScale, new Vector3(x,1,1));
                                    break;
                                    case 2: // Y Scale
                                        float y = (float)property.values[0];
                                        go.transform.localScale  = Vector3.Scale(go.transform.localScale, new Vector3(1,y,1));
                                    break;
                                    case 3: // X Offset
                                        float xOff = (float)property.values[0];
                                        go.transform.localPosition += new Vector3(xOff, 0, 0);
                                    break;
                                    case 4: // Y Offset
                                        float yOff = (float)property.values[0];
                                        go.transform.localPosition += new Vector3(0, yOff, 0);
                                    break;
                                }
                            }
                        }
                    }
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
