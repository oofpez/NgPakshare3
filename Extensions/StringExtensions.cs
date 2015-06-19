using System;

namespace Wimt.CachingFramework.Extensions
{
    internal static class StringExtensions
    {
        public static string FormatWith(this string formatString, params object[] args)
        {
            return args == null || args.Length == 0 ? formatString : String.Format(formatString, args);
        }

        ///<summary>
        /// Tests the string to see if it is null or "".
        ///</summary>
        ///<returns>True if null or "".</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        ///<summary>
        /// Tests the string to see if it is null, empty or consists only of whitespace.
        ///</summary>
        ///<returns>True if null or "".</returns>
        public static bool IsNullOrWhitespace(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }
    }
}