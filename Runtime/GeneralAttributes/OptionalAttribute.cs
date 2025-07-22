using UnityEngine;


namespace SOSXR.SeaShark
{

    /// <summary>
    /// Use this attribute to mark a field as optional in the inspector.
    /// It does not affect the functionality of the field, but it can be used to indicate that the field is not required.
    /// </summary>
    public class OptionalAttribute : PropertyAttribute
    {
        public OptionalType Type { get; private set; } = OptionalType.Optional;

        public OptionalAttribute() { }

        public OptionalAttribute(OptionalType type)
        {
            Type = type;
        }
    }

    public enum OptionalType
    {
        WillAdd,
        WillGet,
        WillFind,
        Optional,
    }
}