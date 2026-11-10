using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(LabelSearchAttribute))]
    public class LabelSearchDrawer : PropertyDrawer
    {
        private LabelSearchAttribute labelSearchAttribute => (LabelSearchAttribute)attribute;
        private const int CONTENT_HEIGHT = 16;


        /// <summary>
        ///     Per-field state. The attribute instance is shared by every object that uses the field,
        ///     so it must only provide the starting values, never hold state itself.
        /// </summary>
        private sealed class State
        {
            public bool Search;
            public bool Foldout;
        }


        private static readonly Dictionary<(Object Target, string Path), State> States = new();
        private State _state;


        private State GetState(SerializedProperty property)
        {
            var key = (property.serializedObject.targetObject, property.propertyPath);

            if (!States.TryGetValue(key, out var state))
            {
                state = new State {Search = labelSearchAttribute.search, Foldout = labelSearchAttribute.foldout};
                States[key] = state;
            }

            return state;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _state = GetState(property);

            if (labelSearchAttribute.canPrintLabelName)
            {
                label = new GUIContent(label) {text = $"{label.text} ( Label = {labelSearchAttribute.labelName} )"};
            }

            var previousIndent = EditorGUI.indentLevel;

            if (property.isArray)
            {
                EditorGUI.indentLevel = 0;
                var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                _state.Foldout = EditorGUI.Foldout(foldoutRect, _state.Foldout, label, true);

                if (_state.Search)
                {
                    DrawArrayProperty(position, property, label);
                }
                else
                {
                    DrawCachedArrayProperty(position, property, label);
                }
            }
            else
            {
                if (_state.Search)
                {
                    DrawSingleProperty(position, property, label);
                }
                else
                {
                    DrawCachedSingleProperty(position, property, label);
                }
            }

            _state.Search = false;
            EditorGUI.indentLevel = previousIndent;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            if (property.isArray && GetState(property).Foldout)
            {
                height = (property.arraySize + 1) * CONTENT_HEIGHT;
            }

            return base.GetPropertyHeight(property, label) + height;
        }


        private void DrawCachedSingleProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.ObjectField(position, label, property.objectReferenceValue, GetType(property), false);

            if (EditorGUI.EndChangeCheck())
            {
                property.objectReferenceValue = value;
            }

            EditorGUI.showMixedValue = false;
        }


        private void DrawCachedArrayProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_state.Foldout)
            {
                position.y += CONTENT_HEIGHT;
                EditorGUI.indentLevel = 2;
                var type = GetType(property);
                EditorGUI.LabelField(position, "Size", property.arraySize.ToString());

                for (var i = 0; i < property.arraySize; i++)
                {
                    position.y += CONTENT_HEIGHT;
                    position.height = CONTENT_HEIGHT;
                    var content = EditorGUIUtility.ObjectContent(property.GetArrayElementAtIndex(i).objectReferenceValue, type);
                    content.image = AssetPreview.GetMiniTypeThumbnail(type);
                    EditorGUI.LabelField(position, new GUIContent(ObjectNames.NicifyVariableName($"Element {i}")), content);
                }
            }
        }


        private void DrawSingleProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            var type = GetType(property);
            property.objectReferenceValue = null;

            foreach (var path in GetAllAssetPaths())
            {
                Object asset = null;

                if (!LabelSearchAttribute.assetTypes.TryGetValue(path, out var assetType))
                {
                    asset = AssetDatabase.LoadMainAssetAtPath(path);

                    if (asset == null)
                    {
                        continue;
                    }

                    assetType = asset.GetType();
                    LabelSearchAttribute.assetTypes[path] = assetType;
                }

                if (type == null || !type.IsAssignableFrom(assetType))
                {
                    continue;
                }

                asset ??= AssetDatabase.LoadMainAssetAtPath(path);

                if (asset == null)
                {
                    continue;
                }

                if (AssetDatabase.GetLabels(asset).Contains(labelSearchAttribute.labelName))
                {
                    property.objectReferenceValue = asset;

                    break;
                }
            }

            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, type, false);
        }


        private void DrawArrayProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            var size = 0;
            EditorGUI.indentLevel = 2;

            if (_state.Foldout)
            {
                position.y += CONTENT_HEIGHT;
                EditorGUI.LabelField(position, "Size", property.arraySize.ToString());
            }

            property.arraySize = 0;
            var type = GetType(property); // From the field's declared type: works for empty arrays and leaves no stray null element

            foreach (var path in GetAllAssetPaths())
            {
                Object asset = null;

                if (!LabelSearchAttribute.assetTypes.TryGetValue(path, out var assetType))
                {
                    asset = AssetDatabase.LoadMainAssetAtPath(path);

                    if (asset == null)
                    {
                        continue;
                    }

                    assetType = asset.GetType();
                    LabelSearchAttribute.assetTypes[path] = assetType;
                }

                if (type == null || !type.IsAssignableFrom(assetType))
                {
                    continue;
                }

                asset ??= AssetDatabase.LoadMainAssetAtPath(path);

                if (asset == null)
                {
                    continue;
                }

                if (AssetDatabase.GetLabels(asset).Contains(labelSearchAttribute.labelName))
                {
                    property.arraySize = ++size;
                    property.GetArrayElementAtIndex(size - 1).objectReferenceValue = asset;

                    if (_state.Foldout)
                    {
                        position.y += CONTENT_HEIGHT;
                        position.height = CONTENT_HEIGHT;
                        var content = EditorGUIUtility.ObjectContent(asset, type);
                        content.image = AssetPreview.GetMiniTypeThumbnail(type);

                        EditorGUI.LabelField(position,
                            new GUIContent(ObjectNames.NicifyVariableName($"Element {size - 1}")), content);
                    }

                    if (labelSearchAttribute.limit <= property.arraySize)
                    {
                        break;
                    }
                }
            }
        }


        private string[] GetAllAssetPaths()
        {
            var allPaths = AssetDatabase.GetAllAssetPaths();
            Array.Sort(allPaths);

            if (labelSearchAttribute.direction == LabelSearchAttribute.Direction.DESC)
            {
                Array.Reverse(allPaths);
            }

            return allPaths;
        }


        /// <summary>
        ///     The element type of the field (T, T[] or List&lt;T&gt;). Taken from the field itself instead of the
        ///     serialized type name, so it is not limited to UnityEngine.* types.
        /// </summary>
        private Type GetType(SerializedProperty property)
        {
            var type = fieldInfo.FieldType;

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }
    }
}
