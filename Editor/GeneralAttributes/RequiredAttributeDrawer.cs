using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor
{
    /// <summary>
    ///     From Warped Imagination: https://www.youtube.com/watch?v=BN6NXMHJ8v0
    /// </summary>
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsFieldEmpty(property))
            {
                var height = EditorGUIUtility.singleLineHeight * 2f;
                height += base.GetPropertyHeight(property, label);

                return height;
            }

            return base.GetPropertyHeight(property, label);
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
                position.height = EditorGUIUtility.singleLineHeight * 2;
                position.height += base.GetPropertyHeight(property, label);

                EditorGUI.HelpBox(position, "Required", MessageType.Error);
                EditorGUI.DrawRect(position, new Color(1f, 0f, 0f, 0.1f));

                position.height = base.GetPropertyHeight(property, label);
                position.y += EditorGUIUtility.singleLineHeight * 2;
            }

            EditorGUI.PropertyField(position, property, label);
        }


        private bool IsFieldEmpty(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue == null;

                case SerializedPropertyType.String:
                    return string.IsNullOrEmpty(property.stringValue);

                case SerializedPropertyType.Enum:
                    return property.enumValueIndex == 0; // Assuming index 0 is "None" or equivalent

                case SerializedPropertyType.ArraySize:
                    return property.arraySize == 0;

                case SerializedPropertyType.Color:
                    return property.colorValue == Color.clear;


                default:
                    return false;
            }
        }


        private bool IsFieldSupported(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                case SerializedPropertyType.String:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Color:
                    return true;

                default:
                    return false;
            }
        }
    }
}