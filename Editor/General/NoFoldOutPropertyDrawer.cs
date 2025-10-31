using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Based on the Unity Timeline Samples
    ///     Draws all child properties inline
    /// </summary>
    [CustomPropertyDrawer(typeof(NoFoldOutAttribute))]
    public class NoFoldOutPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.hasVisibleChildren)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            var total = 0f;
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            // include the label height only once, not per child
            total += EditorGUIUtility.singleLineHeight;

            var enterChildren = iterator.NextVisible(true);

            while (enterChildren && !SerializedProperty.EqualContents(iterator, end))
            {
                total += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
                enterChildren = iterator.NextVisible(false);
            }

            return total;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.hasVisibleChildren)
            {
                EditorGUI.PropertyField(position, property, label, true);

                return;
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = indent + 1;

            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, label);

            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            var enterChildren = iterator.NextVisible(true);

            while (enterChildren && !SerializedProperty.EqualContents(iterator, end))
            {
                var height = EditorGUI.GetPropertyHeight(iterator, true);
                position.height = height;

                EditorGUI.PropertyField(position, iterator, true);

                position.y += height + EditorGUIUtility.standardVerticalSpacing;
                enterChildren = iterator.NextVisible(false);
            }

            EditorGUI.indentLevel = indent;
        }
    }
}