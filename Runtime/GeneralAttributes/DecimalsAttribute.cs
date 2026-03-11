using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Specifies the number of decimal places displayed for a floating-point field in the Inspector.
    /// </summary>
    public class DecimalsAttribute : PropertyAttribute
    {
        /// <summary>Number of decimals to display.</summary>
        public readonly int Decimals;


        /// <summary>
        ///     Creates a new instance with the desired decimal precision.
        /// </summary>
        /// <param name="decimals">Decimal places to display.</param>
        public DecimalsAttribute(int decimals)
        {
            Decimals = decimals;
        }
    }
}
