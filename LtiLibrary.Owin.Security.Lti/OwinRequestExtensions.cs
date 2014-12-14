using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;
using LtiLibrary.Core.Lti1;
using LtiLibrary.Core.OAuth;
using LtiLibrary.Core.Outcomes;
using Microsoft.Owin;

namespace LtiLibrary.Owin.Security.Lti
{
    /// <summary>
    /// Extension methods for using <see cref="OwinRequest"/>
    /// </summary>
    public static class OwinRequestExtensions
    {
        // These OAuth parameters are required
        private static readonly string[] RequiredOauthParameters =
            {
                OAuthConstants.ConsumerKeyParameter,
                OAuthConstants.NonceParameter,
                OAuthConstants.SignatureParameter,
                OAuthConstants.SignatureMethodParameter,
                OAuthConstants.TimestampParameter,
                OAuthConstants.VersionParameter
            };

        // These LTI launch parameters are required
        private static readonly string[] RequiredBasicLaunchParameters =
            {
                LtiConstants.LtiMessageTypeParameter,
                LtiConstants.LtiVersionParameter,
                LtiConstants.ResourceLinkIdParameter,
                LtiConstants.UserIdParameter
            };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemLaunchParameters =
            {
                LtiConstants.AcceptMediaTypesParameter,
                LtiConstants.AcceptPresentationDocumentTargetsParameter,
                LtiConstants.ContentItemReturnUrlParameter,
                LtiConstants.LtiMessageTypeParameter,
                LtiConstants.LtiVersionParameter,
                LtiConstants.UserIdParameter
            };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemResponseParameters =
            {
                LtiConstants.ContentItemPlacementParameter
            };

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
        /// Check if the <see cref="IFormCollection"/> has all the parameters.
        /// </summary>
        /// <param name="form">The <see cref="IFormCollection"/> to examine</param>
        /// <param name="parameters">The form keys to check</param>
        /// <returns>True if the form has all the keys</returns>
        private static bool HasAllOf(this IFormCollection form, IEnumerable<string> parameters)
        {
            return parameters.Count(parameter => string.IsNullOrEmpty(form[parameter])) == 0;
        }

        /// <summary>
        /// Check if the <see cref="IOwinRequest"/> is an LTI launch-style request.
        /// </summary>
        public static async Task<bool> IsAuthenticatedWithLtiFormPostAsync(this IOwinRequest request)
        {
            if (!request.Method.Equals(WebRequestMethods.Http.Post)) return false;
            if (!request.ContentType.Equals("application/x-www-form-urlencoded")) return false;

            var form = await request.ReadFormAsync();
            var messageType = form[LtiConstants.LtiMessageTypeParameter] ?? string.Empty;
            var isLtiRequest = 
                messageType.Equals(LtiConstants.BasicLaunchLtiMessageType, StringComparison.OrdinalIgnoreCase) ||
                messageType.Equals(LtiConstants.ContentItemSelectionRequestLtiMessageType, StringComparison.OrdinalIgnoreCase) ||
                messageType.Equals(LtiConstants.ContentItemSelectionResponseLtiMessageType, StringComparison.OrdinalIgnoreCase);
            if (!isLtiRequest) return false;

            // Make sure the request contains all the required OAuth parameters
            if (!form.HasAllOf(RequiredOauthParameters)) return false;

            // Make sure the request contains all the required LTI parameters
            switch (form[LtiConstants.LtiMessageTypeParameter])
            {
                case LtiConstants.BasicLaunchLtiMessageType:
                {
                    if (form.HasAllOf(RequiredBasicLaunchParameters))
                    {
                        return true;
                    }
                    break;
                }
                case LtiConstants.ContentItemSelectionRequestLtiMessageType:
                {
                    if (form.HasAllOf(RequiredContentItemLaunchParameters))
                    {
                        return true;
                    }
                    break;
                }
                case LtiConstants.ContentItemSelectionResponseLtiMessageType:
                {
                    if (form.HasAllOf(RequiredContentItemResponseParameters))
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the <see cref="IOwinRequest"/> is an LTI Plain Old XML (POX) style request.
        /// </summary>
        public static bool IsAuthenticatedWithLtiPoxPost(this IOwinRequest request)
        {
            if (!request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.OrdinalIgnoreCase)) return false;
            if (!request.ContentType.Equals("application/xml")) return false;

            // All of the OAuth parameters in an LTI Outcomes request are in the
            // Authorization header
            var authorizationHeader = request.GetAuthorizationHeader();
            if (!authorizationHeader.Scheme.Equals(OAuthConstants.AuthScheme)) return false;

            return true;
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
                //using (var reader = new StreamReader(request.Body))
                //{
                //    try
                //    {
                //        ltiRequest.ImsxPoxEnvelope = await reader.ReadToEndAsync();
                //    }
                //    finally
                //    {
                //        if (request.Body.CanSeek)
                //        {
                //            request.Body.Position = 0;
                //        }
                //    }
                //}
                var authorizationHeader = request.GetAuthorizationHeader();
                ltiRequest.Parameters.Add(authorizationHeader.Parameters);
                return ltiRequest;
            }
            return ltiRequest;
        }

        /// <summary>
        /// Parse the <see cref="IOwinRequest"/> into an LTI launch-style request.
        /// </summary>
        /// <param name="request">The <see cref="IOwinRequest"/> to parse</param>
        /// <returns>The <see cref="LtiRequest"/></returns>
        public static async Task<LtiRequest> ParseRequestAsync(this IOwinRequest request)
        {
            var ltiRequest = new LtiRequest(null) {HttpMethod = request.Method, Url = request.Uri};

            var form = await request.ReadFormAsync();
            foreach (var pair in form)
            {
                ltiRequest.Parameters.Add(pair.Key, string.Join(",", pair.Value));
            }
            return ltiRequest;
        }

    }
}
