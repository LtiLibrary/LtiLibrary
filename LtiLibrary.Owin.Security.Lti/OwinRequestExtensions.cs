using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;
using LtiLibrary.Core.Lti1;
using LtiLibrary.Core.OAuth;
using LtiLibrary.Core.Outcomes;
using Microsoft.Owin;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LtiLibrary.Owin.Security.Lti
{
    /// <summary>
    /// Extension methods for using <see cref="OwinRequest"/>
    /// </summary>
    public static class OwinRequestExtensions
    {
        public static AuthorizationHeader GetAuthorizationHeader(this IOwinRequest request)
        {
            var authorizationHeader = new AuthorizationHeader();
            var header = request.Headers[OAuthConstants.AuthorizationHeader];
            if (string.IsNullOrWhiteSpace(header)) return authorizationHeader;

            var schemeSeparatorIndex = header.IndexOf(' ');
            authorizationHeader.Scheme = header.Substring(0, schemeSeparatorIndex);
            var headerParameter = header.Substring(schemeSeparatorIndex + 1);

            foreach (var pair in headerParameter.Split(','))
            {
                var keyValue = pair.Split('=');
                var key = keyValue[0].Trim();

                // Ignore unknown parameters
                if (!OAuthConstants.OAuthParameters.Any(p => p.Equals(key)))
                    continue;

                var value = WebUtility.UrlDecode(keyValue[1].Trim('"'));
                authorizationHeader.Parameters.AddParameter(key, value);
            }
            return authorizationHeader;
        }

        /// <summary>
        /// Returns True if the <see cref="IOwinRequest"/> contains an LTI request that requires authentication.
        /// </summary>
        /// <param name="request">The <see cref="IOwinRequest"/> to examine.</param>
        /// <returns>True if the request contains an LTI request that requires authentication.</returns>
        public static async Task<bool> IsAuthenticatedLtiRequestAsync(this IOwinRequest request)
        {
            // All LTI 1.x communication is handled with HTTP POST
            if (!request.Method.Equals(WebRequestMethods.Http.Post)) return false;

            // LTI launch and content item requests are sent as form posts
            if (request.ContentType.Equals("application/x-www-form-urlencoded"))
            {
                var form = await request.ReadFormAsync();
                var messageType = form[LtiConstants.LtiMessageTypeParameter];
                return !string.IsNullOrWhiteSpace(messageType);
            }

            // LTI Outcome requests and responses are sent as Plain Old XML
            if (request.ContentType.Equals("application/xml"))
            {
                return LtiOutcomesHelper.IsLtiOutcomesRequest(request.Body);
            }

            // Tool Consumer Profile requests and responses do not need authentication
            return false;
        }

        /// <summary>
        /// Parse the <see cref="IOwinRequest"/> into an <see cref="ILtiRequest"/>.
        /// </summary>
        /// <param name="request">The <see cref="IOwinRequest"/> to parse.</param>
        /// <returns>The <see cref="ILtiRequest"/>.</returns>
        public static async Task<ILtiRequest> ParseLtiRequestAsync(this IOwinRequest request)
        {
            var ltiRequest = new LtiRequest(null)
            {
                Url = request.Uri,
                HttpMethod = request.Method
            };
            // LTI launch and content item requests are sent as form posts
            if (request.ContentType.Equals("application/x-www-form-urlencoded"))
            {
                var form = await request.ReadFormAsync();
                foreach (var pair in form)
                {
                    ltiRequest.Parameters.Add(pair.Key, string.Join(",", pair.Value));
                }
                return ltiRequest;
            }

            // LTI Outcome requests and responses are sent as Plain Old XML
            if (request.ContentType.Equals("application/xml"))
            {
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        await request.Body.CopyToAsync(ms);
                        using (var reader = new StreamReader(ms))
                        {
                            ltiRequest.ImsxPoxEnvelope = await reader.ReadToEndAsync();
                        }
                    }
                    finally 
                    {
                        if (request.Body.CanSeek)
                        {
                            request.Body.Position = 0;
                        }
                    }
                }
                var authorizationHeader = request.GetAuthorizationHeader();
                ltiRequest.Parameters.Add(authorizationHeader.Parameters);
                return ltiRequest;
            }
            return ltiRequest;
        }
    }
}
