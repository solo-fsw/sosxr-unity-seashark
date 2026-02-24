using UnityEngine;
#if ENABLED_UNITY_MATHEMATICS
using Unity.Mathematics;
#endif


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     Numeric extension helpers.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        ///     Computes what percentage the part represents of the whole as a value between 0 and 1.
        /// </summary>
        /// <param name="part">Numerator value.</param>
        /// <param name="whole">Denominator value.</param>
        /// <returns>The fractional percentage of part with respect to whole. Returns 0 if whole is zero.</returns>
        public static float PercentageOf(this int part, int whole)
        {
            if (whole == 0)
            {
                return 0; // Handling division by zero
            }

            return (float) part / whole;
        }


        /// <summary>
        ///     Determines if two floats are approximately equal.
        /// </summary>
        /// <param name="f1">First value.</param>
        /// <param name="f2">Second value.</param>
        /// <returns>True if values are approximately equal.</returns>
        public static bool Approx(this float f1, float f2)
        {
            return Mathf.Approximately(f1, f2);
        }


        /// <summary>
        ///     Determines if an integer is odd.
        /// </summary>
        /// <param name="i">Value to test.</param>
        /// <returns>True if the value is odd.</returns>
        public static bool IsOdd(this int i)
        {
            return i % 2 == 1;
        }


        /// <summary>
        ///     Determines if an integer is even.
        /// </summary>
        /// <param name="i">Value to test.</param>
        /// <returns>True if the value is even.</returns>
        public static bool IsEven(this int i)
        {
            return i % 2 == 0;
        }


        /// <summary>
        ///     Clamps an integer to be at least the specified minimum.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="min">Minimum allowed value.</param>
        /// <returns>The greater of value and min.</returns>
        public static int AtLeast(this int value, int min)
        {
            return Mathf.Max(value, min);
        }


        /// <summary>
        ///     Clamps an integer to be at most the specified maximum.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="max">Maximum allowed value.</param>
        /// <returns>The smaller of value and max.</returns>
        public static int AtMost(this int value, int max)
        {
            return Mathf.Min(value, max);
        }


        /// <summary>
        ///     Clamps a float to be at least the specified minimum.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="min">Minimum allowed value.</param>
        /// <returns>The greater of value and min.</returns>
        public static float AtLeast(this float value, float min)
        {
            return Mathf.Max(value, min);
        }


        /// <summary>
        ///     Clamps a float to be at most the specified maximum.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="max">Maximum allowed value.</param>
        /// <returns>The smaller of value and max.</returns>
        public static float AtMost(this float value, float max)
        {
            return Mathf.Min(value, max);
        }


        /// <summary>
        ///     Clamps a double to be at least the specified minimum.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="min">Minimum allowed value.</param>
        /// <returns>The greater of value and min.</returns>
        public static double AtLeast(this double value, double min)
        {
            return MathfExtension.Max(value, min);
        }


        /// <summary>
        ///     Clamps a double to be at most the specified maximum.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="min">Maximum allowed value (naming mirrors existing signature).</param>
        /// <returns>The smaller of value and min.</returns>
        public static double AtMost(this double value, double min)
        {
            return MathfExtension.Min(value, min);
        }


        #if ENABLED_UNITY_MATHEMATICS
        public static half AtLeast(this half value, half max)
        {
            return MathfExtension.Max(value, max);
        }


        public static half AtMost(this half value, half max)
        {
            return MathfExtension.Min(value, max);
        }
#endif
    }
}
