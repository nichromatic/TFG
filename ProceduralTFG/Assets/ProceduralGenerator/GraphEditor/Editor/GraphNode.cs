using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ObjectModel
{
    public class GraphNode : BaseNode
    {
        public bool rootNode = false;
        public string nodeName;
        //public List<Property> nodeProperties = new List<Property>();
        public List<GraphProperty> nodePropertyRows = new List<GraphProperty>();
        public VisualElement nodePropertiesContainer;
        public VisualElement nodePropertiesFoldout;
        public Texture2D nodeSprite;
        public VisualElement nodeSpriteContainer;
        public List<string> nodeSpriteModifiers = new List<string>(new string[] {"","","","",""});

        private int nextPortID = -1;

        public string GetNextPortID()
        {
            nextPortID++;
            return nextPortID.ToString();
        }

        public List<string> ExportPropertyData()
        {
            List<string> data = new List<string>();
            nodePropertyRows.ForEach(p => data.Add(p.ExportPropertyData()));
            return data;
        }

        public void InitializeNameField() {
            var textField = new TextField()
            {
                name = string.Empty,
                value = nodeName
            };
            textField.RegisterValueChangedCallback(e =>
            {
                this.nodeName = e.newValue;
            });
            textField.AddToClassList("titleTextField");
            this.titleContainer.Remove(this.titleContainer.Q<Label>("title-label"));
            this.titleContainer.Insert(0, textField);
        }

        public void InitializeSpriteField(Foldout parent, Texture2D loadSprite = null) {
            this.nodeSprite = loadSprite;
            this.nodeSpriteContainer = new VisualElement();
            nodeSpriteContainer.AddToClassList("col");
            nodeSpriteContainer.AddToClassList("spriteContainer");

            var spritePreview = new Image();
            spritePreview.image = this.nodeSprite;
            spritePreview.scaleMode = ScaleMode.ScaleToFit;
            spritePreview.AddToClassList("spritePreview");
            
            var spriteField =  new ObjectField {
                name = string.Empty,
                objectType = typeof(Texture2D),
                value = this.nodeSprite
            };
            spriteField.RegisterValueChangedCallback(e =>
            {
                this.nodeSprite = (Texture2D)e.newValue;
                spritePreview.image = this.nodeSprite;
            });
            spriteField.AddToClassList("spriteField");
            
            var deleteSpriteBtn = new Button(() =>
            {
                this.nodeSprite = null;
                spriteField.value = null;
                spriteField.MarkDirtyRepaint();
            });
            deleteSpriteBtn.text = "X";
            deleteSpriteBtn.AddToClassList("deleteButton");

            var spriteSelectRow = new VisualElement();
            spriteSelectRow.AddToClassList("row");
            spriteSelectRow.AddToClassList("sprite-select");
            spriteSelectRow.Add(spriteField);
            spriteSelectRow.Add(deleteSpriteBtn);

            var modifiers = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/SpriteModifiersRow");
            VisualElement modifiersRow = modifiers.CloneTree();

            var colorTint = modifiersRow.Q<TextField>("colorTint");
            colorTint.value = this.nodeSpriteModifiers[0];
            colorTint.RegisterValueChangedCallback(e =>
            {
                this.nodeSpriteModifiers[0] = e.newValue;
            });
            var xscale = modifiersRow.Q<TextField>("xscale");
            xscale.value = this.nodeSpriteModifiers[1];
            xscale.RegisterValueChangedCallback(e =>
            {
                this.nodeSpriteModifiers[1] = e.newValue;
            });
            var yscale = modifiersRow.Q<TextField>("yscale");
            yscale.value = this.nodeSpriteModifiers[2];
            yscale.RegisterValueChangedCallback(e =>
            {
                this.nodeSpriteModifiers[2] = e.newValue;
            });
            var xoffset = modifiersRow.Q<TextField>("xoffset");
            xoffset.value = this.nodeSpriteModifiers[3];
            xoffset.RegisterValueChangedCallback(e =>
            {
                this.nodeSpriteModifiers[3] = e.newValue;
            });
            var yoffset = modifiersRow.Q<TextField>("yoffset");
            yoffset.value = this.nodeSpriteModifiers[4];
            yoffset.RegisterValueChangedCallback(e =>
            {
                this.nodeSpriteModifiers[4] = e.newValue;
            });


            nodeSpriteContainer.Add(spritePreview);
            nodeSpriteContainer.Add(spriteSelectRow);
            nodeSpriteContainer.Add(modifiersRow);
            parent.Add(nodeSpriteContainer);
        }
    }
}
