using UnityEngine;


namespace SOSXR.SeaShark.Attributes
{
    public class TimeAttribute : PropertyAttribute
    {
        public readonly bool DisplayHours;


        public TimeAttribute(bool displayHours = false)
        {
            DisplayHours = displayHours;
        }
    }
}