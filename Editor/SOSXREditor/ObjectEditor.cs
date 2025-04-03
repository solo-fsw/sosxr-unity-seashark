using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    /// <summary>
    ///     Default Editor for all Objects.
    ///     It allows for the use of the ButtonAttribute.
    /// </summary>
    [CustomEditor(typeof(Object), true)]
    public class ObjectEditor : SOSXRBaseEditor<Object>
    {
    }
}