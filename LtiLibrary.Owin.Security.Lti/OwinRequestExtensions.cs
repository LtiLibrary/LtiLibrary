using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LtiLibrary.Common;
using LtiLibrary.Lti1;
using LtiLibrary.OAuth;
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
        /// Check if the <see cref="IOwinRequest"/> is an LTI request.
        /// </summary>
        public static async Task<bool> IsAuthenticatedWithLti(this IOwinRequest request)
        {
            if (!request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.OrdinalIgnoreCase)) return false;

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
        /// Parse the <see cref="IOwinRequest"/> into an LTI request.
        /// </summary>
        /// <param name="request">The <see cref="IOwinRequest"/> to parse</param>
        /// <returns>The <see cref="LtiRequest"/></returns>
        public static async Task<LtiRequest> ParseRequestAsync(this IOwinRequest request)
        {
            var ltiRequest = new LtiRequest(null);
            ltiRequest.HttpMethod = request.Method;
            ltiRequest.Url = request.Uri;

            var form = await request.ReadFormAsync();
            foreach (var pair in form)
            {
                ltiRequest.Parameters.Add(pair.Key, string.Join(",", pair.Value));
            }
            return ltiRequest;
        }

    }
}
