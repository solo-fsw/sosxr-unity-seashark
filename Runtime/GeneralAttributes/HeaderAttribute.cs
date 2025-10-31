using System;
using UnityEngine;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     <para>Use this PropertyAttribute to add a header above some fields in the Inspector.</para>
    ///     Enhanced version of Unity's HeaderAttribute.
    ///     By Warped Imagination - https://www.youtube.com/watch?v=hGKpZssiN9g&ab_channel=WarpedImagination
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class HeaderAttribute : PropertyAttribute
    {
        /// <summary>
        ///     <para>The header text.</para>
        /// </summary>
        public readonly string header;


        /// <summary>
        ///     <para>Add a header above some fields in the Inspector.</para>
        /// </summary>
        /// <param name="header">The header text.</param>
        public HeaderAttribute(string header)
            : base(true)
        {
            this.header = header;
        }
    }
}