using System.Linq;
using LtiLibrary.Common;
using LtiLibrary.Lti1;
using LtiLibrary.OAuth;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;

namespace LtiLibrary.Extensions
{
    public static class HttpRequestBaseExtensions
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

        public static string GenerateOAuthSignature(this HttpRequestBase request, string consumerSecret)
        {
            return OAuthUtility.GenerateSignature(request.HttpMethod, request.Url, request.UnvalidatedParameters(), consumerSecret);
        }

        public static string GenerateOAuthSignatureBase(this HttpRequestBase request)
        {
            return OAuthUtility.GenerateSignatureBase(request.HttpMethod, request.Url, request.UnvalidatedParameters());
        }

        public static LisContextType? GetLisContextType(this HttpRequestBase request, string key)
        {
            var stringValue = request.GetUnvalidatedString(key);
            LisContextType contextTypeEnum;
            return Enum.TryParse(stringValue, out contextTypeEnum)
                       ? contextTypeEnum
                       : default(LisContextType?);
        }

        public static DocumentTarget? GetPresentationTarget(this HttpRequestBase request, string key)
        {
            var stringValue = request.GetUnvalidatedString(key);
            DocumentTarget presentationTargetEnum;
            return Enum.TryParse(stringValue, out presentationTargetEnum)
                       ? presentationTargetEnum
                       : default(DocumentTarget?);
        }

        private static string GetUnvalidatedString(this HttpRequestBase request, string key)
        {
            return request.Unvalidated[key];
        }

        /// <summary>
        /// Get a value indicating whether the current request is authenticated
        /// using LTI.
        /// </summary>
        public static bool IsAuthenticatedWithLti(this HttpRequestBase request)
        {
            var messageType = request[LtiConstants.LtiMessageTypeParameter] ?? string.Empty;
            return request.HttpMethod.Equals(WebRequestMethods.Http.Post)
                   && (
                       messageType.Equals(LtiConstants.BasicLaunchLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       || messageType.Equals(LtiConstants.ContentItemSelectionRequestLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       || messageType.Equals(LtiConstants.ContentItemSelectionResponseLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       );
        }

        public static NameValueCollection UnvalidatedParameters(this HttpRequestBase request)
        {
            var parameters = new NameValueCollection();
            parameters.Add(request.Unvalidated.QueryString);
            parameters.Add(request.Unvalidated.Form);
            return parameters;
        }

        /// <summary>
        /// Parse the HttpRequest and return a filled in LtiRequest.
        /// </summary>
        /// <param name="request">The HttpRequest to parse.</param>
        /// <returns>An LtiInboundRequest filled in with the OAuth and LTI parameters
        /// sent by the consumer.</returns>
        public static void CheckForRequiredLtiParameters(this HttpRequestBase request)
        {
            if (!request.IsAuthenticatedWithLti())
            {
                throw new LtiException("Invalid LTI request.");
            }

            // Make sure the request contains all the required parameters
            request.RequireAllOf(RequiredOauthParameters);
            switch (request[LtiConstants.LtiMessageTypeParameter])
            {
                case LtiConstants.BasicLaunchLtiMessageType:
                {
                    request.RequireAllOf(RequiredBasicLaunchParameters);
                    break;
                }
                case LtiConstants.ContentItemSelectionRequestLtiMessageType:
                {
                    request.RequireAllOf(RequiredContentItemLaunchParameters);
                    break;
                }
                case LtiConstants.ContentItemSelectionResponseLtiMessageType:
                {
                    request.RequireAllOf(RequiredContentItemResponseParameters);
                    break;
                }
            }
        }

        public static void RequireAllOf(this HttpRequestBase request, IEnumerable<string> parameters)
        {
            var missing = parameters.Where(parameter => string.IsNullOrEmpty(request[parameter])).ToList();

            if (missing.Count > 0)
            {
                throw new LtiException("Missing parameters: " + string.Join(", ", missing.ToArray()));
            }
        }
    }
}
