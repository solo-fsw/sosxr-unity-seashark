using System;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    [CustomPropertyDrawer(typeof(DecimalsAttribute))]
    public class DecimalsDrawer : PropertyDrawer
    {
        private readonly GUIStyle _style = GUI.skin.box;
        private DecimalsAttribute _attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _attribute = attribute as DecimalsAttribute;

            if (_attribute == null)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var labelText = new StringBuilder("Round to: " + _attribute.Decimals).ToString();
            EditorGUILayout.LabelField(labelText, _style);

            if (property.propertyType == SerializedPropertyType.Float)
            {
                var roundedValue = (float) Math.Round((decimal) property.floatValue, _attribute.Decimals, MidpointRounding.AwayFromZero);
                property.floatValue = EditorGUILayout.FloatField(new GUIContent(property.name), roundedValue);
            }
            else if (property.propertyType is SerializedPropertyType.Vector2 or SerializedPropertyType.Vector3)
            {
                EditorGUI.LabelField(position, label.text, "Currently Decimals only works with floats. Vector2 / Vector3 is yet to be implemented.");
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use Decimals with float.");
            }

            EditorGUILayout.EndVertical();

            var editorGUI = new SerializedObject(Selection.activeObject);

            editorGUI.ApplyModifiedProperties();
        }
    }
}