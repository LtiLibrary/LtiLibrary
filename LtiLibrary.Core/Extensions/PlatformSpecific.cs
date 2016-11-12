namespace LtiLibrary.Core.Extensions
{
    using System;

#if NetCore
    using Sha1 = System.Security.Cryptography.SHA1;
#else
    using Sha1 = System.Security.Cryptography.SHA1CryptoServiceProvider;
#endif

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
#if NetCore
            if (character > '\xff')
            {
                throw new ArgumentOutOfRangeException(nameof(character));
            }

            return "%" + HexUpperChars[((character & 0xf0) >> 4)] + HexUpperChars[((character & 0x0f))];
#else
            return Uri.HexEscape(character);
#endif
        }

        /// <summary>
        /// Gets SHA1 implementation.
        /// </summary>
        /// <returns><see cref="Sha1"/>.</returns>
        public static Sha1 GetSha1Provider()
        {
#if NetCore
            return Sha1.Create();
#else
            return new Sha1();
#endif
        }
    }
}

#if NetCore

namespace System
{
    /// <summary>
    /// Empty attribute for backward compatability.
    /// </summary>
    public class SerializableAttribute : Attribute
    {
    }
}

#endif
