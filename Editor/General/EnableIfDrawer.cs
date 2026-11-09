using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Usage:
    ///     [SerializeField, HideInInspector] private bool m_toggleWithThisBool;
    ///     [EnableIf(nameof(m_toggleWithThisBool))]
    ///     [SerializeField] private string m_disableOrEnableThisField;
    /// </summary>
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public class EnableIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enableIf = (EnableIfAttribute) attribute;
            var condition = FindSiblingProperty(property, enableIf.ConditionField);

            // The condition must be a serialized bool, so edits go through SerializedProperty:
            // Undo, prefab overrides and multi-object editing then work.
            if (condition == null || condition.propertyType != SerializedPropertyType.Boolean)
            {
                EditorGUI.PropertyField(position, property, label, true);

                return;
            }

            const float toggleWidth = 18f;
            const float spacing = 6f;

            var toggleRect = new Rect(position.x, position.y, toggleWidth, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(position.x + toggleWidth + spacing, position.y, position.width - toggleWidth - spacing, position.height);

            EditorGUI.BeginProperty(toggleRect, GUIContent.none, condition);
            EditorGUI.showMixedValue = condition.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.Toggle(toggleRect, condition.boolValue);

            if (EditorGUI.EndChangeCheck())
            {
                condition.boolValue = newValue;
            }

            EditorGUI.showMixedValue = false;
            EditorGUI.EndProperty();

            EditorGUI.BeginDisabledGroup(!condition.boolValue);
            EditorGUI.PropertyField(fieldRect, property, label, true);
            EditorGUI.EndDisabledGroup();
        }


        /// <summary>
        ///     Finds the condition field next to <paramref name="property" /> (also inside nested classes/arrays).
        /// </summary>
        private static SerializedProperty FindSiblingProperty(SerializedProperty property, string name)
        {
            if (property == null || string.IsNullOrEmpty(name))
            {
                return null;
            }

            var path = property.propertyPath;
            var lastDot = path.LastIndexOf('.');
            var siblingPath = lastDot >= 0 ? path.Substring(0, lastDot + 1) + name : name;

            return property.serializedObject.FindProperty(siblingPath);
        }
    }
}