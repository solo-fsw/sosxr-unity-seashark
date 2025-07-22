using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     From: https://github.com/adammyhre/Unity-Utils
    /// </summary>
    public static class Helpers
    {
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            return WaitFor.Seconds(seconds);
        }


        /// <summary>
        ///     Clears the console log in the Unity Editor.
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