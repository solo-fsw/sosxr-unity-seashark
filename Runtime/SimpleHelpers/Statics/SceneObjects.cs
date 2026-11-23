using System;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Finds loaded objects without sorting, on every Unity 6 version this package supports.
    ///     Unity 6000.4 and newer have <c>FindObjectsByType</c> overloads without a sort mode and mark the
    ///     <see cref="FindObjectsSortMode" /> overloads obsolete. Older versions only have the sort-mode overloads.
    /// </summary>
    public static class SceneObjects
    {
        /// <summary>Returns all loaded objects of type <typeparamref name="T" />, unsorted.</summary>
        public static T[] FindAll<T>(FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude) where T : Object
        {
#if UNITY_6000_4_OR_NEWER
            return Object.FindObjectsByType<T>(findObjectsInactive);
#else
            return Object.FindObjectsByType<T>(findObjectsInactive, FindObjectsSortMode.None);
#endif
        }


        /// <summary>Returns all loaded objects of <paramref name="type" />, unsorted.</summary>
        public static Object[] FindAll(Type type, FindObjectsInactive findObjectsInactive = FindObjectsInactive.Exclude)
        {
#if UNITY_6000_4_OR_NEWER
            return Object.FindObjectsByType(type, findObjectsInactive);
#else
            return Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.None);
#endif
        }
    }
}
