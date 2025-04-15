using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    /// <summary>
    ///     Use this class to draw the <see cref="OptionalAttribute" /> in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(OptionalAttribute))]
    public class OptionalDrawer : DecoratorDrawer
    {
        private const float INDENT_WIDTH = 15f;


        public override void OnGUI(Rect position)
        {
            var optionalAttr = attribute as OptionalAttribute;

            var label = GetLabelText(optionalAttr.Type);

            var style = new GUIStyle(EditorStyles.miniLabel);
            style.normal.textColor = new Color(style.normal.textColor.r, style.normal.textColor.g, style.normal.textColor.b, 0.55f);

            var textSize = style.CalcSize(new GUIContent(label));

            position.height = EditorGUIUtility.singleLineHeight;
            position.x += EditorGUIUtility.labelWidth - textSize.x - EditorGUI.indentLevel * INDENT_WIDTH;

            EditorGUI.LabelField(position, label, style);
        }


        private string GetLabelText(OptionalType type)
        {
            switch (type)
            {
                case OptionalType.WillAdd:
                    return "(Will Add)";
                case OptionalType.WillGet:
                    return "(Will Get)";
                case OptionalType.WillFind:
                    return "(Will Find)";
                case OptionalType.Optional:
                    return "(Optional)";
            }

            return null;
        }


        public override float GetHeight()
        {
            return 0f;
        }
    }
}