#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    [CustomPropertyDrawer(typeof(SelectableLabelAttribute))]
    public class SelectableLabelDrawer : PropertyDrawer
    {
        private SelectableLabelAttribute selectableLabelAttribute => (SelectableLabelAttribute) attribute;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.SelectableLabel(position, selectableLabelAttribute.text);
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return selectableLabelAttribute.text.Split('\n').Length * base.GetPropertyHeight(property, label);
        }
    }
}


#endif