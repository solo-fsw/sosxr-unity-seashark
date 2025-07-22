using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyGroupBeginAttribute))]
    public class ReadOnlyGroupBeginDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }


        public override void OnGUI(Rect position)
        {
            EditorGUI.BeginDisabledGroup(true);
        }
    }
}