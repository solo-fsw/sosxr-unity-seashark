using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes.Editor
{
    /// <summary>
    ///     Based on Warped Imagination: https://youtu.be/533OH2m7fNg?si=hfyYb2p9s5WBl1vP
    /// </summary>
    [CustomPropertyDrawer(typeof(InfoAttribute))]
    public class InfoDrawer : DecoratorDrawer
    {
        private float _height = 0f;
        private const float _padding = 20f;


        public override float GetHeight()
        {
            var attr = (InfoAttribute) attribute;

            var style = EditorStyles.helpBox;
            style.alignment = TextAnchor.MiddleLeft;
            style.wordWrap = true;
            style.padding = new RectOffset(10, 10, 10, 10);
            style.fontSize = 12;

            var inspectorWidth = Screen.width; // Strangely enough is Screen.width the width of the inspector in this case.
            _height = style.CalcHeight(new GUIContent(attr.InfoText), inspectorWidth);

            return _height * _padding;
        }


        public override void OnGUI(Rect position)
        {
            var attr = (InfoAttribute) attribute;

            position.height = _height;
            position.y += _padding * .5f;
            EditorGUI.HelpBox(position, attr.InfoText, (MessageType) attr.MessageType);
            position.y += _padding * .5f;
        }
    }
}