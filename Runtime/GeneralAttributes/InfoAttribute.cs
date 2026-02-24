using System;
using System.Diagnostics;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Displays informational text in the Inspector with an optional message style.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class InfoAttribute : PropertyAttribute
    {
        /// <summary>Info text to display in the Inspector.</summary>
        public readonly string InfoText;


        /// <summary>
        ///     Creates a new instance with the specified text.
        /// </summary>
        /// <param name="infoText">Text to display.</param>
        public InfoAttribute(string infoText)
        {
            InfoText = infoText;
        }


        /// <summary>Visual style for the message.</summary>
        public SOSXRMessageType MessageType { get; set; } = SOSXRMessageType.None;
    }


    /// <summary>
    ///     This is a copy of the UnityEditor.MessageType enum.
    ///     Since that is in the UnityEditor namespace, you can't use it in a build.
    ///     However, if we use this here, and cast it's int to the UnityEditor.MessageType enum, we can use it in a build.
    ///     Many thanks to Warped Imagination: https://youtu.be/533OH2m7fNg?si=hfyYb2p9s5WBl1vP for this helpful tip
    /// </summary>
    public enum SOSXRMessageType
    {
        /// <summary>
        ///     <para>Neutral message.</para>
        /// </summary>
        None,

        /// <summary>
        ///     <para>Info message.</para>
        /// </summary>
        Info,

        /// <summary>
        ///     <para>Warning message.</para>
        /// </summary>
        Warning,

        /// <summary>
        ///     <para>Error message.</para>
        /// </summary>
        Error
    }
}
