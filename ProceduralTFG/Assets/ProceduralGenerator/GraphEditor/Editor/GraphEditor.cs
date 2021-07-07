using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

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
                            _graphView.ClearGraph();
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
