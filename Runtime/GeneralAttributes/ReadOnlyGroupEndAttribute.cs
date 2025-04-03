using UnityEngine;


namespace SOSXR.SeaShark.Attributes
{
    /// <summary>
    ///     Use with <see cref="ReadOnlyGroupBeginAttribute" />.
    ///     Close the read-only group and resume editable fields.
    /// </summary>
    /// <seealso cref="ReadOnlyGroupBeginAttribute" />
    /// <seealso cref="ReadOnlyAttribute" />
    public class ReadOnlyGroupEndAttribute : PropertyAttribute
    {
    }
}