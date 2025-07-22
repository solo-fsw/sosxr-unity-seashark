using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyGroupEndAttribute))]
    public class ReadOnlyGroupEndDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }


        public override void OnGUI(Rect position)
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}