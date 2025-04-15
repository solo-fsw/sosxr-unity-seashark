using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    /// <summary>
    ///     Drawer for the RequireInterface attribute.
    ///     It works by setting [SerializeReference, Interface(typeof(---))] next to your interface in the
    ///     inspector. This then allows a drag/drop of any object that implements the interface. It is best to use
    ///     UnityEngine.Object as the type, but MonoBehaviour, Component, and ScriptableObject can be used as well.
    ///     Then create a property getter that casts the object to the interface type, which is the one you use in the script
    ///     to refer to.
    ///     Example:
    ///     [SerializeReference] [Interface(typeof(IAmTheNameOfTheInterface))] private Object m_interfaceObject;
    ///     public IAmTheNameOfTheInterface InterfaceObject => m_interfaceObject as IAmTheNameOfTheInterface;
    ///     From: https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/
    /// </summary>
    [CustomPropertyDrawer(typeof(InterfaceAttribute))]
    public class InterfaceDrawer : PropertyDrawer
    {
        /// <summary>
        ///     Overrides GUI drawing for the attribute.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="property">Property.</param>
        /// <param name="label">Label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check if this is reference type property.
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                // Get attribute parameters.
                var requiredAttribute = attribute as InterfaceAttribute;

                EditorGUI.BeginProperty(position, label, property);

                if (requiredAttribute != null)
                {
                    var reference = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.requiredType, true);

                    if (reference is null)
                    {
                        var obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Object), true);

                        if (obj is GameObject g)
                        {
                            reference = g.GetComponent(requiredAttribute.requiredType);
                        }
                    }

                    property.objectReferenceValue = reference;
                }

                EditorGUI.EndProperty();
            }
            else
            {
                // If field is not reference, show error message.
                // Save previous color and change GUI to red.
                var previousColor = GUI.color;
                GUI.color = Color.red;

                // Display label with error message.
                EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));

                // Revert color change.
                GUI.color = previousColor;
            }
        }
    }
}