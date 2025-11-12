using System.Reflection;
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
            if (!ShouldShow(property))
            {
                return 0;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property))
            {
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }


        private bool ShouldShow(SerializedProperty property)
        {
            var showIf = (ShowIfAttribute) attribute;
            var target = property.serializedObject.targetObject;

            var conditionField = target.GetType().GetField(showIf.ConditionField, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (conditionField == null)
            {
                return true; // fallback if field not found
            }

            var value = conditionField.GetValue(target);

            return value is true;
        }
    }
}