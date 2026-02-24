using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Shows a visual preview of an asset in the Inspector.
    /// </summary>
    public class PreviewAttribute : PropertyAttribute
    {
        /// <summary>Height of the preview in pixels.</summary>
        public readonly int Height;


        /// <summary>
        ///     Creates a new instance with the specified preview height.
        /// </summary>
        /// <param name="height">Preview height in pixels.</param>
        public PreviewAttribute(int height = 100)
        {
            Height = height;
        }
    }
}
