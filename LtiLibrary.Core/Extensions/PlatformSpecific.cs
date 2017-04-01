namespace LtiLibrary.Core.Extensions
{
    using System;

    /// <summary>
    /// Contains platform specific implementations.
    /// </summary>
    public static class PlatformSpecific
    {
        private static readonly char[] HexUpperChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>
        /// Converts a specified character into its hexadecimal equivalent.
        /// </summary>
        /// <param name="character">The character to convert to hexadecimal representation.</param>
        /// <returns>The hexadecimal representation of the specified character.</returns>
        public static string HexEscape(char character)
        {
            // .Net Core doesn't have escape function.
            if (character > '\xff')
            {
                throw new ArgumentOutOfRangeException(nameof(character));
            }

            return "%" + HexUpperChars[((character & 0xf0) >> 4)] + HexUpperChars[((character & 0x0f))];
        }
    }
}

namespace System
{
    /// <summary>
    /// Empty attribute for backward compatability with XmlSerializer created
    /// by pre-.NET Core XSD utility. Rumors are Microsoft will update the XSD
    /// utility in Q2 of 2017.
    /// </summary>
    public class SerializableAttribute : Attribute
    {
    }
}