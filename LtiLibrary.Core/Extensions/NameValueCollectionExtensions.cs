using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using LtiLibrary.Core.Lti1;
using LtiLibrary.Core.OAuth;

namespace LtiLibrary.Core.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static void AddParameter(this NameValueCollection parameters, string name, IList<Role> values)
        {
            if (values.Count > 0)
            {
                parameters.Add(name, string.Join(",", values));
            }
        }

        public static void AddParameter(this NameValueCollection parameters, string name, int? value)
        {
            if (value.HasValue)
            {
                parameters.Add(name, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static void AddParameter(this NameValueCollection parameters, string name, Int64 value)
        {
            parameters.Add(name, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void AddParameter(this NameValueCollection parameters, string name, DocumentTarget? value)
        {
            if (value.HasValue)
            {
                parameters.Add(name, value.Value.ToString());
            }
        }

        public static void AddParameter(this NameValueCollection parameters, string name, LisContextType? value)
        {
            if (value.HasValue)
            {
                parameters.Add(name, value.Value.ToString());
            }
        }

        public static void AddParameter(this NameValueCollection parameters, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                parameters.Add(name, value);
            }
        }

        /// <summary>
        /// Return a normalized string of parameters suitable for OAuth 1.0 signature base string
        /// as defined by https://tools.ietf.org/html/rfc5849#section-3.4.1.3.2
        /// </summary>
        /// <param name="collection">The list of name/value pairs to normalize.</param>
        /// <returns>A normalized string of parameters.</returns>
        public static string ToNormalizedString(this NameValueCollection collection)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery

            // https://tools.ietf.org/html/rfc5849#section-3.4.1.3.1
            // Exclude the OAuth signature or realm in the signature base string
            var list = new List<KeyValuePair<string, string>>();
            var excludedNames = new List<string> { OAuthConstants.SignatureParameter, OAuthConstants.RealmParameter };
            foreach (var key in collection.AllKeys)
            {
                if (excludedNames != null && excludedNames.Contains(key)) continue;
                var value = collection[key] ?? string.Empty;
                list.Add(new KeyValuePair<string, string>(key.ToRfc3986EncodedString(),
                    value.ToRfc3986EncodedString()));
            }

            // https://tools.ietf.org/html/rfc5849#section-3.4.1.3.2
            // Sort the parameters by name, then value
            list.Sort((left, right) => left.Key.Equals(right.Key, StringComparison.Ordinal)
                ? string.Compare(left.Value, right.Value, StringComparison.Ordinal)
                : string.Compare(left.Key, right.Key, StringComparison.Ordinal));

            var normalizedString = new StringBuilder();
            foreach (var pair in list)
            {
                normalizedString.Append('&').Append(pair.Key).Append('=').Append(pair.Value);
            }
            return normalizedString.ToString().TrimStart('&');
        }
    }
}
