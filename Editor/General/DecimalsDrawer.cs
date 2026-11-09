using System;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Rounds a float to <see cref="DecimalsAttribute.Decimals" /> decimals.
    ///     Uses rect-based EditorGUI only (EditorGUILayout is not supported inside PropertyDrawers).
    /// </summary>
    [CustomPropertyDrawer(typeof(DecimalsAttribute))]
    public class DecimalsDrawer : PropertyDrawer
    {
        private static float Line => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.Float ? Line * 2f : Line;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var decimals = ((DecimalsAttribute) attribute).Decimals;

            if (property.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.LabelField(position, label.text, "Use Decimals with float.");

                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var row = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(row, "Round to: " + decimals, EditorStyles.miniLabel);
            row.y += Line;

            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.FloatField(row, label, Round(property.floatValue, decimals));

            if (EditorGUI.EndChangeCheck())
            {
                property.floatValue = Round(value, decimals);
            }

            EditorGUI.EndProperty();
        }


        /// <summary>
        ///     Double-based rounding: unlike a (decimal) cast it cannot overflow and passes NaN/Infinity through.
        /// </summary>
        private static float Round(float value, int decimals)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return value;
            }

            return (float) Math.Round(value, Mathf.Clamp(decimals, 0, 15), MidpointRounding.AwayFromZero);
        }
    }
}
