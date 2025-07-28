using SOSXR.SeaShark.EditorScripts;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Samples.EditorScripts
{
    /// <summary>
    ///     Example class that shows you can still create custom Editors, whilst retaining the Button functionality.
    /// </summary>
    [CustomEditor(typeof(ButtonExampleClass), true)]
    public class ButtonClassExampleEditor : SOSXRBaseEditor<ButtonExampleClass>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI(); // Always call this first

            // Add custom editor code below
        }
    }


    #if EDITORSPICE



    namespace SOSXR.SeaShark.Samples.EditorScripts
    {
    }
    #endif

}