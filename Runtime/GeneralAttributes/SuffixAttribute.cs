using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Adds a suffix label to a field in the Inspector.
    /// </summary>
    public class SuffixAttribute : PropertyAttribute
    {
        /// <summary>Suffix text rendered after the field's value.</summary>
        public readonly string Suffix;


        public SuffixAttribute(string suffix)
        {
            Suffix = suffix;
        }
    }
}
