using System.Reflection;
using UnityEngine;


namespace SOSXR.SeaShark.Editor
{
    /// <summary>
    ///     By using this as a base, you can keep the Button functionality in one place, and use it in multiple Editors.
    ///     Those editors should inherit from this class,  with the type of the object they are editing.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SOSXRBaseEditor<T> : UnityEditor.Editor where T : Object
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var targetType = target.GetType();
            var methods = targetType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                if (buttonAttribute == null)
                {
                    continue;
                }

                if (GUILayout.Button(buttonAttribute.Label ?? method.Name))
                {
                    method.Invoke(target, null);
                }
            }
        }
    }
}