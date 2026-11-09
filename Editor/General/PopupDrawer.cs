#if UNITY_EDITOR

using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupDrawer : PropertyDrawer
    {
        private string[] _list;

        private PopupAttribute popupAttribute => (PopupAttribute) attribute;


        private string[] list
        {
            get
            {
                if (_list == null)
                {
                    var source = popupAttribute.list ?? Array.Empty<object>();
                    _list = new string[source.Length];

                    for (var i = 0; i < source.Length; i++)
                    {
                        _list[i] = Convert.ToString(source[i], CultureInfo.InvariantCulture);
                    }
                }

                return _list;
            }
        }


        // No cached closures: a PropertyDrawer instance is shared by every element of an array/list,
        // so the property must always come from the OnGUI argument.
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (list.Length == 0 || !IsSupported(property))
            {
                EditorGUI.PropertyField(position, property, label);

                return;
            }

            var currentIndex = Array.IndexOf(list, GetValueAsString(property));

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var selectedIndex = EditorGUI.Popup(position, label.text, Mathf.Max(currentIndex, 0), list);

            if (EditorGUI.EndChangeCheck())
            {
                SetValue(property, list[selectedIndex]);
            }

            EditorGUI.EndProperty();
        }


        private static bool IsSupported(SerializedProperty property)
        {
            return property.propertyType is SerializedPropertyType.String or SerializedPropertyType.Integer or SerializedPropertyType.Float;
        }


        private static string GetValueAsString(SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.String => property.stringValue,
                SerializedPropertyType.Integer => property.intValue.ToString(CultureInfo.InvariantCulture),
                SerializedPropertyType.Float => property.floatValue.ToString(CultureInfo.InvariantCulture),
                _ => null
            };
        }


        private static void SetValue(SerializedProperty property, string value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = value;

                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);

                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = Convert.ToSingle(value, CultureInfo.InvariantCulture);

                    break;
            }
        }
    }
}


#endif