using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class ObjectModelGraphEditor : GraphViewEditorWindow
{
    private ObjectModelGraphView _graphView;
    private string graphName = "New Object Model"; // Default name for saving the graph

    [UnityEditor.MenuItem("Procedural Generator/Object Model Graph")]
    public static void OpenGraphWindow()
    {
        var window = GetWindow<ObjectModelGraphEditor>();
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
        _graphView = new ObjectModelGraphView
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

        /*var toolbar = new Toolbar();
        var createNodeBtn = new Button(() =>
        {
            _graphView.AddElement(_graphView.CreateNode("Node", _graphView.defaultNodePos)); 
        });
        createNodeBtn.text = "Create child node";
        toolbar.Add(createNodeBtn);

        var clearGraphBtn = new Button(() =>
        {
            if (EditorUtility.DisplayDialog("Clear graph", "Are you sure you want to delete the graph? All data not saved to an asset will be lost.", "Clear graph", "Cancel"))
                _graphView.ClearGraph();
        });
        clearGraphBtn.text = "Clear graph";
        toolbar.Add(clearGraphBtn);

        var graphNameField = new TextField("Graph name:");
        graphNameField.SetValueWithoutNotify(graphName);
        graphNameField.MarkDirtyRepaint();
        graphNameField.RegisterValueChangedCallback(evt => graphName = evt.newValue);
        toolbar.Add(graphNameField);

        var saveGraph = new Button(() =>
        {
            RequestSystemOperation(true);
        });
        saveGraph.text = "Save Model";
        toolbar.Add(saveGraph);

        var loadGraph = new Button(() =>
        {
            RequestSystemOperation(false);
        });
        loadGraph.text = "Load Model";
        toolbar.Add(loadGraph);

        //rootVisualElement.Add(toolbar);*/
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

        var saveUtility = ObjectModelSaveUtility.GetInstance(_graphView);
        if (save)
        {
            saveUtility.Save(graphName);
        } else
        {
            saveUtility.Load(graphName);
        }
    }


}
