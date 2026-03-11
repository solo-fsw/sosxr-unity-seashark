#define SOSXR_EDITORTOOLS_INSTALLED
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Provides a dropdown selector for tags in the Inspector.
    /// </summary>
    public class TagSelectorAttribute : PropertyAttribute
    {
        /// <summary>If true, use the default Unity tag field drawer instead of a custom one.</summary>
        public bool UseDefaultTagFieldDrawer = false;
    }
}
