using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Usage:
    ///     [SerializeField, HideInInspector] private bool m_toggleWithThisBool;
    ///     [EnableIf(nameof(m_toggleWithThisBool))]
    ///     [SerializeField] private string m_disableOrEnableThisField;
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    /// <summary>
    ///     Conditionally enables a field in the Inspector based on another field's value.
    /// </summary>
    public class EnableIfAttribute : PropertyAttribute
    {
        /// <summary>Name of the field that controls enabled state.</summary>
        public readonly string ConditionField;


        public EnableIfAttribute(string conditionField)
        {
            ConditionField = conditionField;
        }
    }
}
