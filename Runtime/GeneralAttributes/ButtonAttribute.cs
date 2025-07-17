using System;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Uses the SOSXRBaseEditor.cs
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public ButtonAttribute(string itemName = null, string tooltip = null)
        {
            ItemName = itemName;
            Tooltip = tooltip;
        }


        public string Tooltip { get; set; }
        public string ItemName { get; set; }
    }
}