using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    /// General helper utilities for common tasks.
    /// Source: https://github.com/adammyhre/Unity-Utils
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Gets a cached WaitForSeconds instance for the specified duration.
        /// </summary>
        /// <param name="seconds">The duration in seconds to wait.</param>
        /// <returns>A WaitForSeconds instance for use in coroutines.</returns>
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            return WaitFor.Seconds(seconds);
        }


        /// <summary>
        /// Clears the console log in the Unity Editor.
        /// This method is only available in the editor and uses reflection to access internal Unity APIs.
        /// </summary
        #if UNITY_EDITOR
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
        #endif
    }
}