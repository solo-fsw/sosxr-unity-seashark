using System;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     From: https://docs.unity3d.com/ScriptReference/PropertyDrawer.html
    ///     Uses rect-based EditorGUI only (EditorGUILayout is not supported inside PropertyDrawers) and
    ///     reports its height through GetPropertyHeight.
    /// </summary>
    [CustomPropertyDrawer(typeof(BoxRangeAttribute))]
    public class BoxRangeDrawer : PropertyDrawer
    {
        private const float Padding = 4f;

        private BoxRangeAttribute Attribute => (BoxRangeAttribute) attribute;

        private static float Line => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


        private static int SliderCount(SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.Integer => 1,
                SerializedPropertyType.Float => 1,
                SerializedPropertyType.Vector2 => 2,
                SerializedPropertyType.Vector3 => 3,
                SerializedPropertyType.Vector3Int => 3,
                _ => 1 // Room for the "unsupported type" message
            };
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (1 + SliderCount(property)) * Line + Padding * 2f;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousLabelWidth = EditorGUIUtility.labelWidth;

            EditorGUI.BeginProperty(position, label, property);
            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            var row = new Rect(position.x + Padding, position.y + Padding, position.width - Padding * 2f, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(row, $"{label.text} - Range: [{Attribute.Min}, {Attribute.Max}]", EditorStyles.boldLabel);
            row.y += Line;

            EditorGUIUtility.labelWidth = 20f;

            // Only write on a real edit: writing every frame would copy one object's value to all selected objects.
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                {
                    var value = IntSlider(ref row, "", property.intValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.intValue = value;
                    }

                    break;
                }
                case SerializedPropertyType.Float:
                {
                    var value = Slider(ref row, "", property.floatValue);

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.floatValue = value;
                    }

                    break;
                }
                case SerializedPropertyType.Vector2:
                {
                    var v = property.vector2Value;
                    v.x = Slider(ref row, "X", v.x);
                    v.y = Slider(ref row, "Y", v.y);

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector2Value = v;
                    }

                    break;
                }
                case SerializedPropertyType.Vector3:
                {
                    var v = property.vector3Value;
                    v.x = Slider(ref row, "X", v.x);
                    v.y = Slider(ref row, "Y", v.y);
                    v.z = Slider(ref row, "Z", v.z);

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector3Value = v;
                    }

                    break;
                }
                case SerializedPropertyType.Vector3Int:
                {
                    var v = property.vector3IntValue;
                    v.x = IntSlider(ref row, "X", v.x);
                    v.y = IntSlider(ref row, "Y", v.y);
                    v.z = IntSlider(ref row, "Z", v.z);

                    if (EditorGUI.EndChangeCheck())
                    {
                        property.vector3IntValue = v;
                    }

                    break;
                }
                default:
                    EditorGUI.EndChangeCheck();
                    EditorGUI.LabelField(row, "Use BoxRange with int, float, Vector2, Vector3, or Vector3Int.");

                    break;
            }

            EditorGUI.showMixedValue = false;
            EditorGUIUtility.labelWidth = previousLabelWidth;
            EditorGUI.EndProperty();
        }


        private float Slider(ref Rect row, string name, float value)
        {
            var result = EditorGUI.Slider(row, new GUIContent(name), value, Attribute.Min, Attribute.Max);
            row.y += Line;

            return result;
        }


        private int IntSlider(ref Rect row, string name, int value)
        {
            var result = EditorGUI.IntSlider(row, new GUIContent(name), value, Convert.ToInt32(Attribute.Min), Convert.ToInt32(Attribute.Max));
            row.y += Line;

            return result;
        }
    }
}
