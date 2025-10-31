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


        public override float GetHeight()
        {
            var attr = (InfoAttribute) attribute;
            var style = new GUIStyle(EditorStyles.helpBox) {wordWrap = true, fontSize = 12};
            var inspectorWidth = EditorGUIUtility.currentViewWidth - 20f;

            return style.CalcHeight(new GUIContent(attr.InfoText), inspectorWidth) + _padding * 2;
        }


        public override void OnGUI(Rect position)
        {
            var attr = (InfoAttribute) attribute;
            var style = new GUIStyle(EditorStyles.helpBox) {wordWrap = true, fontSize = 12};
            position.y += _padding;
            position.height -= _padding * 2;
            EditorGUI.HelpBox(position, attr.InfoText, (MessageType) attr.MessageType);
        }
    }
}