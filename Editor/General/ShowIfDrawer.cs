using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     [SerializeField] private bool m_toggleWithThisBool;
    ///     [ShowIf(nameof(m_toggleWithThisBool))]
    ///     [SerializeField] private string m_showOrHideThisValue;
    /// </summary>
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property?.serializedObject?.targetObject == null)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            if (!ShouldShow(property))
            {
                return 0;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property?.serializedObject?.targetObject == null)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            if (!ShouldShow(property))
            {
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }


        private bool ShouldShow(SerializedProperty property)
        {
            return ShouldShow(property, ((ShowIfAttribute) attribute).ConditionField);
        }


        /// <summary>
        ///     Looks the condition up on the object that declares the field, so it also works inside nested
        ///     serializable classes and list elements, and finds private fields declared on base classes.
        /// </summary>
        internal static bool ShouldShow(SerializedProperty property, string conditionField)
        {
            if (property?.serializedObject?.targetObject == null)
            {
                return true;
            }

            var parent = SerializedPropertyUtility.GetParentObject(property);
            var field = SerializedPropertyUtility.FindField(parent?.GetType(), conditionField);

            if (field == null)
            {
                return true; // fallback if field not found
            }

            return field.GetValue(parent) is true;
        }
    }
}