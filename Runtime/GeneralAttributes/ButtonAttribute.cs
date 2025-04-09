using System;


namespace SOSXR.SeaShark
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public ButtonAttribute(string label = null)
        {
            Label = label;
        }


        public string Label { get; }
    }
}