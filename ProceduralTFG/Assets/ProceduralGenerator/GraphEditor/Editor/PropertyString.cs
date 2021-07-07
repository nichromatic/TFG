using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ObjectModel
{
    [Serializable]
    public class PropertyString : Property
    {
        public List<string> values = new List<string>();
        public List<float> valueWeights = new List<float>();

        private const string defaultValue = "String";
        private const string defaultName = "String property";
        private const float defaultWeight = 1f;

        public PropertyString(string name, GraphProperty element) : base(name, PropertyType.String, element) { }

        public PropertyString(PropertyData data, GraphProperty element) : base(data, element) { }

        public override List<Object> GetValues()
        {
            List<Object> castValues = new List<Object>();
            values.ForEach(value => castValues.Add(value));
            return castValues;
        }

        public override List<float> GetWeights()
        {
            return valueWeights;
        }

        public override void SetValues(List<Object> newValues, List<float> newWeights)
        {
            newValues.ForEach(value =>
            {
                values.Add((string)value);
            });
            newWeights.ForEach(value =>
            {
                valueWeights.Add(value);
            });
        }

        public override void InitializeRow(VisualElement parent, List<Object> newValues, List<float> newWeights)
        {
            parentElement = parent;
            var varRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/StringPropertyRow");
            varRow = varRowTemplate.CloneTree();
            parentElement.Add(varRow);

            valueContainer = varRow.Query<VisualElement>("valueRowContainer");
            var rowButtons = varRow.Query<Button>();
            rowButtons.ForEach(button => InitializeRowButton(button));

            TextField nameField = varRow.Query<TextField>("propertyName").ToList()[0];
            if (propertyName != null) nameField.SetValueWithoutNotify(propertyName);
            else nameField.SetValueWithoutNotify(defaultName);
            nameField.MarkDirtyRepaint();
            nameField.RegisterValueChangedCallback(evt => propertyName = evt.newValue);


            var multipleValueConfig = varRow.Query<VisualElement>("multipleValueConfig");
            if (multipleValues) multipleValueConfig.ForEach(div => div.RemoveFromClassList("display-none"));

            Toggle multipleValuesCheck = varRow.Query<Toggle>("multipleValues").ToList()[0];
            multipleValuesCheck.SetValueWithoutNotify(multipleValues);
            multipleValuesCheck.MarkDirtyRepaint();
            multipleValuesCheck.RegisterValueChangedCallback(evt => {
                if (evt.newValue)
                {
                    multipleValueConfig.ForEach(div => div.RemoveFromClassList("display-none"));
                }
                else
                {
                    multipleValueConfig.ForEach(div => div.AddToClassList("display-none"));
                }
                multipleValues = evt.newValue;
            });
            Toggle repeatValuesCheck = varRow.Query<Toggle>("repeatValues").ToList()[0];
            repeatValuesCheck.SetValueWithoutNotify(repeatValues);
            repeatValuesCheck.MarkDirtyRepaint();
            repeatValuesCheck.RegisterValueChangedCallback(evt => {
                repeatValues = evt.newValue;
            });

            IntegerField minField = varRow.Query<IntegerField>("multipleMin").ToList()[0];
            minField.SetValueWithoutNotify(minMultiple);
            minField.MarkDirtyRepaint();
            minField.RegisterValueChangedCallback(evt => {
                minMultiple = evt.newValue;
            });
            IntegerField maxField = varRow.Query<IntegerField>("multipleMax").ToList()[0];
            maxField.SetValueWithoutNotify(maxMultiple);
            maxField.MarkDirtyRepaint();
            maxField.RegisterValueChangedCallback(evt => {
                maxMultiple = evt.newValue;
            });

            InitializeRowValues(newValues, newWeights);
        }

        public override void InitializeRowButton(Button button)
        {
            switch (button.name)
            {
                case "deletePropertyBtn":
                    //parentElement.Remove(varRow);
                    //graphElement.DeleteProperty();
                    break;
                case "addValueBtn":
                    button.clickable.clicked += () =>
                    {
                        AddValueRow();
                    };
                    break;
            }
        }

        private void AddValueRow(Object value = null, float weight = defaultWeight, int index = 0)
        {
            if (value == null)
            {
                index = values.Count;
                value = defaultValue;
            }

            values.Add((string)value);
            valueWeights.Add(weight);

            var valueRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/StringValueRow");
            VisualElement valueRow = valueRowTemplate.CloneTree();

            TextField valueField = valueRow.Query<TextField>("propertyValue").ToList()[0];
            valueField.SetValueWithoutNotify((string)value);
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
        }

        public override void InitializeRowValues(List<Object> newValues = null, List<float> newWeights = null)
        {
            values = new List<string>();
            valueWeights = new List<float>();
            int i = 0;
            newValues.ForEach(v =>
            {
                AddValueRow(v, newWeights[i], i);
                i++;
            });
        }
    }
}
