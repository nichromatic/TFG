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
            } else if (propertyType == PropertyType.String || propertyType == PropertyType.Number || propertyType == PropertyType.Color) {

                List<object> possibleValues = new List<object>(property.values);
                List<float> possibleValuesWeights = new List<float>(property.valueWeights);

                int repeatTimes = 1;
                if (property.multipleValues) repeatTimes = UnityEngine.Random.Range(property.minMultiple, property.maxMultiple+1);
                for (int i = 0; i < repeatTimes; i++) {
                    float weightSum = 0f;
                    for (int j = 0; j < possibleValuesWeights.Count; j++) {
                        weightSum += possibleValuesWeights[j];
                    }
                    /* var coinToss = UnityEngine.Random.Range(0, possibleValues.Count);
                    values.Add(possibleValues[coinToss]);
                    if (!property.repeatValues) possibleValues.RemoveAt(coinToss); */
                    var coinToss = UnityEngine.Random.Range(0f,weightSum);
                    //Debug.Log("Property chosen: " + coinToss);
                    int index = 0;
                    bool foundChosen = false;
                    float weightCarry = 0f;
                    do {
                        //Debug.Log("Property " + index + ": Accumulated weight " + (possibleValuesWeights[index] + weightCarry));
                        if ((possibleValuesWeights[index] + weightCarry) >= coinToss) {
                            foundChosen = true;
                        } else {
                            weightCarry += possibleValuesWeights[index];
                            index++;
                        }
                    } while (!foundChosen);
                    values.Add(possibleValues[index]);
                    if (!property.repeatValues) {
                        possibleValues.RemoveAt(index);
                        possibleValuesWeights.RemoveAt(index);
                    }
                }
            } /*else if (propertyType == PropertyType.Color) {
                List<object> possibleValues = new List<object>(property.values);
                int repeatTimes = 1;
                if (property.multipleValues) repeatTimes = UnityEngine.Random.Range(property.minMultiple, property.maxMultiple+1);
                for (int i = 0; i < repeatTimes; i++) {
                    var coinToss = UnityEngine.Random.Range(0, possibleValues.Count);
                    values.Add(possibleValues[coinToss]);
                    if (!property.repeatValues) possibleValues.RemoveAt(coinToss);
                }
            }*/
        }
        
        public int GetValueCount() {
            return values.Count;
        }

        public PropertyType GetValueType() {
            return propertyType;
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
                try {
                    castList.Add(float.Parse(val.ToString()));
                } catch {
                    Debug.Log("Property isn't a number property. Values can't be parsed to float.");
                    return null;
                }    
            }
            return castList;
        }

        public List<int> GetIntValues() {
            var castList = new List<int>();
            foreach (object val in values) {
                try {
                    castList.Add(Mathf.RoundToInt(float.Parse(val.ToString())));
                } catch {
                    Debug.Log("Property isn't a number property. Values can't be parsed to int.");
                    return null;
                }
            }
            return castList;
        }

        public List<Color> GetColorValues() {
            var castList = new List<Color>();
            foreach (object val in values) {
                Color c;
                bool canParse = ColorUtility.TryParseHtmlString(val.ToString(), out c);
                if (!canParse) {
                    Debug.LogError("Property isn't a color property. Values can't be parsed to Color.");
                    return null;
                }
                castList.Add(c);
            }
            return castList;
        }
        
        public Object GetValueAt(int index) {
            if (values.Count <= index) {
                Debug.LogError("Index " + index + " is out of bounds. Property only has " + values.Count + " values.");
                return null;
            } else {
                return (Object)values[index];
            }
        }
    }

}