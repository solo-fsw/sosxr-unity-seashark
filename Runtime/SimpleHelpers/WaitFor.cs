using System.Collections.Generic;
using UnityEngine;


namespace SOSXR.SeaShark
{
        /// <summary>
        ///     Utilities for awaiting coroutines with common Unity wait instructions.
        ///     From: https://github.com/adammyhre/Unity-Utils
        /// </summary>
    public static class WaitFor
    {
        public static WaitForFixedUpdate FixedUpdate { get; } = new();

        public static WaitForEndOfFrame EndOfFrame { get; } = new();

        private static readonly Dictionary<float, WaitForSeconds> WaitForSecondsDict = new(100, new FloatComparer());


        /// <summary>
        ///     Gets a cached WaitForSeconds instance for the specified duration.
        ///     Returns null if the duration would result in a zero frame wait.
        /// </summary>
        /// <param name="seconds">Duration in seconds to wait.</param>
        /// <returns>A cached WaitForSeconds instance or null when too short for a frame-based wait.</returns>
        public static WaitForSeconds Seconds(float seconds)
        {
            if (seconds < 1f / Application.targetFrameRate)
            {
                return null;
            }

            if (!WaitForSecondsDict.TryGetValue(seconds, out var forSeconds))
            {
                forSeconds = new WaitForSeconds(seconds);
                WaitForSecondsDict[seconds] = forSeconds;
            }

            return forSeconds;
        }


        private class FloatComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y)
            {
                return Mathf.Abs(x - y) <= Mathf.Epsilon;
            }


            public int GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
