using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Display one or more fields as read-only in the inspector.
    ///     Use <see cref="ReadOnlyGroupAttribute" /> to close the group.
    ///     Works with CustomPropertyDrawers.
    /// </summary>
    /// <seealso cref="ReadOnlyGroupAttribute" />
    /// <seealso cref="ReadOnlyAttribute" />
    public class ReadOnlyGroupBeginAttribute : PropertyAttribute
    {
    }
}