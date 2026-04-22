using System;
using System.Collections;
using System.Reflection;
// using BasteRainGames;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.Editor
{
    [CustomPropertyDrawer(typeof(HidingAttribute), true)]
    public class HidingDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldHide(property) ? 0f : EditorGUI.GetPropertyHeight(property, label, true);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldHide(property))
            {
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }


        private bool ShouldHide(SerializedProperty property)
        {
            var attr = (HidingAttribute) attribute;
            var parent = GetParentObject(property);

            switch (attr)
            {
                case HideIfAttribute hideIfAttr:
                    return GetBoolValue(parent, hideIfAttr.variable) == hideIfAttr.state;

                case HideIfNullAttribute hideIfNullAttr:
                    return GetObjectValue(parent, hideIfNullAttr.variable) == null;

                case HideIfNotNullAttribute hideIfNotNullAttr:
                    return GetObjectValue(parent, hideIfNotNullAttr.variable) != null;

                case HideIfEnumValueAttribute hideIfEnumAttr:
                    var enumValue = GetEnumValue(parent, hideIfEnumAttr.variable);
                    var match = Array.Exists(hideIfEnumAttr.states, s => s == enumValue);

                    return hideIfEnumAttr.hideIfEqual ? match : !match;
            }

            return false;
        }


        private object GetParentObject(SerializedProperty property)
        {
            var path = property.propertyPath; // e.g. "list.Array.data[0].OriginName"
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');

            for (var i = 0; i < elements.Length - 1; i++)
            {
                var element = elements[i];

                if (element.StartsWith("Array"))
                {
                    var index = int.Parse(elements[i + 1].Replace("data[", "").Replace("]", ""));
                    obj = ((IList) obj)[index];
                    i++;
                }
                else
                {
                    var field = obj.GetType().GetField(element, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    if (field == null)
                    {
                        return null;
                    }

                    obj = field.GetValue(obj);
                }
            }

            return obj;
        }


        private bool GetBoolValue(object obj, string variable)
        {
            var field = obj.GetType().GetField(variable, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null && field.FieldType == typeof(bool))
            {
                return (bool) field.GetValue(obj);
            }

            return false;
        }


        private Object GetObjectValue(object obj, string variable)
        {
            var field = obj.GetType().GetField(variable, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null && typeof(Object).IsAssignableFrom(field.FieldType))
            {
                return (Object) field.GetValue(obj);
            }

            return null;
        }


        private int GetEnumValue(object obj, string variable)
        {
            var field = obj.GetType().GetField(variable, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field != null && field.FieldType.IsEnum)
            {
                return (int) field.GetValue(obj);
            }

            return -1;
        }
    }
}