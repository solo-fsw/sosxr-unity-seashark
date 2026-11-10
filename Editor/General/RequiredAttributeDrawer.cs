using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     From Warped Imagination: https://www.youtube.com/watch?v=BN6NXMHJ8v0
    /// </summary>
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeDrawer : PropertyDrawer
    {
        private static float WarningHeight => EditorGUIUtility.singleLineHeight * 2f;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUI.GetPropertyHeight(property, label, true);

            return IsFieldSupported(property) && IsFieldEmpty(property) ? height + WarningHeight : height;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsFieldSupported(property))
            {
                EditorGUI.HelpBox(position, $"Setting a 'Required' attribute on field type {property.propertyType.ToString().ToUpper()} is not supported", MessageType.Error);

                return;
            }

            if (IsFieldEmpty(property))
            {
                var warningRect = new Rect(position.x, position.y, position.width, WarningHeight);
                EditorGUI.HelpBox(warningRect, "Required", MessageType.Error);
                EditorGUI.DrawRect(position, new Color(1f, 0f, 0f, 0.1f));

                position.y += WarningHeight;
                position.height -= WarningHeight;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }


        /// <summary>
        ///     True when the field has no value. A List or array counts as empty when it has no elements.
        /// </summary>
        internal static bool IsFieldEmpty(SerializedProperty property)
        {
            if (IsCollection(property))
            {
                return property.arraySize == 0;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue == null;

                case SerializedPropertyType.String:
                    return string.IsNullOrEmpty(property.stringValue);

                case SerializedPropertyType.Enum:
                    return property.enumValueIndex == 0; // Assuming index 0 is "None" or equivalent

                case SerializedPropertyType.Color:
                    return property.colorValue == Color.clear;


                default:
                    return false;
            }
        }


        internal static bool IsFieldSupported(SerializedProperty property)
        {
            if (IsCollection(property))
            {
                return true;
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.Color:
                    return true;

                default:
                    return false;
            }
        }


        // A string is also an "array" to SerializedProperty: only treat real Lists and arrays as collections.
        private static bool IsCollection(SerializedProperty property)
        {
            return property.isArray && property.propertyType != SerializedPropertyType.String;
        }
    }
}