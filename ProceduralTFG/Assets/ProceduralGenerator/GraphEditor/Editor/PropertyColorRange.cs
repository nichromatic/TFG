using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ObjectModel
{
    [Serializable]
    public class PropertyColorRange : Property
    {
        public List<float> values = new List<float>();
        private const float defaultMin = 0f;
        private const float defaultMax = 1f;
        private const string defaultName = "Range property";

        public PropertyColorRange(string name, GraphProperty element) : base(name, PropertyType.Range, element) { }

        public PropertyColorRange(string name, string data, GraphProperty element, bool loadValues = true) : base(data, element) {
            PropertyColorRange p = UnityEngine.JsonUtility.FromJson<PropertyColorRange>(data);
            if (loadValues) {
                values = p.values;
            } else {
                values = new List<float>(new float[]{defaultMin, defaultMax, defaultMin, defaultMax, defaultMin, defaultMax, defaultMin, defaultMax});
            }
        }

        public override List<Object> GetValues()
        {
            List<Object> castValues = new List<Object>();
            values.ForEach(value => castValues.Add(value));
            return castValues;
        }

        public override void SetValues(string JSONValues, List<float> newWeights)
        {
            string[] newValues = UnityEngine.JsonUtility.FromJson<string[]>(JSONValues);
            List<string> newValuesList = new List<string>(newValues);
            newValuesList.ForEach(value =>
            {
                values.Add(float.Parse(value));
            });
            /* newWeights.ForEach(value =>
            {
                valueWeights.Add(value);
            }); */
        }

        public override void InitializeRow(VisualElement parent)
        {
            parentElement = parent;
            var varRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/ColorRangePropertyRow");
            varRow = varRowTemplate.CloneTree();
            parentElement.Add(varRow);

            valueContainer = varRow.Query<VisualElement>("valueRowContainer");
            var rowButtons = varRow.Query<Button>();
            rowButtons.ForEach(button => InitializeRowButton(button));

            Label propertyLabel = varRow.Query<Label>("propertyLabel").ToList()[0];
            propertyLabel.text = propertyName;

            TextField nameField = varRow.Query<TextField>("propertyName").ToList()[0];
            if (propertyName != null) nameField.SetValueWithoutNotify(propertyName);
            else nameField.SetValueWithoutNotify(defaultName);
            nameField.MarkDirtyRepaint();
            nameField.RegisterValueChangedCallback(evt => {
                propertyName = evt.newValue;
                propertyLabel.text = evt.newValue;
            });

            EnumField typeField = varRow.Query<EnumField>("propertyType").ToList()[0];
            typeField.SetValueWithoutNotify(propertyType);
            typeField.MarkDirtyRepaint();
            typeField.RegisterValueChangedCallback(evt => {
                parentElement.Remove(varRow);
                this.propertyType = (PropertyType)evt.newValue;
                graphElement.InitializeRow(parentElement, (PropertyType)evt.newValue, true, this.GetAsJSON(), false);
                //graphElement.DeleteProperty();
            });

            if (values == null || values.Count < 2) values = new List<float>(new float[]{defaultMin, defaultMax, defaultMin, defaultMax, defaultMin, defaultMax, defaultMin, defaultMax});

            // HUE 
            FloatField hueFrom = varRow.Query<FloatField>("hueFrom").ToList()[0];
            InitializeField(hueFrom, 0);
            FloatField hueTo = varRow.Query<FloatField>("hueTo").ToList()[0];
            InitializeField(hueTo, 1);

            // SATURATION
            FloatField saturationFrom = varRow.Query<FloatField>("saturationFrom").ToList()[0];
            InitializeField(saturationFrom, 2);
            FloatField saturationTo = varRow.Query<FloatField>("saturationTo").ToList()[0];
            InitializeField(saturationTo, 3);

            // LIGHT 
            FloatField lightFrom = varRow.Query<FloatField>("lightFrom").ToList()[0];
            InitializeField(lightFrom, 4);
            FloatField lightTo = varRow.Query<FloatField>("lightTo").ToList()[0];
            InitializeField(lightTo, 5);
            
            // Alpha 
            FloatField alphaFrom = varRow.Query<FloatField>("alphaFrom").ToList()[0];
            InitializeField(alphaFrom, 6);
            FloatField alphaTo = varRow.Query<FloatField>("alphaTo").ToList()[0];
            InitializeField(alphaTo, 7);
        }

        private void InitializeField(FloatField field, int index) {
            field.SetValueWithoutNotify(values[index]);
            field.MarkDirtyRepaint();
            field.RegisterValueChangedCallback(evt => {
                if (evt.newValue > 1f) {
                    field.value = 1f;
                    values[index] = 1f;
                } else if (evt.newValue < 0f) {
                    field.value = 0f;
                    values[index] = 1f;
                }
                values[index] = evt.newValue;
            });
        }

        public override void InitializeRowButton(Button button)
        {
            switch (button.name)
            {
                case "deletePropertyBtn":
                    button.clickable.clicked += () =>
                    {
                        parentElement.Remove(varRow);
                        graphElement.DeleteProperty();
                    };
                    break;
                /* case "addValueBtn":
                    button.clickable.clicked += () =>
                    {
                        AddValueRow();
                    };
                    break; */
            }
        }

        public override string GetAsJSON() {
            return (UnityEngine.JsonUtility.ToJson(this));
        }

    }
}
