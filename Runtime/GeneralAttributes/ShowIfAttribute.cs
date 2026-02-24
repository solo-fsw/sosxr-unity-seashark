using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Conditionally shows a field in the Inspector based on another field's value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        /// <summary>Name of the field that controls visibility.</summary>
        public readonly string ConditionField;


        public ShowIfAttribute(string conditionField)
        {
            ConditionField = conditionField;
        }
    }
}
