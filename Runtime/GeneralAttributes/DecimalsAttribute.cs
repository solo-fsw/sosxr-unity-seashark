using UnityEngine;


namespace SOSXR.SeaShark
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