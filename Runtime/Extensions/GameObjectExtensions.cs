using System.Linq;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Extension methods for common GameObject operations.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        ///     Hides the GameObject from the Hierarchy view in the editor.
        /// </summary>
        /// <param name="gameObject">The GameObject to hide in the hierarchy.</param>
        public static void HideInHierarchy(this GameObject gameObject)
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }


        /// <summary>
        ///     Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds
        ///     one.
        /// </summary>
        /// <typeparam name="T">The type of the component to get or add.</typeparam>
        /// <param name="gameObject">The GameObject to operate on.</param>
        /// <remarks>
        ///     This method is useful when you don't know if a GameObject has a specific type of component,
        ///     but you want to work with that component regardless. Instead of checking and adding the component manually,
        ///     you can use this method to do both operations in one line.
        /// </remarks>
        
        /// <returns>The existing component of the given type, or a new one if no such component exists.</returns>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();

            if (!component)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }


        /// <summary>
        ///     Returns the object itself if it exists, null otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object being checked.</param>
        /// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
        public static T OrNull<T>(this T obj) where T : Object
        {
            return obj ? obj : null;
        }


        /// <summary>
        ///     Destroys all children of the GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildren(this GameObject gameObject)
        {
            TransformExtensions.DestroyChildren(gameObject.transform);
        }


        /// <summary>
        ///     Immediately destroys all children of the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose children are to be destroyed.</param>
        public static void DestroyChildrenImmediate(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildrenImmediate();
        }


        /// <summary>
        ///     Enables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be enabled.</param>
        public static void EnableChildren(this GameObject gameObject)
        {
            gameObject.transform.EnableChildren();
        }


        /// <summary>
        ///     Disables all child GameObjects associated with the given GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject whose child GameObjects are to be disabled.</param>
        public static void DisableChildren(this GameObject gameObject)
        {
            gameObject.transform.DisableChildren();
        }


        /// <summary>
        ///     Resets the GameObject's transform's position, rotation, and scale to their default values.
        /// </summary>
        /// <param name="gameObject">GameObject whose transformation is to be reset.</param>
        public static void ResetTransformation(this GameObject gameObject)
        {
            gameObject.transform.Reset();
        }


        /// <summary>
        ///     Returns the hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>
        ///     A '/'-separated string representing the full hierarchical path of this GameObject in the scene.
        /// </returns>
        public static string Path(this GameObject gameObject)
        {
            return "/" + string.Join("/",
                gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
        }


        /// <summary>
        ///     Returns the full hierarchical path in the Unity scene hierarchy for this GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to get the path for.</param>
        /// <returns>
        ///     A string representing the full hierarchical path of this GameObject in the Unity scene.
        /// </returns>
        public static string PathFull(this GameObject gameObject)
        {
            return gameObject.Path() + "/" + gameObject.name;
        }


        /// <summary>
        ///     Recursively sets the provided layer for this GameObject and all of its descendants in the Unity scene hierarchy.
        /// </summary>
        /// <param name="gameObject">The GameObject to set layers for.</param>
        /// <param name="layer">The layer number to set for GameObject and all of its descendants.</param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            gameObject.transform.ForEveryChild(child => SetLayersRecursively(child.gameObject, layer));
        }
    }
}
