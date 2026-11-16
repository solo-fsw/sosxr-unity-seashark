using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Default Editor for all Objects.
    ///     It allows for the use of the ButtonAttribute.
    ///     Builds a UI Toolkit inspector, so UI Toolkit-only PropertyDrawers keep working; the buttons are drawn in an IMGUIContainer.
    /// </summary>
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class ObjectEditor : SOSXRBaseEditor<Object>
    {
        /// <summary>
        ///     Builds the default UI Toolkit inspector and adds an IMGUI container for [Button] and [Info] drawing when there is custom GUI to show.
        /// </summary>
        /// <returns>The root VisualElement of the inspector.</returns>
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            // Only add the IMGUI part when there is something to draw.
            if (HasCustomGUI)
            {
                root.Add(new IMGUIContainer(DrawCustomGUI));
            }

            return root;
        }
    }
}
