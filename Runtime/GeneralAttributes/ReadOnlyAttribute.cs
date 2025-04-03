using UnityEngine;


namespace SOSXR.SeaShark.Attributes
{
    /// <summary>
    ///     Display a field as read-only in the inspector.
    ///     CustomPropertyDrawers will not work when this attribute is used.
    /// </summary>
    /// <seealso cref="ReadOnlyGroupBeginAttribute" />
    /// <seealso cref="ReadOnlyGroupAttribute" />
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}