using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    /// <summary>
    ///     Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
    ///     Altered by Brecht Lecluyse https://www.brechtos.com
    ///     From: https://www.brechtos.com/tagselectorattribute/
    /// </summary>
    [CustomPropertyDrawer(typeof(TagSelectorAttribute))]
    public class TagSelectorDrawer : PropertyDrawer
    {
        private const string NO_TAG_OPTION = "<NoTag>";
        private const string EMPTY_TAG = "";


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Only process string properties
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);

                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Check if we should use the default Unity tag field
            if (IsUsingDefaultTagField())
            {
                DrawDefaultTagField(position, property, label);
            }
            else
            {
                DrawCustomTagDropdown(position, property, label);
            }

            EditorGUI.EndProperty();
        }


        private bool IsUsingDefaultTagField()
        {
            return attribute is TagSelectorAttribute {UseDefaultTagFieldDrawer: true};
        }


        private void DrawDefaultTagField(Rect position, SerializedProperty property, GUIContent label)
        {
            property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
        }


        private void DrawCustomTagDropdown(Rect position, SerializedProperty property, GUIContent label)
        {
            // Create tag list with "No Tag" option
            var tagList = CreateTagList();

            // Find current tag's index
            var currentIndex = FindTagIndex(property.stringValue, tagList);

            // Draw dropdown and update property
            var newIndex = EditorGUI.Popup(position, label.text, currentIndex, tagList.ToArray());
            UpdatePropertyValue(property, newIndex, tagList);
        }


        private List<string> CreateTagList()
        {
            var tagList = new List<string> {NO_TAG_OPTION};
            tagList.AddRange(InternalEditorUtility.tags);

            return tagList;
        }


        private int FindTagIndex(string propertyString, List<string> tagList)
        {
            if (string.IsNullOrEmpty(propertyString))
            {
                return 0; // Return "No Tag" index
            }

            // Find matching tag index
            for (var i = 1; i < tagList.Count; i++)
            {
                if (tagList[i] == propertyString)
                {
                    return i;
                }
            }

            return 0; // Default to "No Tag" if not found
        }


        private void UpdatePropertyValue(SerializedProperty property, int selectedIndex, List<string> tagList)
        {
            if (selectedIndex == 0)
            {
                property.stringValue = EMPTY_TAG;
            }
            else if (selectedIndex >= 1)
            {
                property.stringValue = tagList[selectedIndex];
            }
            else
            {
                property.stringValue = EMPTY_TAG;
            }
        }
    }
}