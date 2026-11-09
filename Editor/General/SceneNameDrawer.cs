#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameDrawer : PropertyDrawer
    {
        private SceneNameAttribute sceneNameAttribute => (SceneNameAttribute) attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneNames = GetEnabledSceneNames();

            if (sceneNames.Length == 0)
            {
                EditorGUI.LabelField(position, ObjectNames.NicifyVariableName(property.name), "Scene is Empty");

                return;
            }

            // Derive the selection from the property each time; the attribute is shared between properties.
            var options = sceneNames.ToList();
            var currentIndex = options.IndexOf(property.stringValue);

            if (currentIndex == -1 && !string.IsNullOrEmpty(property.stringValue))
            {
                options.Insert(0, property.stringValue + " (not in Build Settings)");
                currentIndex = 0;
            }

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var selected = EditorGUI.Popup(position, label.text, Mathf.Max(currentIndex, 0), options.ToArray());

            // Only write when the user picks a real scene, never silently overwrite the stored value.
            if (EditorGUI.EndChangeCheck() && Array.IndexOf(sceneNames, options[selected]) >= 0)
            {
                property.stringValue = options[selected];
            }

            EditorGUI.EndProperty();
        }


        private string[] GetEnabledSceneNames()
        {
            var scenes = (sceneNameAttribute.enableOnly ? EditorBuildSettings.scenes.Where(scene => scene.enabled) : EditorBuildSettings.scenes).ToList();
            var sceneNames = new HashSet<string>();
            scenes.ForEach(scene => { sceneNames.Add(scene.path.Substring(scene.path.LastIndexOf("/") + 1).Replace(".unity", string.Empty)); });

            return sceneNames.ToArray();
        }
    }
}
#endif