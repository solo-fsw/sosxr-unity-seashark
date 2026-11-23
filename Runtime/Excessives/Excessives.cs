using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;


namespace SOSXR.SeaShark.Excessives
{
    /// <summary>
    /// Math helpers for clamping, wrapping, rounding, interpolation, degree/radian conversion and physical constants.
    /// </summary>
    public static class MathE
    {
        #region Clamp

        /// <summary>
        /// Clamps a value to the inclusive range [min, max].
        /// </summary>
        /// <typeparam name="T">A comparable type.</typeparam>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <returns>The value limited to [min, max].</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            // Ensure value lies within [min, max].
            return value.CompareTo(max) > 0 ? max : value.CompareTo(min) < 0 ? min : value;
        }

        #endregion

        #region Derivatives Of Position

        /// <summary>
        /// Approximates the displacement after a time step from velocity, acceleration, jerk and jounce, using a Taylor expansion up to the 4th derivative.
        /// </summary>
        /// <param name="velocity">First derivative of position.</param>
        /// <param name="acceleration">Second derivative of position.</param>
        /// <param name="jerk">Third derivative of position.</param>
        /// <param name="jounce">Fourth derivative of position.</param>
        /// <param name="time">The time step.</param>
        /// <returns>The approximate displacement over the time step.</returns>
        public static double PosFromDerivatives(
            double velocity, double acceleration,
            double jerk, double jounce,
            double time)
        {
            // Integrate motion from velocity/acceleration/jerk/jounce over a small time step.
            // The terms correspond to kinematic expansion up to the 4th derivative (snap).
            return //This is why I love the order of operations
                velocity * time
                +
                acceleration * time * time / 2
                +
                jerk * time * time * time / 6
                +
                jounce * time * time * time * time / 24;
        }

        #endregion

        #region Constants

        /// <summary>
        /// The golden ratio.
        /// </summary>
        public const double GOLDENRATIO = 1.6180339887498948482; // Classic constant used for pacing or pseudo-randomness heuristics

        /// <summary>
        /// The plastic number, the real root of x^3 = x + 1.
        /// </summary>
        public const double PLASTICNUMBER = 1.32471795724474602596;

        /// <summary>
        /// Tau, equal to 2 * PI.
        /// </summary>
        public const double TAU = 6.283185307179586; // 2 * PI

        /// <summary>
        /// Speed of light in metres per second.
        /// </summary>
        public const double SPEEDOFLIGHT = 299792458;

        /// <summary>
        ///     In metres
        /// </summary>
        public const double PLANCKLENGTH = 1.61622938e-35;

        /// <summary>
        ///     In seconds
        /// </summary>
        public const double PLANCKTIME = 5.3911613e-44;

        /// <summary>
        ///     Note that this is in Kelvin
        /// </summary>
        public const double PLANCKTEMPERATURE = 1.41680833e32;

        /// <summary>
        ///     In kilograms
        /// </summary>
        public const double PLANCKMASS = 2.176434e-8;

        /// <summary>
        ///     In m^3 kg^-1 s^-2
        /// </summary>
        public const double GRAVITATIONALCONSTANT = 6.6740831e-11;

        /// <summary>
        ///     In J s
        /// </summary>
        public const double PLANCKSCONSTANT = 6.62607015e-34;

        /// <summary>
        ///     One light year in metres
        /// </summary>
        public const double LIGHTYEAR = 9460730472580800;

        #endregion

        #region Add Towards

        #region Overflow

        // Adds value2 towards target with overflow-style behavior: sign(target - value1) guides direction
        /// <summary>
        /// Adds value2 in the direction of target from value1, allowing a single step to overshoot the target.
        /// </summary>
        /// <param name="value1">The starting value.</param>
        /// <param name="value2">The step size.</param>
        /// <param name="target">The value to move towards.</param>
        /// <returns>The stepped value, which may pass the target.</returns>
        public static float AddTowardsOverFlow(
            float value1, float value2, float target)
        {
            // Move towards target, allowing overshoot in a single step and relying on sign to pick direction.
            return value1 + value2 * Math.Sign(target - value1);
        }

        #endregion

        #region Obstructed

        // Obstructed variant clamps progress to the target if crossing it would overshoot in an intermediate step.
        /// <summary>
        /// Adds value2 towards target from value1, clamping the result so it never passes the target.
        /// </summary>
        /// <param name="value1">The starting value.</param>
        /// <param name="value2">The step size.</param>
        /// <param name="target">The value to move towards.</param>
        /// <returns>The stepped value, limited to the target.</returns>
        public static float AddTowardsObstructed(
            float value1, float value2, float target)
        {
            var v = value1 + value2 * Math.Sign(target - value1);

            return
                value1 < target //If we were firstly underneath the target
                    ? //Underneath
                    v > target //v is larger than the target
                        ? target //Clamp to target
                        : v //Else return v
                    : //Not firstly underneath
                    value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target //Clamp to target
                            : v //Else return v
                        : //Not above and not below, must be exactly equal to target
                        target;
        }


        /// <summary>
        /// Adds value2 towards target from value1, clamping the result so it never passes the target.
        /// </summary>
        /// <param name="value1">The starting value.</param>
        /// <param name="value2">The step size.</param>
        /// <param name="target">The value to move towards.</param>
        /// <returns>The stepped value, limited to the target.</returns>
        public static int AddTowardsObstructed(
            int value1, int value2, int target)
        {
            var v = value1 + value2 * Math.Sign(target - value1);

            return
                value1 < target //If we were firstly underneath the target
                    ? //Underneath
                    v > target //v is larger than the target
                        ? target //Clamp to target
                        : v //Else return v
                    : //Not firstly underneath
                    value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target //Clamp to target
                            : v //Else return v
                        : //Not above and not below, must be exactly equal to target
                        target;
        }


        /// <summary>
        /// Adds value2 towards target from value1, clamping the result so it never passes the target.
        /// </summary>
        /// <param name="value1">The starting value.</param>
        /// <param name="value2">The step size.</param>
        /// <param name="target">The value to move towards.</param>
        /// <returns>The stepped value, limited to the target.</returns>
        public static double AddTowardsObstructed(
            double value1, double value2, double target)
        {
            var v = value1 + value2 * Math.Sign(target - value1);

            return
                value1 < target //If we were firstly underneath the target
                    ? //Underneath
                    v > target //v is larger than the target
                        ? target //Clamp to target
                        : v //Else return v
                    : //Not firstly underneath
                    value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target //Clamp to target
                            : v //Else return v
                        : //Not above and not below, must be exactly equal to target
                        target;
        }

        #endregion

        #region Rebound

        // Rebound variant: when the next step would cross the target, reflect the excess off the target to simulate bouncing.
        /// <summary>
        /// Adds value2 towards target from value1. If the step would cross the target, the excess is reflected back off it.
        /// </summary>
        /// <param name="value1">The starting value.</param>
        /// <param name="value2">The step size.</param>
        /// <param name="target">The value to move towards.</param>
        /// <returns>The stepped value, reflected off the target when crossed.</returns>
        public static float AddTowardsRebound(
            float value1, float value2, float target)
        {
            var v = value1 + value2 * Math.Sign(target - value1);

            var offcut = Math.Abs(v - value1);

            return
                value1 < target //If we were firstly underneath the target
                    ? //Underneath
                    v > target //v is larger than the target
                        ? target - offcut //Rebound from target
                        : v //Else return v
                    : //Not firstly underneath
                    value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target + offcut //Rebound from target
                            : v //Else return v
                        : //Not above and not below, must be exactly equal to target
                        target;
        }


        /// <summary>
        /// Adds value2 towards target from value1. If the step would cross the target, the excess is reflected back off it.
        /// </summary>
        /// <param name="value1">The starting value.</param>
        /// <param name="value2">The step size.</param>
        /// <param name="target">The value to move towards.</param>
        /// <returns>The stepped value, reflected off the target when crossed.</returns>
        public static int AddTowardsRebound(
            int value1, int value2, int target)
        {
            var v = value1 + value2 * Math.Sign(target - value1);

            var offcut = Math.Abs(v - value1);

            return
                value1 < target //If we were firstly underneath the target
                    ? //Underneath
                    v > target //v is larger than the target
                        ? target - offcut //Rebound from target
                        : v //Else return v
                    : //Not firstly underneath
                    value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target + offcut //Rebound from target
                            : v //Else return v
                        : //Not above and not below, must be exactly equal to target
                        target;
        }


        // Double precision version of the rebound variant
        /// <summary>
        /// Adds value2 towards target from value1. If the step would cross the target, the excess is reflected back off it.
        /// </summary>
        /// <param name="value1">The starting value.</param>
        /// <param name="value2">The step size.</param>
        /// <param name="target">The value to move towards.</param>
        /// <returns>The stepped value, reflected off the target when crossed.</returns>
        public static double AddTowardsRebound(
            double value1, double value2, double target)
        {
            var v = value1 + value2 * Math.Sign(target - value1);

            var offcut = Math.Abs(v - value1);

            return
                value1 < target //If we were firstly underneath the target
                    ? //Underneath
                    v > target //v is larger than the target
                        ? target - offcut //Rebound from target
                        : v //Else return v
                    : //Not firstly underneath
                    value1 > target //If firstly above
                        ? v < target //If smaller than target
                            ? target + offcut //Rebound from target
                            : v //Else return v
                        : //Not above and not below, must be exactly equal to target
                        target;
        }

        #endregion

        #endregion

        #region ClampWrap

        //Useful for clamping angles
        // Wrap a value into the inclusive range [min, max], useful for angle normalization
        /// <summary>
        /// Wraps a value into the range [min, max), for example to normalise angles. Returns min when the range is zero.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <returns>The wrapped value.</returns>
        public static float ClampWrap(float value, float min, float max)
        {
            var range = max - min;

            if (range == 0f)
            {
                return min;
            }

            // Double modulo keeps the result in range for values below min too (C# % keeps the dividend's sign).
            return ((value - min) % range + range) % range + min; // wrap around when exceeding bounds
        }

        /// <summary>
        /// Wraps a value into the range [min, max), for example to normalise angles. Returns min when the range is zero.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <returns>The wrapped value.</returns>
        public static double ClampWrap(double value, double min, double max)
        {
            var range = max - min;

            if (range == 0d)
            {
                return min;
            }

            return ((value - min) % range + range) % range + min; // wrap value into range
        }


        /// <summary>
        /// Wraps an integer into the range [min, max), for example to normalise angles. Returns min when the range is zero.
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The lower bound.</param>
        /// <param name="max">The upper bound.</param>
        /// <returns>The wrapped value.</returns>
        public static int ClampWrap(int value, int min, int max)
        {
            var range = max - min;

            if (range == 0)
            {
                return min;
            }

            return ((value - min) % range + range) % range + min; // wrap for integers
        }

        #endregion

        #region ReMap

        // Maps a value from [inMin, inMax] into [outMin, outMax].
        /// <summary>
        /// Linearly maps a value from the range [inMin, inMax] into [outMin, outMax]. Does not clamp.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="inMin">Lower bound of the input range.</param>
        /// <param name="inMax">Upper bound of the input range.</param>
        /// <param name="outMin">Lower bound of the output range.</param>
        /// <param name="outMax">Upper bound of the output range.</param>
        /// <returns>The mapped value.</returns>
        public static float ReMap(
            float value,
            float inMin, float inMax,
            float outMin, float outMax
        )
        {
            // Standard linear remap.
            return
                (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }


        // Double-precision remap variant
        /// <summary>
        /// Linearly maps a value from the range [inMin, inMax] into [outMin, outMax]. Does not clamp.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="inMin">Lower bound of the input range.</param>
        /// <param name="inMax">Upper bound of the input range.</param>
        /// <param name="outMin">Lower bound of the output range.</param>
        /// <param name="outMax">Upper bound of the output range.</param>
        /// <returns>The mapped value.</returns>
        public static double ReMap(
            double value,
            double inMin, double inMax,
            double outMin, double outMax
        )
        {
            return
                (value - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
        }


        // ReMaps with optional clamping of input and/or output ranges.
        /// <summary>
        /// Linearly maps a value from [inMin, inMax] into [outMin, outMax], with optional clamping of the input and output.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="inMin">Lower bound of the input range.</param>
        /// <param name="inMax">Upper bound of the input range.</param>
        /// <param name="outMin">Lower bound of the output range.</param>
        /// <param name="outMax">Upper bound of the output range.</param>
        /// <param name="clampIn">Whether to clamp the input to [inMin, inMax] first.</param>
        /// <param name="clampOut">Whether to clamp the result to [outMin, outMax].</param>
        /// <returns>The mapped value.</returns>
        public static float ReMapClamped(
            float value,
            float inMin, float inMax,
            float outMin, float outMax,
            bool clampIn = true, bool clampOut = true
        )
        {
            return
                Clamp(
                    ReMap(
                        Clamp(
                            value,
                            clampIn ? inMin : float.NegativeInfinity,
                            clampIn ? inMax : float.PositiveInfinity
                        ),
                        inMin, inMax,
                        outMin, outMax
                    ),
                    clampOut ? outMin : float.NegativeInfinity, clampOut ? outMax : float.PositiveInfinity
                );
        }


        // Double-precision clamped remap
        /// <summary>
        /// Linearly maps a value from [inMin, inMax] into [outMin, outMax], with optional clamping of the input and output.
        /// </summary>
        /// <param name="value">The value to map.</param>
        /// <param name="inMin">Lower bound of the input range.</param>
        /// <param name="inMax">Upper bound of the input range.</param>
        /// <param name="outMin">Lower bound of the output range.</param>
        /// <param name="outMax">Upper bound of the output range.</param>
        /// <param name="clampIn">Whether to clamp the input to [inMin, inMax] first.</param>
        /// <param name="clampOut">Whether to clamp the result to [outMin, outMax].</param>
        /// <returns>The mapped value.</returns>
        public static double ReMapClamped(
            double value,
            double inMin, double inMax,
            double outMin, double outMax,
            bool clampIn = true, bool clampOut = true
        )
        {
            return
                Clamp(
                    ReMap(
                        Clamp(
                            value,
                            clampIn ? inMin : double.NegativeInfinity,
                            clampIn ? inMax : double.PositiveInfinity
                        ),
                        inMin, inMax,
                        outMin, outMax
                    ),
                    clampOut ? outMin : double.NegativeInfinity, clampOut ? outMax : double.PositiveInfinity
                );
        }

        #endregion

        #region Boolean to Num

        /// <summary>
        /// Converts a bool to 1 for true and 0 for false.
        /// </summary>
        /// <param name="boolean">The bool to convert.</param>
        /// <returns>1f or 0f.</returns>
        public static float ToFloat(this bool boolean)
        {
            return boolean ? 1f : 0f;
        }


        /// <summary>
        /// Converts a bool to 1 for true and 0 for false.
        /// </summary>
        /// <param name="boolean">The bool to convert.</param>
        /// <returns>1 or 0.</returns>
        public static int ToInt(this bool boolean)
        {
            return boolean ? 1 : 0;
        }

        #endregion

        #region Rounding

        #region Double

        /// <summary>
        ///     Rounds to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static double RoundToN(double val, double n)
        {
            return n * Math.Round(val / n);
        }


        /// <summary>
        ///     Rounds up to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static double CeilToN(double val, double n)
        {
            return n * Math.Ceiling(val / n);
        }


        /// <summary>
        ///     Rounds down to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static double FloorToN(double val, double n)
        {
            return n * Math.Floor(val / n);
        }

        #endregion

        #region Float

        /// <summary>
        /// Rounds to the nearest integer, with midpoints rounded away from zero.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <returns>The rounded value.</returns>
        public static float Round(float val)
        {
            return (float) Math.Round(val, MidpointRounding.AwayFromZero);
        }


        /// <summary>
        /// Rounds down to the nearest integer.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <returns>The floored value.</returns>
        public static float Floor(float val)
        {
            return (float) Math.Floor(val);
        }


        /// <summary>
        /// Rounds up to the nearest integer.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <returns>The ceiling value.</returns>
        public static float Ceil(float val)
        {
            return (float) Math.Ceiling(val);
        }


        /// <summary>
        ///     Rounds to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static float RoundToN(float val, float n)
        {
            return n * Round(val / n);
        }


        /// <summary>
        ///     Rounds up to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static float CeilToN(float val, float n)
        {
            return n * Ceil(val / n);
        }


        /// <summary>
        ///     Rounds down to the nearest multiple of 'n'.
        /// </summary>
        /// <param name="val">Value.</param>
        /// <param name="n">N.</param>
        public static float FloorToN(float val, float n)
        {
            return n * Floor(val / n);
        }

        #endregion

        #endregion

        #region Lerps

        #region Double

        /// <summary>
        ///     Lerp
        /// </summary>
        public static double Lerp(double a, double b, double t)
        {
            return (b - a) * t + a;
        }


        /// <summary>
        ///     Quadratic Lerp
        /// </summary>
        public static double QuadLerp(double a, double b, double t)
        {
            return (b - a) * t * t + a;
        }


        /// <summary>
        ///     A Sine Lerp
        /// </summary>
        public static double SineLerp(double a, double b, double t)
        {
            return (b - a) * Math.Sin(t * Math.PI / 2) + a;
        }


        //Finds the 't' used to lerp between two numbers
        /// <summary>
        /// Finds the interpolation factor t that lerps from a to b to reach the given value.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="lerped">The value reached.</param>
        /// <returns>The factor t.</returns>
        public static double UnLerp(double a, double b, double lerped)
        {
            //Lerp function: lerped = ((b-a) * t) + a
            //Solve for t
            //t = (lerped - a) / (b - a)
            return (lerped - a) / (b - a);
        }


        /// <summary>
        /// Finds the interpolation factor t that quadratically lerps from a to b to reach the given value.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="lerped">The value reached.</param>
        /// <returns>The factor t.</returns>
        public static double UnQuadLerp(double a, double b, double lerped)
        {
            //Lerp function: lerped = ((b-a) * t * t) + a
            //Solve for t
            //t = Sqrt((lerped - a) / (b - a))
            return Math.Sqrt((lerped - a) / (b - a));
        }

        #endregion

        #region Float

        /// <summary>
        ///     Lerp
        /// </summary>
        public static float Lerp(float a, float b, float t)
        {
            return (b - a) * t + a;
        }


        /// <summary>
        ///     Quadratic Lerp
        /// </summary>
        public static float QuadLerp(float a, float b, float t)
        {
            return (b - a) * t * t + a;
        }


        /// <summary>
        ///     A Sine Lerp
        /// </summary>
        public static float SineLerp(float a, float b, float t)
        {
            return (b - a) * (float) Math.Sin(t * Math.PI / 2) + a;
        }


        //Finds the 't' used to lerp between two numbers
        /// <summary>
        /// Finds the interpolation factor t that lerps from a to b to reach the given value.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="lerped">The value reached.</param>
        /// <returns>The factor t.</returns>
        public static float UnLerp(float a, float b, float lerped)
        {
            //Lerp function: lerped = ((b-a) * t) + a
            //Solve for t
            //t = (lerped - a) / (b - a)

            return (lerped - a) / (b - a);
        }


        /// <summary>
        /// Finds the interpolation factor t that quadratically lerps from a to b to reach the given value.
        /// </summary>
        /// <param name="a">Start value.</param>
        /// <param name="b">End value.</param>
        /// <param name="lerped">The value reached.</param>
        /// <returns>The factor t.</returns>
        public static float UnQuadLerp(float a, float b, float lerped)
        {
            //Lerp function: lerped = ((b-a) * t * t) + a
            //Solve for t
            //t = Sqrt((lerped - a) / (b - a))
            return (float) Math.Sqrt((lerped - a) / (double) (b - a));
        }

        #endregion

        #endregion

        #region Arrays

        //public static T[] Merge<T>(this T[] array, params T[] addative)
        //{
        //    return array;
        //}

        #endregion

        #region Quick Convert

        /// <summary>
        ///     Converts radians to degrees
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float ToDeg(this float v)
        {
            return v * 180 / (float) Math.PI;
        }


        /// <summary>
        ///     Converts radians to degrees
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double ToDeg(this double v)
        {
            return v * 180 / Math.PI;
        }


        /// <summary>
        ///     Converts degrees to radians
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float ToRad(this float v)
        {
            return v * (float) Math.PI / 180;
        }


        /// <summary>
        ///     Converts degrees to radians
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double ToRad(this double v)
        {
            return v * Math.PI / 180;
        }

        #endregion
    }


    /// <summary>
    /// Random helpers backed by a cryptographically secure random number generator.
    /// </summary>
    public static class CryptoRand
    {
        private static readonly RNGCryptoServiceProvider rng = new();


        /// <summary>
        ///     Gets an array of random bytes.
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="byteCount">Length of byte array.</param>
        public static byte[] GetBytes(int byteCount)
        {
            var randBytes = new byte[byteCount];

            rng.GetBytes(randBytes);

            return randBytes;
        }


        /// <summary>
        ///     Fills the given byte array with random bytes in place.
        /// </summary>
        /// <param name="byteArray">The array to fill.</param>
        /// <returns>The same array, now filled with random bytes.</returns>
        public static byte[] GetBytes(byte[] byteArray)
        {
            rng.GetBytes(byteArray);

            return byteArray;
        }


        /// <summary>
        ///     Returns a random double from 0-1
        /// </summary>
        public static double Range()
        {
            var bytes = new byte[8];
            rng.GetBytes(bytes);
            //Testing new way to generate random numbers
            //double d = Math.Abs(bytes.DecDouble()) % 1.0;
            var ul = BitConverter.ToUInt64(bytes, 0) / (1 << 11);
            var d = ul / (double) (1UL << 53);

            return d;
        }


        /// <summary>
        ///     Returns a random double between min and max.
        /// </summary>
        /// <param name="min">Lower bound.</param>
        /// <param name="max">Upper bound.</param>
        /// <returns>A random value in [min, max).</returns>
        public static double Range(double min, double max)
        {
            return (max - min) * Range() + min;
        }


        /// <summary>
        ///     Returns a random float between min and max.
        /// </summary>
        /// <param name="min">Lower bound.</param>
        /// <param name="max">Upper bound.</param>
        /// <returns>A random value in [min, max).</returns>
        public static float Range(float min, float max)
        {
            return (max - min) * (float) Range() + min;
        }


        /// <summary>
        ///     Picks a random element from the parameters passed.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <typeparam name="T">The type parameter.</typeparam>
        public static T Pick<T>(params T[] array)
        {
            return array[(int) (array.Length * Range())];
        }


        /// <summary>
        ///     Has an n probability of returning a true
        /// </summary>
        /// <returns></returns>
        public static bool Chance(double n)
        {
            return Range(0, 1) <= n;
        }
    }


    /// <summary>
    ///     XOR obfuscation with a repeating key.
    ///     WARNING: This is not encryption. Anyone with a few bytes of known plain text can recover the key.
    ///     Do not use it for passwords, personal data or anything else that must stay secret; use System.Security.Cryptography (for example AES) instead.
    /// </summary>
    public static class CryptoSymmetric
    {
        /// <summary>
        ///     XORs the data with the key, repeating or trimming the key to match the data length.
        /// </summary>
        /// <param name="data">Data to encrypt.</param>
        /// <param name="key">Key bytes.</param>
        /// <returns>The XOR-ed bytes.</returns>
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            return XORArrayWithKey(data, EqualizeKey(data, key));
        }


        /// <summary>
        ///     Reverses <see cref="Encrypt"/> by applying the same XOR operation with the same key.
        /// </summary>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="key">Key bytes.</param>
        /// <returns>The decrypted bytes.</returns>
        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            return Encrypt(data, key);
        }


        private static byte[] EqualizeKey(byte[] data, byte[] key)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (key == null || key.LongLength == 0)
            {
                throw new ArgumentException("The key must contain at least one byte.", nameof(key));
            }

            //We need to repeat the key
            if (key.LongLength < data.LongLength)
            {
                var keyOriginalLength = key.LongLength;

                var dest = new byte[data.Length];

                for (var i = 0; i < dest.Length; i++)
                {
                    dest[i] = key[i % keyOriginalLength];
                }

                return dest;
            }

            //We need to cut the key down
            if (key.LongLength > data.LongLength)
            {
                var dest = new byte[data.LongLength];
                Array.Copy(key, 0, dest, 0, data.Length);

                return dest;
            }

            return key;
        }


        private static byte[] XORArrayWithKey(byte[] input, byte[] key)
        {
            var XORd = new byte[input.LongLength];

            for (long i = 0; i < key.LongLength; i++)
            {
                XORd[i] = (byte) (input[i] ^ key[i]);
            }

            return XORd;
        }
    }


    /// <summary>
    /// Helpers that wrap common statements, such as loops, conditionals and timing.
    /// </summary>
    public static class StatementsE
    {
        /// <summary>
        ///     Simply loops a given number of times
        /// </summary>
        /// <param name="cycles"></param>
        /// <param name="action"></param>
        public static void Repeat(ulong cycles, Action action)
        {
            if (cycles == 0)
            {
                return;
            }

            for (ulong i = 0; i < cycles; i++)
            {
                action();
            }
        }


        /// <summary>
        ///     A 'foreach' with a 'for' backbone
        ///     Look at source for example
        /// </summary>
        public static void ForEach<T>(T[] objects, Action<T> action)
        {
            for (var i = 0; i < objects.Length; i++)
            {
                action(objects[i]);
            }

            //Example
            /* ForEach(myArray,
             * (arrayElement) =>{
             *      //Do something with arrayElement
             * });
             */
        }


        /// <summary>
        ///     Times how long the action took to complete and returns that time in milliseconds.
        /// </summary>
        public static long SpeedTest(Action action)
        {
            var watch = Stopwatch.StartNew();
            action();
            watch.Stop();

            return watch.ElapsedMilliseconds;
        }


        /// <summary>
        ///     Times how long the action took to complete and returns that time in milliseconds.
        /// </summary>
        public static long PerfTestInvoke(this Action action)
        {
            var watch = Stopwatch.StartNew();
            action();
            watch.Stop();

            return watch.ElapsedMilliseconds;
        }


        /// <summary>
        ///     Runs exactly one of the given actions depending on two conditions. Each action runs only if its case matches and is not null.
        /// </summary>
        /// <param name="condition1">First condition.</param>
        /// <param name="condition2">Second condition.</param>
        /// <param name="only1">Runs when only condition1 is true.</param>
        /// <param name="only2">Runs when only condition2 is true.</param>
        /// <param name="both">Runs when both conditions are true.</param>
        /// <param name="neither">Runs when neither condition is true.</param>
        public static void IfConditions(
            bool condition1, bool condition2,
            Action only1, Action only2,
            Action both = null, Action neither = null
        )
        {
            if (condition1 && !condition2 && only1 != null)
            {
                only1();
            }

            if (!condition1 && condition2 && only2 != null)
            {
                only2();
            }

            if (condition1 && condition2 && both != null)
            {
                both();
            }

            if (!condition1 && !condition2 && neither != null)
            {
                neither();
            }
        }
    }


    /// <summary>
    /// Proportional-integral-derivative controller. Each <see cref="Step"/> sums the weighted P, I and D terms.
    /// </summary>
    public class PID
    {
        /// <summary>
        /// Creates a PID controller with the given gains.
        /// </summary>
        /// <param name="kP">Proportional gain.</param>
        /// <param name="kI">Integral gain.</param>
        /// <param name="kD">Derivative gain.</param>
        public PID(double kP, double kI, double kD)
        {
            this.kP = kP;
            this.kI = kI;
            this.kD = kD;
        }


        /// <summary>
        /// Proportional gain.
        /// </summary>
        public double kP { get; set; }

        /// <summary>
        /// Integral gain.
        /// </summary>
        public double kI { get; set; }

        /// <summary>
        /// Derivative gain.
        /// </summary>
        public double kD { get; set; }

        /// <summary>
        /// Proportional term, the last error passed to <see cref="Step"/>.
        /// </summary>
        public double P { get; set; }

        /// <summary>
        /// Accumulated integral term.
        /// </summary>
        public double I { get; set; }

        /// <summary>
        /// Derivative term, the change in error per unit time from the last <see cref="Step"/>.
        /// </summary>
        public double D { get; set; }


        /// <param name="error">Current error.</param>
        /// <param name="deltaTime">Time since the previous step. Defaults to 1 (per-step units).</param>
        public double Step(double error, double deltaTime = 1d)
        {
            if (deltaTime <= 0d)
            {
                deltaTime = 1d;
            }

            I += error * deltaTime; //Integral
            D = (error - P) / deltaTime; //Derivative
            P = error; //Proportional

            //Return the resultant
            return P * kP + I * kI + D * kD;
        }
    }


    /// <summary>
    /// Named colours as RGBA hex strings in the form #rrggbbaa.
    /// </summary>
    public static class ColorHex
    {
        /// <summary>
        /// Hex colour strings (#rrggbbaa) for each named colour below.
        /// </summary>
        public static readonly string
            Aqua = "#00ffffff",
            Black = "#000000ff",
            Brown = "#a52a2aff",
            DarkBlue = "#0000a0ff",
            Magenta = "#ff00ffff",
            Green = "#008000ff",
            Grey = "#808080ff",
            LightBlue = "#add8e6ff",
            Lime = "#00ff00ff",
            Maroon = "#800000ff",
            Navy = "#000080ff",
            Olive = "#808000ff",
            Orange = "#ffa500ff",
            Purple = "#800080ff",
            Red = "#ff0000ff",
            Silver = "#c0c0c0ff",
            Teal = "#008080ff",
            White = "#ffffffff",
            Yellow = "#ffff00ff";
    }


    /// <summary>
    /// General extension methods for null checks, Action/Func invocation, console output and debugging collections.
    /// </summary>
    public static class ExtensionsE
    {
        #region Conditions

        // Respects UnityEngine.Object's overloaded == (destroyed objects count as null).
        private static bool IsNullOrDestroyed<T>(T instance)
        {
            if (instance is UnityEngine.Object unityObject)
            {
                return unityObject == null;
            }

            return instance == null;
        }


        /// <summary>
        /// Returns true when the instance is null or a destroyed UnityEngine.Object.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <returns>True when null or destroyed.</returns>
        public static bool IsNull<T>(this T instance)
        {
            return IsNullOrDestroyed(instance);
        }


        /// <summary>
        /// Returns true when the instance is neither null nor a destroyed UnityEngine.Object.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <returns>True when not null and not destroyed.</returns>
        public static bool NotNull<T>(this T instance)
        {
            return !IsNullOrDestroyed(instance);
        }


        /// <summary>
        /// Returns true when the instance is null or destroyed, and invokes actionIfNull with it in that case.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <param name="actionIfNull">Invoked only when the instance is null or destroyed.</param>
        /// <returns>True when null or destroyed.</returns>
        public static bool IsNull<T>(this T instance, Action<T> actionIfNull)
        {
            if (IsNullOrDestroyed(instance))
            {
                actionIfNull?.Invoke(instance);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns true when the instance is not null or destroyed, and invokes actionIfNotNull with it in that case.
        /// </summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <param name="instance">The instance to check.</param>
        /// <param name="actionIfNotNull">Invoked only when the instance is not null or destroyed.</param>
        /// <returns>True when not null and not destroyed.</returns>
        public static bool NotNull<T>(this T instance, Action<T> actionIfNotNull)
        {
            if (!IsNullOrDestroyed(instance))
            {
                actionIfNotNull?.Invoke(instance);

                return true;
            }

            return false;
        }

        #endregion

        #region Action/Func

        /// <summary>
        ///     Invokes the action if it isn't null, if it is null then just ignores it.
        /// </summary>
        /// <param name="action">Action.</param>
        public static void InvokeNull(this Action action)
        {
            action?.Invoke();
        }


        /// <summary>
        ///     Invokes the action if it isn't null, if it is null then invoke the 'ifNull' action
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="ifNull">If null.</param>
        public static void InvokeNull(this Action action, Action ifNull)
        {
            if (action == null)
            {
                ifNull();
            }
            else
            {
                action();
            }
        }

        #endregion

        #region Console

        /// <summary>
        /// Writes the message to the console without a newline.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message to write.</param>
        public static void Write<T>(this T message)
        {
            // System.Console output is not shown in Unity; log to the Unity Console instead.
            UnityEngine.Debug.Log(message);
        }


        /// <summary>
        /// Writes the message to the console followed by a newline.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="message">The message to write.</param>
        public static void WriteLine<T>(this T message)
        {
            UnityEngine.Debug.Log(message);
        }

        #endregion

        #region Debugging

        #region Printing Enumerables

        /// <summary>
        /// Joins the elements of the enumerable into one string, separated by splitter.
        /// </summary>
        /// <typeparam name="TSource">The element type.</typeparam>
        /// <param name="enumerable">The elements to join.</param>
        /// <param name="splitter">Text placed between elements.</param>
        /// <returns>The joined string.</returns>
        public static string ToElementsString<TSource>(
            this IEnumerable<TSource> enumerable,
            string splitter = ", ")
        {
            var str = "";

            using (var enumerator = enumerable.GetEnumerator())
            {
                var putFirst = false;

                while (enumerator.MoveNext())
                {
                    str +=
                        (putFirst ? splitter : "") + //If I already put the first index, add in the splitter
                        enumerator.Current;

                    putFirst = true;
                }
            }

            return str;
        }


        /// <summary>
        /// Writes the elements of the enumerable to the console, separated by splitter.
        /// </summary>
        /// <typeparam name="TSource">The element type.</typeparam>
        /// <param name="enumerable">The elements to write.</param>
        /// <param name="splitter">Text placed between elements.</param>
        public static void WriteArrayElements<TSource>(
            this IEnumerable<TSource> enumerable,
            string splitter = ", ")
        {
            UnityEngine.Debug.Log(ToElementsString(enumerable, splitter));
        }


        /// <summary>
        /// Writes the elements of the enumerable to the console, separated by splitter. Currently delegates to <see cref="WriteArrayElements{TSource}"/> without a trailing newline.
        /// </summary>
        /// <typeparam name="TSource">The element type.</typeparam>
        /// <param name="enumerable">The elements to write.</param>
        /// <param name="splitter">Text placed between elements.</param>
        public static void WriteLineArrayElements<TSource>(
            this IEnumerable<TSource> enumerable,
            string splitter = ", ")
        {
            WriteArrayElements(enumerable, splitter);
        }

        #endregion

        #region Printing Dictionaries

        /// <summary>
        /// Formats a dictionary as one string, with each key and value joined by keyValueSeparator and entries joined by elementSeparator.
        /// </summary>
        /// <typeparam name="TSource1">The key type.</typeparam>
        /// <typeparam name="TSource2">The value type.</typeparam>
        /// <param name="dict">The dictionary to format.</param>
        /// <param name="keyValueSeparator">Text placed between each key and its value.</param>
        /// <param name="elementSeparator">Text placed between entries.</param>
        /// <returns>The formatted string.</returns>
        public static string ToElementsString<TSource1, TSource2>(
            this Dictionary<TSource1, TSource2> dict,
            string keyValueSeparator = " => ",
            string elementSeparator = "\n")
        {
            var returnString = "";

            var dictEnumerator = dict.GetEnumerator();

            while (dictEnumerator.MoveNext())
            {
                returnString +=
                    elementSeparator +
                    dictEnumerator.Current.Key +
                    keyValueSeparator +
                    dictEnumerator.Current.Value;
            }

            return returnString.Remove(0, elementSeparator.Length);
        }


        /// <summary>
        ///     Writes elements to the console
        /// </summary>
        public static void WriteElements<TSource1, TSource2>(
            this Dictionary<TSource1, TSource2> dict,
            string keyValueSeparator = " => ",
            string elementSeparator = "\n")
        {
            ToElementsString(dict, keyValueSeparator, elementSeparator)
                .WriteLine();
        }

        #endregion

        #endregion
    }


    /// <summary>
    ///     Used to store a variable type as a reference type.
    ///     Or for some interesting scoping tricks ;)
    /// </summary>
    public class Ref<T>
    {
        private T val;


        /// <summary>
        /// Creates a reference holder with the given initial value.
        /// </summary>
        /// <param name="value">The initial value.</param>
        public Ref(T value)
        {
            val = value;
        }


        /// <summary>
        /// The stored value. Virtual, so subclasses can change how it is read or written.
        /// </summary>
        public virtual T Value
        {
            get => val;
            set => val = value;
        }
    }


    /// <summary>
    ///     Can be used to setup a getter and setter of an external
    ///     instance of a given object
    /// </summary>
    public class GetSet<T>
    {
        private readonly Func<T> getter;
        private readonly Action<T> setter;


        /// <summary>
        /// Creates a property wrapper that reads and writes through the given delegates.
        /// </summary>
        /// <param name="getter">Returns the external value.</param>
        /// <param name="setter">Assigns the external value.</param>
        public GetSet
        (
            Func<T> getter,
            Action<T> setter
        )
        {
            this.getter = getter;
            this.setter = setter;
        }


        /// <summary>
        /// The external value, read through the getter and written through the setter.
        /// </summary>
        public T value
        {
            get => getter();
            set => setter(value);
        }
    }
}
