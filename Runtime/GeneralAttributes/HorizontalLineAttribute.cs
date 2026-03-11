using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Draws a horizontal line in the Inspector to visually separate sections.
    /// </summary>
    public class HorizontalLineAttribute : PropertyAttribute
    {
        /// <summary>Thickness of the line in pixels.</summary>
        public float Thickness { get; set; } = 2.5f;

        /// <summary>Vertical padding around the line in pixels.</summary>
        public float Padding { get; set; } = 12.5f;
    }
}
