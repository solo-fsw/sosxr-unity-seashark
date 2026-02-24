using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Formats time values in the Inspector.
    ///     When DisplayHours is true, hours are shown in the time representation.
    /// </summary>
    public class TimeAttribute : PropertyAttribute
    {
        /// <summary>Display hours alongside minutes and seconds when formatting time.</summary>
        public readonly bool DisplayHours;


        /// <summary>
        ///     Creates a new instance with the specified hour display option.
        /// </summary>
        /// <param name="displayHours">If true, show hours in the formatted time.</param>
        public TimeAttribute(bool displayHours = false)
        {
            DisplayHours = displayHours;
        }
    }
}
