using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Displays a field as read-only in the Inspector.
    ///     CustomPropertyDrawers will not work when this attribute is used.
     /// </summary>
    /// <seealso cref="ReadOnlyGroupBeginAttribute" />
    /// <seealso cref="ReadOnlyGroupAttribute" />
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}
