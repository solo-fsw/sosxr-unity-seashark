using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Custom Inspector for ConfigValueToUnityEvent&lt;T&gt; and its subclasses. Lets you pick a ConfigData asset,
    ///     choose a matching field or property, and fire the event from the Inspector.
    /// </summary>
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

        /// <summary>
        ///     Draws the ConfigData field, value name popup and event settings, and the Find Values and Fire Event button.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _ = EditorGUILayout.PropertyField(configDataProp);

            if (configDataProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Assign a ConfigData instance to select a field.", MessageType.Info);
                _ = serializedObject.ApplyModifiedProperties();

                return;
            }

            UpdateFieldList(); // Always update field list after assigning a ConfigData

            int selectedIndex = Array.IndexOf(validValueNames, valueNameProp.stringValue);

            if (validValueNames.Length > 0)
            {
                // Only write on a user change, so multi-object editing doesn't stamp the first target's value onto the others.
                EditorGUI.showMixedValue = valueNameProp.hasMultipleDifferentValues;
                EditorGUI.BeginChangeCheck();
                selectedIndex = EditorGUILayout.Popup("Value Name", selectedIndex, validValueNames);

                if (EditorGUI.EndChangeCheck() && selectedIndex >= 0)
                {
                    valueNameProp.stringValue = validValueNames[selectedIndex];
                }

                EditorGUI.showMixedValue = false;
            }
            else
            {
                string typeName = fieldType != null ? fieldType.Name : "<unknown>";

                if (typeName == "Single")
                {
                    typeName = "Float";
                }

                EditorGUILayout.HelpBox($"No properties or fields of type {typeName} found on {configDataProp.objectReferenceValue.GetType().Name}.", MessageType.Warning);
                _ = serializedObject.ApplyModifiedProperties();

                return;
            }

            _ = EditorGUILayout.PropertyField(runOnStartProp);

            _ = EditorGUILayout.PropertyField(subscribeToChangesProp);

            if (invertProp != null)
            {
                _ = EditorGUILayout.PropertyField(invertProp);
            }

            _ = EditorGUILayout.PropertyField(eventToFireProp);

            if (GUILayout.Button("Find Values and Fire Event"))
            {
                // A cast to ConfigValueToUnityEvent<object> always fails (generic types are invariant), so use reflection.
                foreach (var t in targets)
                {
                    var fire = t.GetType().GetMethod("FireCurrentValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                    fire?.Invoke(t, null);
                }
            }

            _ = serializedObject.ApplyModifiedProperties();
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
            var baseType = FindValueType(target.GetType());

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

        /// <summary>
        ///     Walks up the hierarchy to ConfigValueToUnityEvent&lt;T&gt; and returns T, so deeper subclasses work too.
        /// </summary>
        private static Type FindValueType(Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(ConfigValueToUnityEvent<>))
                {
                    return current.GetGenericArguments()[0];
                }
            }

            return null;
        }
    }
}
