using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;

namespace LtiLibrary.NetCore.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="NameValueCollection"/> type.
    /// </summary>
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Add a list of <see cref="Enum"/> values as a comma separated string.
        /// </summary>
        /// <param name="parameters">The <see cref="NameValueCollection"/>.</param>
        /// <param name="name">The key of the entry to add.</param>
        /// <param name="values">The list of <see cref="Enum"/> values to add.</param>
        public static void AddParameter(this NameValueCollection parameters, string name, IList<Enum> values)
        {
            if (values.Count > 0)
            {
                parameters.Add(name, string.Join(",", values));
            }
        }

        /// <summary>
        /// Add an <see cref="int"/> value if it is not null.
        /// </summary>
        /// <param name="parameters">The <see cref="NameValueCollection"/>.</param>
        /// <param name="name">The key of the entry to add.</param>
        /// <param name="value">The nullable <see cref="int"/> value to add.</param>
        public static void AddParameter(this NameValueCollection parameters, string name, int? value)
        {
            if (value.HasValue)
            {
                parameters.Add(name, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Add an <see cref="long"/> value if it is not null.
        /// </summary>
        /// <param name="parameters">The <see cref="NameValueCollection"/>.</param>
        /// <param name="name">The key of the entry to add.</param>
        /// <param name="value">The nullable <see cref="long"/> value to add.</param>
        public static void AddParameter(this NameValueCollection parameters, string name, long value)
        {
            parameters.Add(name, value.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Add a <see cref="DocumentTarget"/> value if it is not null.
        /// </summary>
        /// <param name="parameters">The <see cref="NameValueCollection"/>.</param>
        /// <param name="name">The key of the entry to add.</param>
        /// <param name="value">The nullable <see cref="DocumentTarget"/> value to add.</param>
        public static void AddParameter(this NameValueCollection parameters, string name, DocumentTarget? value)
        {
            if (value.HasValue)
            {
                parameters.Add(name, value.Value.ToString());
            }
        }

        /// <summary>
        /// Add a <see cref="ContextType"/> value if it is not null.
        /// </summary>
        /// <param name="parameters">The <see cref="NameValueCollection"/>.</param>
        /// <param name="name">The key of the entry to add.</param>
        /// <param name="value">The nullable <see cref="ContextType"/> value to add.</param>
        public static void AddParameter(this NameValueCollection parameters, string name, ContextType? value)
        {
            if (value.HasValue)
            {
                parameters.Add(name, value.Value.ToString());
            }
        }

        /// <summary>
        /// Add a <see cref="string"/> value if it is not null or whitespace.
        /// </summary>
        /// <param name="parameters">The <see cref="NameValueCollection"/>.</param>
        /// <param name="name">The key of the entry to add.</param>
        /// <param name="value">The <see cref="string"/> value to add.</param>
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
            var excludedNames = new List<string> {OAuthConstants.SignatureParameter, OAuthConstants.RealmParameter};
            foreach (var key in collection.AllKeys)
            {
                if (excludedNames.Contains(key)) continue;
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