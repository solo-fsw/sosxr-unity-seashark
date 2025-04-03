using UnityEngine;


namespace SOSXR.SeaShark.Attributes
{
    public class SuffixAttribute : PropertyAttribute
    {
        public readonly string Suffix;


        public SuffixAttribute(string suffix)
        {
            Suffix = suffix;
        }
    }
}