using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From: https://docs.unity3d.com/ScriptReference/PropertyDrawer.html
    /// </summary>
    public class BoxRangeAttribute : PropertyAttribute
    {
        public readonly float Max;
        public readonly float Min;


        public BoxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}