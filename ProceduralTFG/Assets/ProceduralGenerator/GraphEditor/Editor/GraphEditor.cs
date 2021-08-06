using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System.Linq;

namespace ObjectModel
{
    public class GraphEditor : GraphViewEditorWindow
    {
        private Graph _graphView;
        private string graphName = "New Object Model";

        [UnityEditor.MenuItem("Procedural Generator/Object Model Graph")]
        public static void OpenGraphWindow()
        {
            var window = GetWindow<GraphEditor>();
            window.titleContent = new GUIContent("Object Model Graph");
        }

        public void OnEnable()
        {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>("ObjectModelGraphStyle"));
            InitializeGraph();
            InitializeToolbar();
            InitializeBlackboard();
        }

        public void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void InitializeGraph()
        {
            _graphView = new Graph
            {
                name = "Object Model Graph"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void InitializeToolbar()
        {
            var toolbarTemplate = Resources.Load<VisualTreeAsset>("ObjectModelGraphToolbar");
            toolbarTemplate.CloneTree(rootVisualElement);
            var toolbar = rootVisualElement.Query<Toolbar>().ToList()[0];

            var toolButtons = toolbar.Query<Button>();
            toolButtons.ForEach(button => InitializeToolbarButton(button));

            var fileNameField = toolbar.Query<TextField>("filename").ToList()[0];
            var fileNameLabel = toolbar.Query<Label>("filenameLabel").ToList()[0];
            fileNameLabel.text = "Filename";
            fileNameField.SetValueWithoutNotify(graphName);
            fileNameField.MarkDirtyRepaint();
            fileNameField.RegisterValueChangedCallback(evt => graphName = evt.newValue);
        }

        private void InitializeToolbarButton(Button button)
        {
            switch (button.name)
            {
                case "clearGraphBtn":
                    button.text = "Clear Graph";
                    button.clickable.clicked += () =>
                    {
                        if (EditorUtility.DisplayDialog("Clear graph", "Are you sure you want to delete the graph? All data not saved to an asset will be lost.", "Clear graph", "Cancel"))
                        {
                            _graphView.ClearGraph(true, true, true);
                        } 
                    };
                    break;
                case "saveGraphBtn":
                    button.text = "Save to file";
                    button.clickable.clicked += () =>
                    {
                        RequestSystemOperation(true);
                    };
                    break;
                case "loadGraphBtn":
                    button.text = "Load from file";
                    button.clickable.clicked += () =>
                    {
                        RequestSystemOperation(false);
                    };
                    break;
            }
        }

        private void InitializeBlackboard() {
            var bb = new Blackboard(_graphView);
            bb.Add(new BlackboardSection());
            bb.subTitle = "Input properties";

            bb.addItemRequested = _blackboard => {
                _graphView.AddBlackboardProperty(new InputProperty());
            };
            bb.editTextRequested = (_blackboad, element, newValue) => {
                if (_graphView.inputProperties.Any(i => i.propertyName == newValue)) {
                    EditorUtility.DisplayDialog("Error","An input property with that name already exists. Please choose a different one.", "OK");
                } else {
                    var oldValue = ((BlackboardField)element).text;
                    var index = _graphView.inputProperties.FindIndex(i => i.propertyName == oldValue);
                    _graphView.inputProperties[index].propertyName = newValue;
                    ((BlackboardField)element).text = newValue;
                }
            };

            bb.SetPosition(new Rect(10, 40, 200, 300));
            
            _graphView._blackboard = bb;
            //_graphView.Add(bb);
        }

        private void RequestSystemOperation(bool save)
        {
            if (string.IsNullOrEmpty(graphName))
            {
                EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name", "OK");
                return;
            }

            var saveUtility = SaveUtility.GetInstance(_graphView);
            if (save)
            {
                saveUtility.Save(graphName);
            }
            else
            {
                saveUtility.Load(graphName);
            }
        }
    }
}
