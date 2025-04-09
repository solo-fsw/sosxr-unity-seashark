using System;
using System.Linq;
using System.Reflection;
using SOSXR.SeaShark.QueryData;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.Attributes.Editor
{
    [CustomEditor(typeof(QueryWrapper), true)]
    public class QueryWrapperEditor : UnityEditor.Editor
    {
        private readonly string[] _excludedNamesTwo =
        {
            "BaseURL", "QueryStringURL", "QueryStringVariables", "PPNString", "UpdateJsonOnValueChange", "ClipDirectory", "DebugUpdateInterval"
        };

        private string[] _validValueNames = Array.Empty<string>();

        private QueryWrapper _queryWrapper;

        private SerializedProperty _dataObjectProp;
        private Object _previousDataObject;


        private void OnEnable()
        {
            _queryWrapper = serializedObject.targetObject as QueryWrapper;

            _dataObjectProp = serializedObject.FindProperty(nameof(_queryWrapper.DataObject));

            if (_dataObjectProp != null && _dataObjectProp.objectReferenceValue != null)
            {
                _previousDataObject = _dataObjectProp.objectReferenceValue;
                UpdateFieldsAndPropertiesList();
            }
        }


        private void UpdateFieldsAndPropertiesList()
        {
            if (_dataObjectProp == null)
            {
                Debug.Log("ConfigDataProp is null");

                return;
            }

            if (_dataObjectProp.objectReferenceValue == null)
            {
                _validValueNames = Array.Empty<string>();

                return;
            }

            var objectType = _dataObjectProp.objectReferenceValue.GetType();

            var properties = objectType
                             .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             .Where(p => p.CanRead)
                             .Where(p => !p.IsUnityBaseMember())
                             .Where(p => !p.IsWrongType())
                             .Where(p => p.PropertyType is {IsArray: false, IsGenericType: false})
                             .Where(p => !_excludedNamesTwo.Contains(p.Name))
                             .Select(p => p.Name);

            var fields = objectType
                         .GetFields(BindingFlags.Public | BindingFlags.Instance)
                         .Where(f => !f.IsUnityBaseMember())
                         .Where(f => !f.IsWrongType())
                         .Where(f => f.FieldType is {IsArray: false, IsGenericType: false})
                         .Where(f => !_excludedNamesTwo.Contains(f.Name))
                         .Select(f => f.Name);

            _validValueNames = properties.Concat(fields).ToArray();
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            var currentDataObject = _dataObjectProp?.objectReferenceValue;

            if (currentDataObject == null)
            {
                EditorGUILayout.HelpBox("No data object found.", MessageType.Warning);

                return;
            }

            if (currentDataObject != _previousDataObject)
            {
                _queryWrapper?.QueryData?.ClearQuery();

                UpdateFieldsAndPropertiesList();

                _previousDataObject = currentDataObject;
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("This editor will allow you to select which fields or properties will be included in the QueryURL", MessageType.Info);

            if (_validValueNames.Length > 0)
            {
                GUILayout.Space(10);

                EditorGUILayout.LabelField("Included Variables", EditorStyles.boldLabel);

                GUILayout.Space(5);

                DrawCheckBoxes();

                GUILayout.Space(15);

                EditorGUILayout.LabelField("Example query URL  :  " + _queryWrapper?.QueryData?.QueryStringURL, EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.HelpBox("No valid fields or properties found.", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void DrawCheckBoxes()
        {
            var stringVars = _queryWrapper.QueryData.QueryStringVariables;

            for (var i = 0; i < _validValueNames.Length; i++)
            {
                var isSelected = stringVars.Count > 0 && Enumerable.Range(0, stringVars.Count)
                                                                   .Select(index => stringVars[index])
                                                                   .Contains(_validValueNames[i]);

                var newSelected = EditorGUILayout.ToggleLeft(_validValueNames[i], isSelected);

                if (newSelected == isSelected)
                {
                    continue;
                }

                UpdateQuery(_validValueNames[i]);
            }
        }


        private void UpdateQuery(string value)
        {
            serializedObject.ApplyModifiedProperties();

            _queryWrapper?.QueryData?.ModifyVariables(value); // Remove if exists, add if not

            EditorUtility.SetDirty(target);
        }
    }
}