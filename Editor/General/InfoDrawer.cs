using UnityEditor;
using UnityEngine;

namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Based on Warped Imagination: https://youtu.be/533OH2m7fNg?si=hfyYb2p9s5WBl1vP
    /// </summary>
    [CustomPropertyDrawer(typeof(InfoAttribute))]
    public class InfoDrawer : DecoratorDrawer
    {
        private const float _padding = 4f;
        private const float _fallbackWidth = 300f;
        private float _lastWidth = -1f;

        public override float GetHeight()
        {
            var attr = (InfoAttribute)attribute;
            var style = new GUIStyle(EditorStyles.helpBox) { wordWrap = true, fontSize = 12 };
            var inspectorWidth = GetAvailableWidth();

            return style.CalcHeight(new GUIContent(attr.InfoText), inspectorWidth) + _padding * 2;
        }

        /// <summary>
        ///     UI Toolkit's PropertyField calls GetHeight outside OnGUI, where currentViewWidth throws.
        ///     Use the width from the last OnGUI pass, or a fallback until the first OnGUI pass.
        /// </summary>
        private float GetAvailableWidth()
        {
            if (_lastWidth > 0f)
            {
                return _lastWidth;
            }

            return _fallbackWidth;
        }

        public override void OnGUI(Rect position)
        {
            var attr = (InfoAttribute)attribute;
            var style = new GUIStyle(EditorStyles.helpBox) { wordWrap = true, fontSize = 12 };
            _lastWidth = position.width;
            position.y += _padding;
            position.height -= _padding * 2;
            EditorGUI.HelpBox(position, attr.InfoText, (MessageType)attr.MessageType);
        }
    }
}
