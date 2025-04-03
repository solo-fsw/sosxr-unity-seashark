using System.IO;
using System.Linq;
using System.Reflection;
using SOSXR.SeaShark.Attributes.Runtime.ConfigData;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor.ConfigData
{
    [CustomEditor(typeof(BaseConfigData), true)]
    public class ConfigDataEditor : UnityEditor.Editor
    {
        private readonly string[] _excludedNames = {"name", "hideFlags", "UpdateJsonOnValueChange"};
        private string[] _validValueNames;
        private SerializedProperty _updateJsonOnValueChangeProp;
        private bool[] _selectedValues;


        private void OnEnable()
        {
            _updateJsonOnValueChangeProp = serializedObject.FindProperty("m_updateJsonOnSpecificValueChanged");

            UpdateFieldsAndPropertiesList();
        }


        private void UpdateFieldsAndPropertiesList()
        {
            var configType = target.GetType();

            var properties = configType
                             .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanRead && !_excludedNames.Contains(p.Name))
                             .Select(p => p.Name);

            var fields = configType
                         .GetFields(BindingFlags.Public | BindingFlags.Instance)
                         .Where(f => !_excludedNames.Contains(f.Name))
                         .Select(f => f.Name);

            _validValueNames = properties.Concat(fields).ToArray();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var configData = (BaseConfigData) target;

            DrawDefaultInspector();

            EditorGUILayout.Space();

            if (_validValueNames.Length > 0)
            {
                EditorGUILayout.LabelField("Changes to these fields/properties will trigger a JSON update", EditorStyles.boldLabel);

                DrawCheckBoxes();
            }
            else
            {
                EditorGUILayout.HelpBox("No valid fields or properties found.", MessageType.Warning);
            }

            GUILayout.Space(50);

            DrawButtons(configData);

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawCheckBoxes()
        {
            for (var i = 0; i < _validValueNames.Length; i++)
            {
                var isSelected = _updateJsonOnValueChangeProp.arraySize > 0 &&
                                 Enumerable.Range(0, _updateJsonOnValueChangeProp.arraySize)
                                           .Select(index => _updateJsonOnValueChangeProp.GetArrayElementAtIndex(index).stringValue)
                                           .Contains(_validValueNames[i]);

                var newSelected = EditorGUILayout.ToggleLeft(_validValueNames[i], isSelected);

                if (newSelected != isSelected)
                {
                    if (newSelected)
                    {
                        AddToJsonUpdateList(_validValueNames[i]);
                    }
                    else
                    {
                        RemoveFromJsonUpdateList(_validValueNames[i]);
                    }
                }
            }
        }


        private void AddToJsonUpdateList(string value)
        {
            _updateJsonOnValueChangeProp.arraySize++;
            _updateJsonOnValueChangeProp.GetArrayElementAtIndex(_updateJsonOnValueChangeProp.arraySize - 1).stringValue = value;
        }


        private void RemoveFromJsonUpdateList(string value)
        {
            for (var i = 0; i < _updateJsonOnValueChangeProp.arraySize; i++)
            {
                if (_updateJsonOnValueChangeProp.GetArrayElementAtIndex(i).stringValue == value)
                {
                    _updateJsonOnValueChangeProp.DeleteArrayElementAtIndex(i);

                    break;
                }
            }
        }


        private static void DrawButtons(BaseConfigData configData)
        {
            if (!File.Exists(HandleConfigData.ConfigPath))
            {
                if (GUILayout.Button(nameof(HandleConfigData.WriteConfigToJson)))
                {
                    HandleConfigData.WriteConfigToJson(configData);
                }

                return;
            }

            if (GUILayout.Button(nameof(HandleConfigData.LoadConfigFromJson)))
            {
                HandleConfigData.LoadConfigFromJson(configData);
            }

            if (GUILayout.Button(nameof(HandleConfigData.UpdateConfigJson)))
            {
                HandleConfigData.UpdateConfigJson(configData);
            }

            if (GUILayout.Button(nameof(HandleConfigData.DeleteConfigJson)))
            {
                HandleConfigData.DeleteConfigJson();
            }

            #if !UNITY_EDITOR_LINUX
            if (GUILayout.Button("Reveal in Finder"))
            {
                EditorUtility.RevealInFinder(HandleConfigData.ConfigPath);
            }
            #endif
        }
    }
}