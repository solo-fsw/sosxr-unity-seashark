using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(PostfixAttribute))]
    public class PostfixDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var postfixAttribute = (PostfixAttribute) attribute;

            var postfixStyle = new GUIStyle(EditorStyles.label)
            {
                normal = {textColor = Color.grey},
                alignment = TextAnchor.MiddleLeft,
                fontSize = EditorStyles.label.fontSize - 2,
                fontStyle = FontStyle.Italic
            };

            var postfixWidth = postfixStyle.CalcSize(new GUIContent(postfixAttribute.Postfix)).x + 4f;

            var fieldRect = new Rect(position.x, position.y, position.width - postfixWidth - 2f, position.height);
            var postfixRect = new Rect(position.xMax - postfixWidth, position.y, postfixWidth, position.height);

            // Check for [Range] attribute via reflection
            var rangeAttribute = fieldInfo.GetCustomAttribute<RangeAttribute>();

            if (rangeAttribute != null && property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUI.Slider(fieldRect, property, rangeAttribute.min, rangeAttribute.max, label);
            }
            else if (rangeAttribute != null && property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUI.IntSlider(fieldRect, property, (int) rangeAttribute.min, (int) rangeAttribute.max, label);
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, property, label, true);
            }

            EditorGUI.LabelField(postfixRect, postfixAttribute.Postfix, postfixStyle);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}