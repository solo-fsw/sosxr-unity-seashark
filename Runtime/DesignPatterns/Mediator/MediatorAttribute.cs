using System;
using UnityEngine;


namespace SOSXR.SeaShark.Patterns.Mediator
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MediatorAttribute : PropertyAttribute
    {
        public MediatorAttribute(bool editableChannel = false)
        {
            EditableChannel = editableChannel;
        }


        public bool EditableChannel { get; }
    }
}