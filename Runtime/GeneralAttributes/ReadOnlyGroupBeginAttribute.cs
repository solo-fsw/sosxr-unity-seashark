using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Marks the start of a read-only group in the Inspector.
    ///     All following fields until the corresponding ReadOnlyGroupEndAttribute
    ///     are shown as read-only.
    /// </summary>
    /// <seealso cref="ReadOnlyGroupAttribute" />
    /// <seealso cref="ReadOnlyAttribute" />
    public class ReadOnlyGroupBeginAttribute : PropertyAttribute
    {
    }
}
