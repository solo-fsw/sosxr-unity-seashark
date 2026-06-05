using System;
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


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!labelSearchAttribute.init)
            {
                labelSearchAttribute.init = true;

                return;
            }

            if (labelSearchAttribute.canPrintLabelName)
            {
                label.text += $" ( Label = {labelSearchAttribute.labelName} )";
            }

            if (property.isArray)
            {
                EditorGUI.indentLevel = 0;
                labelSearchAttribute.foldout = EditorGUI.Foldout(position, labelSearchAttribute.foldout, label);

                if (labelSearchAttribute.search)
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
                if (labelSearchAttribute.search)
                {
                    DrawSingleProperty(position, property, label);
                }
                else
                {
                    DrawCachedSingleProperty(position, property, label);
                }
            }

            labelSearchAttribute.search = false;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0;

            if (property.isArray && labelSearchAttribute.foldout)
            {
                height = (property.arraySize + 1) * CONTENT_HEIGHT;
            }

            return base.GetPropertyHeight(property, label) + height;
        }


        private void DrawCachedSingleProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            property.objectReferenceValue = EditorGUI.ObjectField(
                position, label, property.objectReferenceValue, GetType(property), false);
        }


        private void DrawCachedArrayProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            if (labelSearchAttribute.foldout)
            {
                position.y += CONTENT_HEIGHT;
                EditorGUI.indentLevel = 2;
                var type = GetType(property.GetArrayElementAtIndex(0));
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

                if (type != assetType)
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

            if (labelSearchAttribute.foldout)
            {
                position.y += CONTENT_HEIGHT;
                EditorGUI.LabelField(position, "Size", property.arraySize.ToString());
            }

            property.arraySize = 0;
            property.InsertArrayElementAtIndex(0);
            var type = GetType(property.GetArrayElementAtIndex(0));

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

                if (type != assetType)
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

                    if (labelSearchAttribute.foldout)
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


        private Type GetType(SerializedProperty property)
        {
            try
            {
                var assembly = Assembly.Load("UnityEngine");
                if (assembly == null)
                    return null;

                return assembly.GetType("UnityEngine." + property.type.Replace("PPtr<$", "").Replace(">", ""));
            }
            catch (Exception e)
            {
                Debug.LogError($"Couldn't get type due to {e}");
                return null;
            }
        }
    }
}
