using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor
{
    /// <summary>
    ///     From Warped Imagination: https://youtu.be/533OH2m7fNg?si=hfyYb2p9s5WBl1vP
    /// </summary>
    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            var attr = (HorizontalLineAttribute) attribute;

            return Mathf.Max(attr.Padding, attr.Thickness);
        }


        public override void OnGUI(Rect position)
        {
            var attr = (HorizontalLineAttribute) attribute;

            position.height = attr.Thickness;
            position.position += new Vector2(0, attr.Padding / 2);

            EditorGUI.DrawRect(position, EditorGUIUtility.isProSkin ? new Color(.3f, .3f, .3f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f));

            /*var originalColor = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(position.x, position.y, position.width, 1), EditorGUIUtility.whiteTexture);
            GUI.color = originalColor;*/
        }
    }
}