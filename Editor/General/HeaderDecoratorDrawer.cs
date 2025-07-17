using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    /// <summary>
    ///     <para>Use this PropertyAttribute to add a header above some fields in the Inspector.</para>
    ///     Enhanced version of Unity's HeaderAttribute.
    ///     By Warped Imagination - https://www.youtube.com/watch?v=hGKpZssiN9g&ab_channel=WarpedImagination
    ///     It can usually fallback to Unity's HeaderAttribute, but this one allows for more customization.
    ///     If it complains about ambiguity, you can use the full call `SOSXR.SeaShark.Header`, or better yet, use the `using Header = SOSXR.SeaShark.HeaderAttribute;` directive at the top of your script.
    /// </summary>
    [CustomPropertyDrawer(typeof(HeaderAttribute), true)]
    public class HeaderDecoratorDrawer : DecoratorDrawer
    {
        private readonly int _size = 13;


        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }


        public override void OnGUI(Rect position)
        {
            position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;

            var style = new GUIStyle(EditorStyles.boldLabel);

            style.richText = true;

            var label = new GUIContent($"<color=lightblue><size={_size}>{(attribute as HeaderAttribute)?.header}</size></color>");

            GUI.Label(position, label, style);
        }
    }
}