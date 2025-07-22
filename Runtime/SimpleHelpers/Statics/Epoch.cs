using System;


namespace SOSXR.SeaShark.Statics
{
    /// <summary>
    ///     Provides utilities for working with epoch time.
    ///     All times are managed in milliseconds and can be retrieved or formatted as strings.
    /// </summary>
    public static class Epoch
    {
        /// <summary>
        ///     Gets the current local time in milliseconds since Unix epoch (January 1, 1970).
        /// </summary>
        /// <param name="logResult">If true, logs the result to the Unity console (default: false).</param>
        /// <returns>The current local time in milliseconds as a long.</returns>
        public static long GetCurrentTimeInMilliseconds()
        {
            var unixTime = DateTimeOffset.Now.ToLocalTime().ToUnixTimeMilliseconds();

            return unixTime;
        }


        /// <summary>
        ///     Gets the current local time as a formatted string.
        /// </summary>
        /// <param name="format">The desired date-time format (default: "dd/MM/yyyy HH:mm:ss.fff").</param>
        /// <param name="logResult">If true, logs the result to the Unity console (default: false).</param>
        /// <returns>The current local time as a formatted string.</returns>
        public static string GetCurrentTimeAsString(string format = "dd/MM/yyyy HH:mm:ss.fff")
        {
            var timeString = GetCurrentTimeInMilliseconds().ToString(format);

            return timeString;
        }


        /// <summary>
        ///     Converts an epoch time (in milliseconds) to a formatted local time string.
        /// </summary>
        /// <param name="epoch">The epoch time in milliseconds.</param>
        /// <param name="format">The desired date-time format (default: "dd/MM/yyyy HH:mm:ss.fff").</param>
        /// <returns>The formatted local time string.</returns>
        public static string FormatEpoch(long epoch, string format = "dd/MM/yyyy HH:mm:ss.fff")
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(epoch).ToLocalTime().ToString(format);
        }


        /// <summary>
        ///     Converts an epoch time (in milliseconds, as a double) to a formatted local time string.
        /// </summary>
        /// <param name="epoch">The epoch time in milliseconds (as a double).</param>
        /// <param name="format">The desired date-time format (default: "dd/MM/yyyy HH:mm:ss.fff").</param>
        /// <returns>The formatted local time string.</returns>
        public static string FormatEpoch(double epoch, string format = "dd/MM/yyyy HH:mm:ss.fff")
        {
            return FormatEpoch((long) epoch, format);
        }
    }
}