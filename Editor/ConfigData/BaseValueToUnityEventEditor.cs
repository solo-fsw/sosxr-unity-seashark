using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    [CustomEditor(typeof(ConfigValueToUnityEvent<>), true)] // Supports subclasses
    [CanEditMultipleObjects]
    public class BaseValueToUnityEventEditor : UnityEditor.Editor
    {
        private string[] validValueNames;
        private Type fieldType;
        private SerializedProperty configDataProp;
        private SerializedProperty valueNameProp;
        private SerializedProperty eventToFireProp;
        private SerializedProperty runOnStartProp;
        private SerializedProperty subscribeToChangesProp;

        private SerializedProperty invertProp;


        private void OnEnable()
        {
            runOnStartProp = serializedObject.FindProperty(nameof(ConfigValueToUnityEvent<object>.RunOnStart));
            configDataProp = serializedObject.FindProperty(nameof(ConfigValueToUnityEvent<object>.ConfigData));
            valueNameProp = serializedObject.FindProperty(nameof(ConfigValueToUnityEvent<object>.ValueName));
            eventToFireProp = serializedObject.FindProperty(nameof(ConfigValueToUnityEvent<object>.EventToFire));
            subscribeToChangesProp = serializedObject.FindProperty(nameof(ConfigValueToUnityEvent<object>.SubscribeToChanges));

            invertProp = serializedObject.FindProperty(nameof(ConfigBoolToUnityEvent.Invert));

            UpdateFieldList();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(configDataProp);

            if (configDataProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a ConfigData instance to select a field.", MessageType.Info);
                serializedObject.ApplyModifiedProperties();

                return;
            }

            UpdateFieldList(); // Always update field list after assigning a ConfigData

            var selectedIndex = Array.IndexOf(validValueNames, valueNameProp.stringValue);

            if (validValueNames.Length > 0)
            {
                selectedIndex = EditorGUILayout.Popup("Value Name", selectedIndex, validValueNames);
            }
            else
            {
                var typeName = fieldType.Name;

                if (typeName == "Single")
                {
                    typeName = "Float";
                }

                EditorGUILayout.HelpBox($"No properties or fields of type {typeName} found on {configDataProp.objectReferenceValue.GetType().Name}.", MessageType.Warning);
                serializedObject.ApplyModifiedProperties();

                return;
            }

            if (selectedIndex >= 0)
            {
                valueNameProp.stringValue = validValueNames[selectedIndex];
            }

            EditorGUILayout.PropertyField(runOnStartProp);

            EditorGUILayout.PropertyField(subscribeToChangesProp);

            if (invertProp != null)
            {
                EditorGUILayout.PropertyField(invertProp);
            }

            EditorGUILayout.PropertyField(eventToFireProp);

            if (GUILayout.Button("Find Values and Fire Event"))
            {
                ((ConfigValueToUnityEvent<object>) target).FireCurrentValue();
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void UpdateFieldList()
        {
            if (configDataProp == null)
            {
                return;
            }

            if (configDataProp.objectReferenceValue == null)
            {
                validValueNames = Array.Empty<string>();

                return;
            }

            var configType = configDataProp.objectReferenceValue.GetType();
            var baseType = target.GetType().BaseType?.GetGenericArguments().FirstOrDefault();

            if (baseType == null)
            {
                validValueNames = Array.Empty<string>();

                return;
            }

            fieldType = baseType; // Store the expected type for filtering

            var properties = configType
                             .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.PropertyType == fieldType && p.CanRead)
                             .Select(p => p.Name);

            var fields = configType
                         .GetFields(BindingFlags.Public | BindingFlags.Instance)
                         .Where(f => f.FieldType == fieldType)
                         .Select(f => f.Name);

            validValueNames = properties.Concat(fields).ToArray();
        }
    }
}