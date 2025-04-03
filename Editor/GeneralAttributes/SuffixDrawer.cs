using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(SuffixAttribute))]
    public class SuffixDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var suffixAttribute = (SuffixAttribute) attribute;

            var labelWidth = Mathf.Max(30f, suffixAttribute.Suffix.Length * 7f);

            var fieldRect = new Rect(position.x, position.y, position.width, position.height);
            var labelRect = new Rect(position.x + position.width - labelWidth, position.y, labelWidth, position.height);

            EditorGUI.PropertyField(fieldRect, property, label);

            var rightAlignedStyle = new GUIStyle(EditorStyles.label)
            {
                normal = {textColor = Color.grey},
                alignment = TextAnchor.MiddleRight,
                fontSize = EditorStyles.label.fontSize - 2,
                fontStyle = FontStyle.Italic
            };

            EditorGUI.LabelField(labelRect, suffixAttribute.Suffix, rightAlignedStyle);
        }
    }
}