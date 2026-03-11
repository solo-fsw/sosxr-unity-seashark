using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Constrains a numeric field to a inclusive range in the Inspector.
    ///     The value is clamped between Min and Max when edited in the Inspector.
    /// </summary>
    public class BoxRangeAttribute : PropertyAttribute
    {
        /// <summary>Minimum value of the allowed range.</summary>
        public readonly float Min;
        /// <summary>Maximum value of the allowed range.</summary>
        public readonly float Max;


        /// <summary>
        ///     Creates a new instance with the specified minimum and maximum range.
        /// </summary>
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        public BoxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
