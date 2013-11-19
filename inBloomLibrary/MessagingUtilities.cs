using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inBloomLibrary
{
    public static class MessagingUtilities
    {
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        // From DotNetOpenAuth.Messaging.MessagingUtilities
        internal static void AppendQueryArgs(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            var keyValuePairs = args as KeyValuePair<string, string>[] ?? args.ToArray();
            if (args != null && keyValuePairs.Any())
            {
                StringBuilder sb = new StringBuilder(50 + (keyValuePairs.Count() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    sb.Append(builder.Query.Substring(1));
                    sb.Append('&');
                }
                sb.Append(CreateQueryString(keyValuePairs));

                builder.Query = sb.ToString();
            }
        }

        // From DotNetOpenAuth.Messaging.MessagingUtilities
        static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            var keyValuePairs = args as KeyValuePair<string, string>[] ?? args.ToArray();
            if (!keyValuePairs.Any())
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder(keyValuePairs.Count() * 10);

            foreach (var p in keyValuePairs)
            {
                sb.Append(EscapeUriDataStringRfc3986(p.Key));
                sb.Append('=');
                sb.Append(EscapeUriDataStringRfc3986(p.Value));
                sb.Append('&');
            }
            sb.Length--; // remove trailing &

            return sb.ToString();
        }

        internal static string EscapeUriDataStringRfc3986(string value)
        {
            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            foreach (var t in UriRfc3986CharsToEscape)
            {
                escaped.Replace(t, Uri.HexEscape(t[0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }

    }
}
