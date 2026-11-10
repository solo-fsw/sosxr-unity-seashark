using System;
using System.Collections;
using System.Reflection;
using UnityEditor;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Reflection helpers shared by drawers that need the C# object behind a SerializedProperty.
    /// </summary>
    internal static class SerializedPropertyUtility
    {
        /// <summary>
        ///     Returns the object that declares the property's field: the target object for a top-level field,
        ///     or the nested class / list element for a field inside one (e.g. "list.Array.data[0].value").
        ///     Returns null when the path cannot be resolved.
        /// </summary>
        internal static object GetParentObject(SerializedProperty property)
        {
            var path = property.propertyPath;
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');

            for (var i = 0; i < elements.Length - 1; i++)
            {
                var element = elements[i];

                if (element == "Array" && i + 1 < elements.Length && elements[i + 1].StartsWith("data["))
                {
                    var index = int.Parse(elements[i + 1].Replace("data[", "").Replace("]", ""));

                    if (obj is not IList list || index < 0 || index >= list.Count)
                    {
                        return null;
                    }

                    obj = list[index];
                    i++;
                }
                else
                {
                    var field = FindField(obj?.GetType(), element);

                    if (field == null)
                    {
                        return null;
                    }

                    obj = field.GetValue(obj);
                }
            }

            return obj;
        }


        /// <summary>
        ///     Finds an instance field by name, including private fields declared on base classes.
        /// </summary>
        internal static FieldInfo FindField(Type type, string name)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            for (var current = type; current != null; current = current.BaseType)
            {
                var field = current.GetField(name, flags);

                if (field != null)
                {
                    return field;
                }
            }

            return null;
        }
    }
}
