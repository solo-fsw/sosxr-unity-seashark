#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(EnumLabelAttribute))]
    public class EnumLabelDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, string> customEnumNames = new();

        private EnumLabelAttribute enumLabelAttribute => (EnumLabelAttribute) attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SetUpCustomEnumNames(property, property.enumNames);

            if (property.propertyType == SerializedPropertyType.Enum)
            {
                EditorGUI.BeginChangeCheck();

                var displayedOptions = property.enumNames
                                               .Where(enumName => customEnumNames.ContainsKey(enumName))
                                               .Select(enumName => customEnumNames[enumName])
                                               .ToArray();

                var selectedIndex = EditorGUI.Popup(position, enumLabelAttribute.label, property.enumValueIndex, displayedOptions);

                if (EditorGUI.EndChangeCheck())
                {
                    property.enumValueIndex = selectedIndex;
                }
            }
        }


        public void SetUpCustomEnumNames(SerializedProperty property, string[] enumNames)
        {
            var type = property.serializedObject.targetObject.GetType();

            foreach (var fieldInfo in type.GetFields())
            {
                var customAttributes = fieldInfo.GetCustomAttributes(typeof(EnumLabelAttribute), false);

                foreach (EnumLabelAttribute customAttribute in customAttributes)
                {
                    var enumType = fieldInfo.FieldType;

                    foreach (var enumName in enumNames)
                    {
                        var field = enumType.GetField(enumName);

                        if (field == null)
                        {
                            continue;
                        }

                        var attrs = (EnumLabelAttribute[]) field.GetCustomAttributes(customAttribute.GetType(), false);

                        if (!customEnumNames.ContainsKey(enumName))
                        {
                            foreach (var labelAttribute in attrs)
                            {
                                customEnumNames.Add(enumName, labelAttribute.label);
                            }
                        }
                    }
                }
            }
        }
    }
}


#endif