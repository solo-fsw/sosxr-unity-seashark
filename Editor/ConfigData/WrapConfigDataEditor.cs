using UnityEditor;
using UnityEngine;

namespace SOSXR.SeaShark.EditorScripts
{
    [CustomEditor(typeof(WrapConfigData), true)]
    public class WrapConfigDataEditor : UnityEditor.Editor
    {
        private UnityEditor.Editor configDataEditor;
        private SerializedProperty configDataProp;

        private void OnEnable() => configDataProp = serializedObject.FindProperty(nameof(WrapConfigData.ConfigData));

        public override void OnInspectorGUI()
        {
            _ = DrawDefaultInspector();

            serializedObject.Update();

            _ = EditorGUILayout.PropertyField(configDataProp); // Allow assigning/removing

            if (configDataProp.objectReferenceValue != null)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Config Data", EditorStyles.boldLabel);

                if (configDataEditor == null || configDataEditor.target != configDataProp.objectReferenceValue)
                {
                    configDataEditor = CreateEditor(configDataProp.objectReferenceValue);
                }

                configDataEditor?.OnInspectorGUI();
            }

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
