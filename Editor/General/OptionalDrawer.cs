using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Use this class to draw the <see cref="OptionalAttribute" /> in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(OptionalAttribute))]
    public class OptionalDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var optionalAttr = (OptionalAttribute) attribute;

            EditorGUI.PropertyField(position, property, label);

            var style = new GUIStyle(EditorStyles.miniLabel)
            {
                // Dimmed label colour of the current skin, so it is readable in both the light and dark theme.
                normal = {textColor = DimmedLabelColor()},
                alignment = TextAnchor.MiddleRight
            };

            var text = GetLabelText(optionalAttr.Type);
            var size = style.CalcSize(new GUIContent(text));

            var rect = new Rect(position.xMax - size.x - 20f, position.y, size.x, position.height);
            EditorGUI.LabelField(rect, text, style);
        }


        private static Color DimmedLabelColor()
        {
            var color = EditorStyles.label.normal.textColor;
            color.a = 0.55f;

            return color;
        }


        private string GetLabelText(OptionalType type)
        {
            return type switch
                   {
                       OptionalType.WillAdd => "(Will Add)",
                       OptionalType.WillGet => "(Will Get)",
                       OptionalType.WillFind => "(Will Find)",
                       OptionalType.Optional => "(Optional)",
                       _ => ""
                   };
        }
    }
}