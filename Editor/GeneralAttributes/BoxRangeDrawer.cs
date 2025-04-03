using System;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor.PropertyDrawers
{
    /// <summary>
    ///     From: https://docs.unity3d.com/ScriptReference/PropertyDrawer.html
    /// </summary>
    [CustomPropertyDrawer(typeof(BoxRangeAttribute))]
    public class BoxRangeDrawer : PropertyDrawer
    {
        private readonly GUIStyle _style = GUI.skin.box;
        private BoxRangeAttribute _attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = attribute as BoxRangeAttribute;

            if (_attribute == null)
            {
                return;
            }

            EditorGUIUtility.labelWidth = 15;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var newLabel = new StringBuilder(label + " - " + "Range: [" + _attribute.Min + ", " + _attribute.Max + "]").ToString();
            EditorGUILayout.LabelField(newLabel, _style);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = CreateIntSlider("", property.intValue);
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = CreateSlider("", property.floatValue);
            }
            else if (property.propertyType == SerializedPropertyType.Vector2)
            {
                CreateVector2Sliders(property);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3)
            {
                CreateVector3Sliders(property);
            }
            else if (property.propertyType == SerializedPropertyType.Vector3Int)
            {
                CreateVector3IntSliders(property);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use BoxRange with int, float, Vector2, Vector3, or Vector3Int.");
            }

            EditorGUILayout.EndVertical();

            var editorGUI = new SerializedObject(Selection.activeObject);

            editorGUI.ApplyModifiedProperties();
        }


        private void CreateVector2Sliders(SerializedProperty property)
        {
            var temp = property.vector2Value;
            temp = new Vector2(CreateSlider(nameof(temp.x), temp.x), temp.y);
            temp = new Vector2(temp.x, CreateSlider(nameof(temp.y), temp.y));
            property.vector2Value = temp;
        }


        private void CreateVector3Sliders(SerializedProperty property)
        {
            var temp = property.vector3Value;
            temp = new Vector3(CreateSlider(nameof(temp.x), temp.x), temp.y, temp.z);
            temp = new Vector3(temp.x, CreateSlider(nameof(temp.y), temp.y), temp.z);
            temp = new Vector3(temp.x, temp.y, CreateSlider(nameof(temp.z), temp.z));
            property.vector3Value = temp;
        }


        private void CreateVector3IntSliders(SerializedProperty property)
        {
            var temp = property.vector3IntValue;
            temp = new Vector3Int(CreateIntSlider(nameof(temp.x), temp.x), temp.y, temp.z);
            temp = new Vector3Int(temp.x, CreateIntSlider(nameof(temp.y), temp.y), temp.z);
            temp = new Vector3Int(temp.x, temp.y, CreateIntSlider(nameof(temp.z), temp.z));
            property.vector3IntValue = temp;
        }


        private float CreateSlider(string name, float value)
        {
            return EditorGUILayout.Slider(new GUIContent(name.ToUpper()), value, _attribute.Min, _attribute.Max);
        }


        private int CreateIntSlider(string name, float value)
        {
            var intValue = Convert.ToInt32(value);
            var intRange = new Vector2Int(Convert.ToInt32(_attribute.Min), Convert.ToInt32(_attribute.Max));

            return EditorGUILayout.IntSlider(new GUIContent(name.ToUpper()), intValue, intRange.x, intRange.y);
        }
    }
}