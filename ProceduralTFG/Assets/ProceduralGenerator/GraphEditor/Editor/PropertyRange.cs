using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ObjectModel
{
    [Serializable]
    public class PropertyRange : Property
    {
        public List<float> values = new List<float>();
        private const float defaultMin = 0f;
        private const string defaultName = "Range property";
        private const float defaultMax = 10f;

        private const bool defaultRound = false;

        public PropertyRange(string name, GraphProperty element) : base(name, PropertyType.Range, element) { }

        public PropertyRange(string name, string data, GraphProperty element, bool loadValues = true) : base(data, element) {
            PropertyRange p = UnityEngine.JsonUtility.FromJson<PropertyRange>(data);
            if (loadValues) {
                values = p.values;
                roundValue = p.roundValue;
            } else {
                values = new List<float>(new float[]{defaultMin, defaultMax});
                roundValue = defaultRound;
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
            var varRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/RangePropertyRow");
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

            Toggle roundToInt = varRow.Query<Toggle>("roundValue").ToList()[0];
            roundToInt.SetValueWithoutNotify(roundValue);
            roundToInt.MarkDirtyRepaint();
            roundToInt.RegisterValueChangedCallback(evt => {
                roundValue = evt.newValue;
            });

            if (values == null || values.Count < 2) values = new List<float>(new float[]{defaultMin, defaultMax});
            FloatField minField = varRow.Query<FloatField>("propertyValueMin").ToList()[0];
            minField.SetValueWithoutNotify(values[0]);
            minField.MarkDirtyRepaint();
            minField.RegisterValueChangedCallback(evt => values[0] = evt.newValue);

            FloatField maxField = varRow.Query<FloatField>("propertyValueMax").ToList()[0];
            maxField.SetValueWithoutNotify(values[1]);
            maxField.MarkDirtyRepaint();
            maxField.RegisterValueChangedCallback(evt => values[1] = evt.newValue);
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

        /* private void AddValueRow(Object value = null, float weight = defaultWeight, int index = 0)
        {
            if (value == null)
            {
                index = values.Count;
                value = defaultValue;
            }

            values.Add((float)value);
            //valueWeights.Add(weight);

            var valueRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/NumberValueRow");
            VisualElement valueRow = valueRowTemplate.CloneTree();

             FloatField valueField = valueRow.Query<FloatField>("propertyValue").ToList()[0];
            valueField.SetValueWithoutNotify((float)value);
            valueField.MarkDirtyRepaint();
            valueField.RegisterValueChangedCallback(evt => values[index] = evt.newValue);

            FloatField weightField = valueRow.Query<FloatField>("valueWeight").ToList()[0];
            weightField.SetValueWithoutNotify(weight);
            weightField.MarkDirtyRepaint();
            weightField.RegisterValueChangedCallback(evt => valueWeights[index] = evt.newValue);

            Button deleteBtn = valueRow.Query<Button>("deleteValueBtn").ToList()[0];
            deleteBtn.clickable.clicked += () =>
            {
                valueContainer.Remove(valueRow);
            };

            valueContainer.Add(valueRow);
        } */

        /* public override void InitializeRowValues()
        {
            int i = 0;
            var tempVal = values;
            var tempWeights = valueWeights;
            values = new List<float>();
            valueWeights = new List<float>();
            tempVal.ForEach(v =>
            {
                AddValueRow(v, tempWeights[i], i);
                i++;
            });
        } */

        public override string GetAsJSON() {
            return (UnityEngine.JsonUtility.ToJson(this));
        }

    }
}
