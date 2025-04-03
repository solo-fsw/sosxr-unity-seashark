using UnityEngine;


namespace SOSXR.SeaShark.Attributes
{
    public class DecimalsAttribute : PropertyAttribute
    {
        public readonly int Decimals;


        public DecimalsAttribute(int decimals)
        {
            Decimals = decimals;
        }
    }
}