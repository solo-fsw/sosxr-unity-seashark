using System;
using UnityEngine;


namespace SOSXR.SeaShark.Attributes
{
    /// <summary>
    ///     Attribute that require implementation of the provided interface.
    ///     From: https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/
    /// </summary>
    public class InterfaceAttribute : PropertyAttribute
    {
        /// <summary>
        ///     Requiring implementation of the <see cref="T:InterfaceAttribute" /> interface.
        /// </summary>
        /// <param name="type">Interface type.</param>
        public InterfaceAttribute(Type type)
        {
            requiredType = type;
        }


        // Interface type.
        public Type requiredType { get; private set; }
    }
}