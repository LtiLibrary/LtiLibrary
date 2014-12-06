using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using LtiLibrary.Common;
using LtiLibrary.OAuth;

namespace LtiLibrary.Extensions
{
    public static class AuthorizationHeaderValueExtensions
    {
        /// <summary>
        /// Parse the OAuth AuthenticationHeaderValue.Parameter into a NameValueCollection.
        /// </summary>
        /// <param name="authorizationHeader">The AuthorizationHeaderValue to parse.</param>
        /// <returns>A NameValueCollection of all the parameters found.</returns>
        public static NameValueCollection ParseOAuthAuthorizationHeader(this AuthenticationHeaderValue authorizationHeader)
        {
            if (!authorizationHeader.Scheme.Equals(OAuthConstants.AuthScheme))
            {
                throw new LtiException("Invalid Authorization scheme");
            }

            var parameters = new NameValueCollection();
            foreach (var pair in authorizationHeader.Parameter.Split(','))
            {
                var keyValue = pair.Split('=');
                var key = keyValue[0].Trim();

                // Ignore unknown parameters
                if (!OAuthConstants.OAuthParameters.Any(p => p.Equals(key)))
                    continue;

                var value = HttpUtility.UrlDecode(keyValue[1].Trim('"'));
                parameters.AddParameter(key, value);
            }
            return parameters;
        }
    }
}
