using UnityEngine;


namespace SOSXR.SeaShark
{
    public class PreviewAttribute : PropertyAttribute
    {
        public readonly int Height;


        public PreviewAttribute(int height = 100)
        {
            Height = height;
        }
    }
}