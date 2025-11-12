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
    public class EnableIfAttribute : PropertyAttribute
    {
        public readonly string ConditionField;


        public EnableIfAttribute(string conditionField)
        {
            ConditionField = conditionField;
        }
    }
}