using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Use with <see cref="ReadOnlyGroupBeginAttribute" />.
    ///     Closes the read-only group and resumes editable fields.
    /// </summary>
    /// <seealso cref="ReadOnlyGroupBeginAttribute" />
    /// <seealso cref="ReadOnlyAttribute" />
    public class ReadOnlyGroupEndAttribute : PropertyAttribute
    {
        /// <summary>Applies to a List or array as a whole, not to each element.</summary>
        public ReadOnlyGroupEndAttribute() : base(true)
        {
        }
    }
}
