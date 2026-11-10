using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark.EditorScripts
{
    /// <summary>
    ///     Finds [Info] attributes on fields that Unity does not serialize (e.g. plain private or static fields).
    ///     Unity never draws those fields, so their InfoDrawer never runs: SOSXRBaseEditor draws them instead.
    /// </summary>
    internal static class InfoFieldCollector
    {
        internal static List<(string Label, InfoAttribute Info)> GetUnserializedFieldInfos(SerializedObject serializedObject)
        {
            var result = new List<(string, InfoAttribute)>();

            if (serializedObject == null || serializedObject.targetObject == null)
            {
                return result;
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

            // Walk base classes too: private fields of a base class are not returned by GetFields on the derived type.
            // Collect from the base class down, so the order matches the default inspector.
            var types = new List<Type>();

            for (var type = serializedObject.targetObject.GetType(); type != null && type != typeof(Object); type = type.BaseType)
            {
                types.Insert(0, type);
            }

            foreach (var type in types)
            {
                foreach (var field in type.GetFields(flags))
                {
                    var infos = (InfoAttribute[]) field.GetCustomAttributes(typeof(InfoAttribute), true);

                    if (infos.Length == 0)
                    {
                        continue;
                    }

                    // Let Unity decide what is serialized: if it has a property, the InfoDrawer already handles it.
                    if (!field.IsStatic && serializedObject.FindProperty(field.Name) != null)
                    {
                        continue;
                    }

                    var label = ObjectNames.NicifyVariableName(field.Name);

                    foreach (var info in infos)
                    {
                        result.Add((label, info));
                    }
                }
            }

            return result;
        }
    }
}
