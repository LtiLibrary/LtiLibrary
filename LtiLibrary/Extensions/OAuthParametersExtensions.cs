using System;
using System.Collections.Generic;
using System.Globalization;
using HtmlAgilityPack;
using LtiLibrary.Common;
using OAuth.Net.Common;

namespace LtiLibrary.Extensions
{
    public static class OAuthParametersExtensions
    {
        public static void AddHtmlParameter(this OAuthParameters parameters, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(value);

                var text = doc.ToPlainText().Trim(new[] {' ', '\r', '\n'});
                if (!string.IsNullOrWhiteSpace(text))
                {
                    parameters.AdditionalParameters.Add(name, text);
                }
            }
        }

        public static void AddParameter(this OAuthParameters parameters, string name, IList<LtiRoles> values)
        {
            if (values.Count > 0)
            {
                parameters.AdditionalParameters.Add(name, string.Join(",", values));
            }
        }

        public static void AddParameter(this OAuthParameters parameters, string name, int? value)
        {
            if (value.HasValue)
            {
                parameters.AdditionalParameters.Add(name, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static void AddParameter(this OAuthParameters parameters, string name, PresentationTargets? value)
        {
            if (value.HasValue)
            {
                parameters.AdditionalParameters.Add(name, value.Value.ToString());
            }
        }

        public static void AddParameter(this OAuthParameters parameters, string name, LisContextTypes? value)
        {
            if (value.HasValue)
            {
                parameters.AdditionalParameters.Add(name, value.Value.ToString());
            }
        }

        public static void AddParameter(this OAuthParameters parameters, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                parameters.AdditionalParameters.Add(name, value);
            }
        }

        public static int? GetInt(this OAuthParameters parameters, string key)
        {
            var stringValue = parameters.GetString(key);
            int intValue;
            return int.TryParse(stringValue, out intValue)
                       ? intValue
                       : default(int?);
        }

        public static LisContextTypes? GetLisContextType(this OAuthParameters parameters, string key)
        {
            var stringValue = parameters.GetString(key);
            LisContextTypes contextTypeEnum;
            return Enum.TryParse(stringValue, out contextTypeEnum)
                       ? contextTypeEnum
                       : default(LisContextTypes?);
        }

        public static PresentationTargets? GetPresentationTarget(this OAuthParameters parameters, string key)
        {
            var stringValue = parameters.GetString(key);
            PresentationTargets presentationTargetEnum;
            return Enum.TryParse(stringValue, out presentationTargetEnum)
                       ? presentationTargetEnum
                       : default(PresentationTargets?);
        }

        public static string GetString(this OAuthParameters parameters, string key)
        {
            return parameters.AdditionalParameters[key];
        }

        public static void RequireAllAdditionalOf(this OAuthParameters parameters, params string[] requiredParameters)
        {
            var missing = new List<string>();

            foreach (var requiredParameter in requiredParameters)
            {
                if (string.IsNullOrEmpty(parameters.AdditionalParameters[requiredParameter]))
                    missing.Add(requiredParameter);
            }

            if (missing.Count > 0)
            {
                OAuthRequestException.ThrowParametersAbsent(missing.ToArray(), null);
            }
        }
    }
}
