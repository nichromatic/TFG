using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectModel;

namespace ProceduralGenerator
{
    public class ProceduralObjectProperty
    {
        public string propertyName;
        public PropertyType propertyType;
        public List<object> values = new List<object>();



        public ProceduralObjectProperty(PropertyData property) {
            
            propertyType = property.propertyType;
            values = new List<object>();
            propertyName = property.propertyName;

            if (propertyType == PropertyType.Boolean) {

                var value = bool.Parse(property.values[0]);
                var chance = property.valueWeights[0];
                if (chance == 100f) values.Add(value);
                else if (chance == 0f) values.Add(!value);
                else if (UnityEngine.Random.Range(0f, 100f) <= chance) values.Add(value);
                else values.Add(!value);

            } else if (propertyType == PropertyType.Range) {
                var min = float.Parse(property.values[0]);
                var max = float.Parse(property.values[1]);
                if (!property.roundValue) {
                    values.Add(UnityEngine.Random.Range(min, max));
                } else {
                    values.Add(Mathf.RoundToInt(UnityEngine.Random.Range(min, max)));
                }
            } else if (propertyType == PropertyType.String || propertyType == PropertyType.Number) {

                List<object> possibleValues = new List<object>(property.values);
                int repeatTimes = 1;
                if (property.multipleValues) repeatTimes = UnityEngine.Random.Range(property.minMultiple, property.maxMultiple+1);
                for (int i = 0; i < repeatTimes; i++) {
                    var coinToss = UnityEngine.Random.Range(0, possibleValues.Count);
                    values.Add(possibleValues[coinToss]);
                    if (!property.repeatValues) possibleValues.RemoveAt(coinToss);
                }
            }
        }
        public int GetValueCount() {
            return values.Count;
        }
        public List<string> GetStringValues() {
            var castList = new List<string>();
            foreach (object val in values) {
                castList.Add(val.ToString());
            }
            return castList;
        }
        public List<float> GetFloatValues() {
            var castList = new List<float>();
            foreach (object val in values) {
                castList.Add(float.Parse(val.ToString()));
            }
            return castList;
        }
    }

}