using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ObjectModel
{
    [Serializable]
    public class PropertyNumber : Property
    {
        private List<string> values = new List<string>();
        private List<float> valueWeights = new List<float>();

        private readonly string defaultValue = "String";
        private readonly float defaultWeight = 1f;

        public PropertyNumber(string name, GraphProperty element) : base(name, PropertyType.String, element) { }

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
            var varRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/StringPropertyRow");
            VisualElement varRow = varRowTemplate.CloneTree();
            parent.Add(varRow);

            valueContainer = parent.Query<VisualElement>("valueRowContainer");
            var rowButtons = parent.Query<Button>();
            rowButtons.ForEach(button => InitializeRowButton(button));

            int i = 0;
            newValues.ForEach(value =>
            {
                values.Add((string)value);
                valueWeights.Add(newWeights[i]);
                var valueRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/StringValueRow");
                VisualElement valueRow = valueRowTemplate.CloneTree();

                TextField valueField = valueRow.Query<TextField>("propertyValue").ToList()[0];
                valueField.SetValueWithoutNotify((string)value);
                valueField.MarkDirtyRepaint();
                valueField.RegisterValueChangedCallback(evt => values[i] = evt.newValue);

                FloatField weightField = valueRow.Query<FloatField>("propertyWeight").ToList()[0];
                weightField.SetValueWithoutNotify(newWeights[i]);
                weightField.MarkDirtyRepaint();
                weightField.RegisterValueChangedCallback(evt => valueWeights[i] = evt.newValue);

                valueContainer.Add(valueRow);
            });
        }

        public override void InitializeRowButton(Button button)
        {
            switch (button.name)
            {
                case "deletePropertyBtn":
                    graphElement.DeleteProperty();
                    break;
                case "addValueBtn":
                    button.clickable.clicked += () =>
                    {
                        var valueRowTemplate = UnityEngine.Resources.Load<VisualTreeAsset>("PropertyRows/StringValueRow");
                        VisualElement valueRow = valueRowTemplate.CloneTree();

                        values.Add(defaultValue);
                        valueWeights.Add(defaultWeight);
                        int i = values.Count;

                        TextField valueField = valueRow.Query<TextField>("propertyValue").ToList()[0];
                        valueField.SetValueWithoutNotify(defaultValue);
                        valueField.MarkDirtyRepaint();
                        valueField.RegisterValueChangedCallback(evt => values[i] = evt.newValue);

                        FloatField weightField = valueRow.Query<FloatField>("propertyWeight").ToList()[0];
                        weightField.SetValueWithoutNotify(defaultWeight);
                        weightField.MarkDirtyRepaint();
                        weightField.RegisterValueChangedCallback(evt => valueWeights[i] = evt.newValue);

                        valueContainer.Add(valueRow);
                    };
                    break;
            }
        }
    }
}

