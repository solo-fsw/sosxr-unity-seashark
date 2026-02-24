using System;
using System.Collections.Generic;
using System.Linq;


namespace SOSXR.SeaShark
{
    /// <summary>
    ///     String extension helpers used across the project.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>Checks if a string is Null or white space.</summary>
        /// <param name="val">The string to test.</param>
        /// <returns>True if the string is null or consists only of white-space characters.</returns>
        public static bool IsNullOrWhiteSpace(this string val)
        {
            return string.IsNullOrWhiteSpace(val);
        }


        /// <summary>Checks if a string is Null or empty.</summary>
        /// <param name="value">The string to test.</param>
        /// <returns>True if the string is null or empty.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }


        /// <summary>Checks if a string contains null, empty or white space.</summary>
        /// <param name="val">The string to test.</param>
        /// <returns>True if the string is blank (null, empty, or whitespace).</returns>
        public static bool IsBlank(this string val)
        {
            return val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();
        }


        /// <summary>Returns an empty string if the value is null; otherwise returns the original value.</summary>
        /// <param name="val">The string value to coalesce.</param>
        /// <returns>Null-safe string value.</returns>
        public static string OrEmpty(this string val)
        {
            return val ?? string.Empty;
        }


        /// <summary>
        ///     Shortens a string to the specified maximum length. If the string's length
        ///     is less than the maxLength, the original string is returned.
        /// </summary>
        /// <param name="val">The string to shorten.</param>
        /// <param name="maxLength">Maximum allowed length.</param>
        /// <returns>The shortened string or the original if within bounds.</returns>
        public static string Shorten(this string val, int maxLength)
        {
            if (val.IsBlank())
            {
                return val;
            }

            return val.Length <= maxLength ? val : val.Substring(0, maxLength);
        }


        /// <summary>Slices a string from the start index to the end index.</summary>
        /// <param name="val">The string to slice.</param>
        /// <param name="startIndex">Inclusive start index.</param>
        /// <param name="endIndex">Exclusive end index. Can be negative to count from end.</param>
        /// <returns>The sliced substring.</returns>
        public static string Slice(this string val, int startIndex, int endIndex)
        {
            if (val.IsBlank())
            {
                throw new ArgumentNullException(nameof(val), "Value cannot be null or empty.");
            }

            if (startIndex < 0 || startIndex > val.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // If the end index is negative, it will be counted from the end of the string.
            endIndex = endIndex < 0 ? val.Length + endIndex : endIndex;

            if (endIndex < 0 || endIndex < startIndex || endIndex > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }

            return val.Substring(startIndex, endIndex - startIndex);
        }


        /// <summary>
        ///     Converts the input string to an alphanumeric string, optionally allowing periods.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <param name="allowPeriods">Whether periods are allowed in the output.</param>
        /// <returns>A new string containing only alphanumeric characters, underscores, and optionally periods.</returns>
        public static string ConvertToAlphanumeric(this string input, bool allowPeriods = false)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var filteredChars = new List<char>();
            var lastValidIndex = -1;

            // Iterate over the input string, filtering and determining valid start/end indices
            foreach (var character in input
                                      .Where(character => char
                                          .IsLetterOrDigit(character) || character == '_' || (allowPeriods && character == '.'))
                                      .Where(character => filteredChars.Count != 0 || (!char.IsDigit(character) && character != '.')))
            {
                filteredChars.Add(character);
                lastValidIndex = filteredChars.Count - 1; // Update lastValidIndex for valid characters
            }

            // Remove trailing periods
            while (lastValidIndex >= 0 && filteredChars[lastValidIndex] == '.')
            {
                lastValidIndex--;
            }

            // Return the filtered string
            return lastValidIndex >= 0
                ? new string(filteredChars.ToArray(), 0, lastValidIndex + 1)
                : string.Empty;
        }


        // Rich text formatting, for Unity UI elements that support rich text.
        public static string RichColor(this string text, string color)
        {
            return $"<color={color}>{text}</color>";
        }


        public static string RichSize(this string text, int size)
        {
            return $"<size={size}>{text}</size>";
        }


        public static string RichBold(this string text)
        {
            return $"<b>{text}</b>";
        }


        public static string RichItalic(this string text)
        {
            return $"<i>{text}</i>";
        }


        public static string RichUnderline(this string text)
        {
            return $"<u>{text}</u>";
        }


        public static string RichStrikethrough(this string text)
        {
            return $"<s>{text}</s>";
        }


        public static string RichFont(this string text, string font)
        {
            return $"<font={font}>{text}</font>";
        }


        public static string RichAlign(this string text, string align)
        {
            return $"<align={align}>{text}</align>";
        }


        public static string RichGradient(this string text, string color1, string color2)
        {
            return $"<gradient={color1},{color2}>{text}</gradient>";
        }


        public static string RichRotation(this string text, float angle)
        {
            return $"<rotate={angle}>{text}</rotate>";
        }


        public static string RichSpace(this string text, float space)
        {
            return $"<space={space}>{text}</space>";
        }
    }
}
