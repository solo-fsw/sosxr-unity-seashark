using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RequiredAttribute : PropertyAttribute
    {
        /// <summary>Applies to a List or array as a whole, not to each element.</summary>
        public RequiredAttribute() : base(true)
        {
        }
    }
}