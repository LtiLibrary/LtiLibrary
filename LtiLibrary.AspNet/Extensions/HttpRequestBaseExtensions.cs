using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Lti1;
using LtiLibrary.Core.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace LtiLibrary.AspNet.Extensions
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
                LtiConstants.ResourceLinkIdParameter
            };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemLaunchParameters =
            {
                LtiConstants.AcceptMediaTypesParameter,
                LtiConstants.AcceptPresentationDocumentTargetsParameter,
                LtiConstants.ContentItemReturnUrlParameter,
                LtiConstants.LtiMessageTypeParameter,
                LtiConstants.LtiVersionParameter
            };

        // These LTI Content Item parameters are required
        private static readonly string[] RequiredContentItemResponseParameters =
            {
                LtiConstants.ContentItemPlacementParameter
            };

        public static string GenerateOAuthSignature(this HttpRequest request, string consumerSecret)
        {
            var url = new Uri(request.GetDisplayUrl());
            var parameters = new NameValueCollection();
            foreach (var formKey in request.Form.Keys)
            {
                parameters.Set(formKey, request.Form[formKey]);
            }
            return OAuthUtility.GenerateSignature(request.Method, url, parameters, consumerSecret);
        }

        public static LisContextType? GetLisContextType(this HttpRequest request, string key)
        {
            var stringValue = request.Form[key];
            LisContextType contextTypeEnum;
            return Enum.TryParse(stringValue, out contextTypeEnum)
                       ? contextTypeEnum
                       : default(LisContextType?);
        }

        public static DocumentTarget? GetPresentationTarget(this HttpRequest request, string key)
        {
            var stringValue = request.Form[key];
            DocumentTarget presentationTargetEnum;
            return Enum.TryParse(stringValue, out presentationTargetEnum)
                       ? presentationTargetEnum
                       : default(DocumentTarget?);
        }

        //private static string GetUnvalidatedString(this HttpRequest request, string key)
        //{
        //    return request.Unvalidated[key];
        //}

        /// <summary>
        /// Get a value indicating whether the current request is authenticated
        /// using LTI.
        /// </summary>
        public static bool IsAuthenticatedWithLti(this HttpRequest request)
        {
            var messageType = request.Form[LtiConstants.LtiMessageTypeParameter][0] ?? string.Empty;
            return request.Method.Equals("POST")
                   && (
                       messageType.Equals(LtiConstants.BasicLaunchLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       || messageType.Equals(LtiConstants.ContentItemSelectionRequestLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       || messageType.Equals(LtiConstants.ContentItemSelectionLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       );
        }

        /// <summary>
        /// Parse the HttpRequest and return a filled in LtiRequest.
        /// </summary>
        /// <param name="request">The HttpRequest to parse.</param>
        /// <returns>An LtiInboundRequest filled in with the OAuth and LTI parameters
        /// sent by the consumer.</returns>
        public static void CheckForRequiredLtiParameters(this HttpRequest request)
        {
            if (!request.IsAuthenticatedWithLti())
            {
                throw new LtiException("Invalid LTI request.");
            }

            // Make sure the request contains all the required parameters
            request.RequireAllOf(RequiredOauthParameters);
            switch (request.Form[LtiConstants.LtiMessageTypeParameter])
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
                case LtiConstants.ContentItemSelectionLtiMessageType:
                {
                    request.RequireAllOf(RequiredContentItemResponseParameters);
                    break;
                }
            }
        }

        public static void RequireAllOf(this HttpRequest request, IEnumerable<string> parameters)
        {
            var missing = parameters.Where(parameter => string.IsNullOrEmpty(request.Form[parameter])).ToList();

            if (missing.Count > 0)
            {
                throw new LtiException("Missing parameters: " + string.Join(", ", missing.ToArray()));
            }
        }
    }
}
