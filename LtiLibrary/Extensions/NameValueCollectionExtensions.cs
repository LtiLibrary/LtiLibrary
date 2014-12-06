using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using HtmlAgilityPack;
using LtiLibrary.Lti1;

namespace LtiLibrary.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static void AddHtmlParameter(this NameValueCollection parameters, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);

                var text = doc.ToPlainText().Trim(new[] { ' ', '\r', '\n' });
                if (!string.IsNullOrWhiteSpace(text))
                {
                    parameters.Add(name, text);
                }
            }
        }

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

        public static string ToNormalizedString(this NameValueCollection collection, IList<string> excludedNames = null)
        {
            var list = new List<KeyValuePair<string, string>>();

// ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var key in collection.AllKeys)
            {
                if (excludedNames != null && excludedNames.Contains(key)) continue;
                var value = collection[key] ?? string.Empty;
                list.Add(new KeyValuePair<string, string>(key.ToRfc3986EncodedString(),
                    value.ToRfc3986EncodedString()));
            }

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
