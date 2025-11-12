using System.Reflection;
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
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enableIf = (EnableIfAttribute) attribute;
            var target = property.serializedObject.targetObject;

            var conditionField = target.GetType().GetField(enableIf.ConditionField,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (conditionField == null || conditionField.FieldType != typeof(bool))
            {
                EditorGUI.PropertyField(position, property, label, true);

                return;
            }

            var currentValue = (bool) conditionField.GetValue(target);

            var toggleWidth = 18f;
            var spacing = 6f;

            var toggleRect = new Rect(position.x, position.y, toggleWidth, position.height);

            var fieldRect = new Rect(position.x + toggleWidth + spacing, position.y,
                position.width - toggleWidth - spacing, position.height);

            EditorGUI.BeginChangeCheck();
            var newValue = EditorGUI.Toggle(toggleRect, currentValue);

            if (EditorGUI.EndChangeCheck())
            {
                conditionField.SetValue(target, newValue);
                EditorUtility.SetDirty(target);
            }

            EditorGUI.BeginDisabledGroup(!newValue);
            EditorGUI.PropertyField(fieldRect, property, label, true);
            EditorGUI.EndDisabledGroup();
        }
    }
}