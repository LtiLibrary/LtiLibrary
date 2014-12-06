using LtiLibrary.Common;
using LtiLibrary.OAuth;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Provider.Owin
{
    public static class LtiAuthenticationExtensions
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

        public static IAppBuilder UseLtiAuthentication(this IAppBuilder app)
        {
            app.Use<LtiAuthenticationMiddleware>(app, new LtiAuthenticationOptions());
            return app;
        }

        /// <summary>
        /// Get a value indicating whether the current request is authenticated
        /// using LTI.
        /// </summary>
        public static bool IsAuthenticatedWithLti(this IFormCollection form)
        {
            string messageType = form[LtiConstants.LtiMessageTypeParameter] ?? string.Empty;
            return messageType.Equals(LtiConstants.BasicLaunchLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       || messageType.Equals(LtiConstants.ContentItemSelectionRequestLtiMessageType, StringComparison.OrdinalIgnoreCase)
                       || messageType.Equals(LtiConstants.ContentItemSelectionResponseLtiMessageType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parse the HttpRequest and return a filled in LtiRequest.
        /// </summary>
        /// <param name="request">The HttpRequest to parse.</param>
        /// <returns>An LtiInboundRequest filled in with the OAuth and LTI parameters
        /// sent by the consumer.</returns>
        public static void CheckForRequiredLtiParameters(this IFormCollection request)
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

        public static string GenerateOAuthSignature(this IOwinRequest request, IFormCollection form, string consumerSecret)
        {
            return OAuthUtility.GenerateSignature(request.Method, request.Uri, form.AsNameValueCollection(), consumerSecret);
        }

        public static string GenerateOAuthSignatureBase(this IOwinRequest request, IFormCollection form)
        {
            return OAuthUtility.GenerateSignatureBase(request.Method, request.Uri, form.AsNameValueCollection());
        }

        public static NameValueCollection AsNameValueCollection(this IFormCollection form)
        {
            var collection = new NameValueCollection();
            foreach (var pair in form)
            {
                foreach (var value in pair.Value)
                {
                    collection.Add(pair.Key, value);
                }
            }
            return collection;
        }

        public static void RequireAllOf(this IFormCollection form, IEnumerable<string> parameters)
        {
            var missing = parameters.Where(parameter => string.IsNullOrEmpty(form[parameter])).ToList();

            if (missing.Count > 0)
            {
                throw new LtiException("Missing parameters: " + string.Join(", ", missing.ToArray()));
            }
        }

    }
}