using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    [CustomPropertyDrawer(typeof(Medium))]
    public class MediatorDrawer : PropertyDrawer
    {
        private int _numberOfListeners;
        private int _numberOfCallers;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var mediatorAttribute = (MediatorAttribute) fieldInfo.GetCustomAttribute(typeof(MediatorAttribute));

            if (mediatorAttribute == null)
            {
                EditorGUI.HelpBox(position, "MediatorAttribute not found!", MessageType.Warning);

                return;
            }

            var eventNameProperty = property.FindPropertyRelative(nameof(Medium.Channel));
            var typeProperty = property.FindPropertyRelative(nameof(Medium.TypeName));
            var eventDataProperty = property.FindPropertyRelative(nameof(Medium.DataString));

            var registry = Resources.Load<MediatorRegistry>(nameof(MediatorRegistry));

            if (registry == null)
            {
                EditorGUI.PropertyField(position, eventNameProperty, label);

                return;
            }

            // Reset position height
            position.height = EditorGUIUtility.singleLineHeight;

            var propertyName = FormatDisplayName(property, eventNameProperty.stringValue);

            // Name field
            if (mediatorAttribute.EditableChannel)
            {
                DrawNameTextField(position, propertyName, eventNameProperty);
            }
            else
            {
                DrawNameDropdown(position, registry, eventNameProperty, propertyName);
            }

            // Move to data field
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Draw data field
            GUI.enabled = false;

            EditorGUI.TextField(position, "Type", typeProperty.stringValue);
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.TextField(position, "Data", eventDataProperty.stringValue);
            GUI.enabled = true;

            property.serializedObject.ApplyModifiedProperties();
        }


        private static string FormatDisplayName(SerializedProperty property, string nameString)
        {
            var propertyName = property.name;

            if (propertyName.StartsWith("m_"))
            {
                propertyName = propertyName.Substring(2);
            }

            propertyName = char.ToUpper(propertyName[0]) + propertyName.Substring(1);

            if (propertyName == "Data")
            {
                propertyName = nameString;
            }

            return propertyName;
        }


        private static void DrawNameTextField(Rect position, string propertyName, SerializedProperty eventNameProperty)
        {
            var mediumName = EditorGUI.TextField(position, propertyName, eventNameProperty.stringValue);

            if (mediumName != eventNameProperty.stringValue)
            {
                eventNameProperty.stringValue = mediumName;
            }
        }


        private static void DrawNameDropdown(Rect position, MediatorRegistry registry, SerializedProperty eventNameProperty, string propertyName)
        {
            var eventNames = registry.Registry.ConvertAll(m => m.Medium.Channel);
            var selectedIndex = Mathf.Max(eventNames.IndexOf(eventNameProperty.stringValue), 0);
            selectedIndex = EditorGUI.Popup(position, propertyName, selectedIndex, eventNames.ToArray());

            if (selectedIndex >= 0 && selectedIndex < eventNames.Count)
            {
                eventNameProperty.stringValue = eventNames[selectedIndex];
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var mediatorAttribute = (MediatorAttribute) fieldInfo.GetCustomAttribute(typeof(MediatorAttribute));

            if (mediatorAttribute == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var height = EditorGUIUtility.singleLineHeight * 3 + // Channel, Type and Data fields
                         EditorGUIUtility.standardVerticalSpacing * 2; // Spacing between fields

            return height;
        }
    }
}