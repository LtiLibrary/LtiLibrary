using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.Models;
using OAuth.Net.Common;
using OAuth.Net.Components;

namespace LtiLibrary.Provider
{
    public class LtiInboundRequestHandler : IDisposable
    {
        // The "Learn to LTI" app on http://learn-lti.herokuapp.com/ sends "LTI-1.0" instead
        // of the normal "LTI-1p0"
        private static readonly string LtiVersion = LtiConstants.LtiVersion + "|LTI-1.0";
        
        private readonly LtiLibraryContext _db = new LtiLibraryContext();

        // This OAuth parameters are required
        private readonly string[] _requiredOauthParameters =
            {
                Constants.ConsumerKeyParameter,
                Constants.NonceParameter,
                Constants.SignatureParameter,
                Constants.SignatureMethodParameter,
                Constants.TimestampParameter,
                Constants.VersionParameter
            };

        // These LTI parameters are required
        private readonly string[] _requiredLtiParameters =
            {
                LtiConstants.LtiMessageTypeParameter,
                LtiConstants.LtiVersionParameter,
                LtiConstants.ResourceLinkIdParameter,
                LtiConstants.UserIdParameter
            };

        /// <summary>
        /// Get a value indicating whether the current user is authenticated
        /// by an LTI Tool Consumer.
        /// </summary>
        public static bool IsAuthenticatedWithLti(HttpRequest request)
        {
            return request.HttpMethod.Equals(WebRequestMethods.Http.Post)
                && (request[LtiConstants.LtiMessageTypeParameter] ?? string.Empty)
                .Equals(LtiConstants.LtiMessageType, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parse the HttpRequest and return a filled in LtiInboundRequest.
        /// </summary>
        /// <param name="request">The HttpRequest to parse.</param>
        /// <returns>An LtiInboundRequest filled in with the OAuth and LTI parameters
        /// sent by the consumer.</returns>
        public virtual LtiInboundRequest Parse(HttpRequest request)
        {
            if (!IsAuthenticatedWithLti(request))
            {
                throw new LtiException("Invalid LTI request.");
            }

            // Make sure the minimum required fields are present. This will throw
            // OAuthRequestException if any required OAuth parameters are missing and
            // LtiException if any required LTI parameters are missing.
            var parameters = OAuthParameters.Parse(request);
            parameters.RequireAllOf(_requiredOauthParameters);
            parameters.RequireAllAdditionalOf(_requiredLtiParameters);

            if (!Regex.IsMatch(parameters.AdditionalParameters[LtiConstants.LtiVersionParameter], LtiVersion))
            {
                throw new LtiException("Missing or invalid " + LtiConstants.LtiVersionParameter);
            }

            Int64 oauthTimestamp;
            if (!Int64.TryParse(parameters.Timestamp, out oauthTimestamp))
            {
                throw new LtiException("Invalid " + Constants.TimestampParameter + " format");
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timeout = TimeSpan.FromMinutes(5);

            var oauthTimestampAbsolute = epoch.AddSeconds(oauthTimestamp);
            if (DateTime.UtcNow - oauthTimestampAbsolute > timeout)
            {
                throw new LtiException("Expired " + Constants.TimestampParameter);
            }

            var ltiRequest = _db.LtiInboundRequests.SingleOrDefault(r => r.Nonce == parameters.Nonce);
            if (ltiRequest != null)
            {
                throw new LtiException("Nonce already used");
            }

            var oldestTimestamp = Convert.ToInt64(((DateTime.UtcNow - timeout) - epoch).TotalSeconds);
            foreach (var oldRequest in _db.LtiInboundRequests.Where(n => n.Timestamp < oldestTimestamp).ToList())
            {
                _db.LtiInboundRequests.Remove(oldRequest);
            }

            ltiRequest = new LtiInboundRequest();
            ltiRequest.Nonce = parameters.Nonce;
            ltiRequest.Timestamp = oauthTimestamp;

            // Find the consumer (throws an LtiException if consumer not found)
            ltiRequest.ConsumerId = GetConsumerId(parameters, request.Url);

            // Fill in the LTI parameters
            ltiRequest.ContextId = parameters.GetString(LtiConstants.ContextIdParameter);
            ltiRequest.ContextLabel = parameters.GetString(LtiConstants.ContextLabelParameter);
            ltiRequest.ContextTitle = parameters.GetString(LtiConstants.ContextTitleParameter);
            ltiRequest.ContextType = parameters.GetLisContextType(LtiConstants.ContextTypeParameter);
            ltiRequest.LaunchPresentationCssUrl = parameters.GetString(LtiConstants.LaunchPresentationCssUrlParameter);
            ltiRequest.LaunchPresentationDocumentTarget = parameters.GetPresentationTarget(LtiConstants.LaunchPresentationDocumentTargetParameter);
            ltiRequest.LaunchPresentationHeight = parameters.GetInt(LtiConstants.LaunchPresentationHeightParameter);
            ltiRequest.LaunchPresentationLocale = parameters.GetString(LtiConstants.LaunchPresentationLocaleParameter);
            ltiRequest.LaunchPresentationReturnUrl = parameters.GetString(LtiConstants.LaunchPresentationReturnUrlParameter);
            ltiRequest.LaunchPresentationWidth = parameters.GetInt(LtiConstants.LaunchPresentationWidthParameter);
            ltiRequest.LisCourseOfferingSourcedId = parameters.GetString(LtiConstants.LisCourseOfferingSourcedId);
            ltiRequest.LisCourseSectionSourcedId = parameters.GetString(LtiConstants.LisCourseSectionSourceId);
            ltiRequest.LisOutcomeServiceUrl = parameters.GetString(LtiConstants.LisOutcomeServiceUrlParameter);
            ltiRequest.LisPersonEmailPrimary = parameters.GetString(LtiConstants.LisPersonContactEmailPrimaryParameter);
            ltiRequest.LisPersonNameFamily = parameters.GetString(LtiConstants.LisPersonNameFamilyParameter);
            ltiRequest.LisPersonNameFull = parameters.GetString(LtiConstants.LisPersonNameFullParameter);
            ltiRequest.LisPersonNameGiven = parameters.GetString(LtiConstants.LisPersonNameGivenParameter);
            ltiRequest.LisPersonSourcedId = parameters.GetString(LtiConstants.LisPersonSourcedId);
            ltiRequest.LisResultSourcedId = parameters.GetString(LtiConstants.LisResultSourcedIdParameter);
            ltiRequest.LtiMessageType = parameters.GetString(LtiConstants.LtiMessageTypeParameter);
            ltiRequest.LtiVersion = parameters.GetString(LtiConstants.LtiVersionParameter);
            ltiRequest.ResourceLinkDescription = parameters.GetString(LtiConstants.ResourceLinkDescriptionParameter);
            ltiRequest.ResourceLinkId = parameters.GetString(LtiConstants.ResourceLinkIdParameter);
            ltiRequest.ResourceLinkTitle = parameters.GetString(LtiConstants.ResourceLinkTitleParameter);
            ltiRequest.RoleScopeMentor = parameters.GetString(LtiConstants.RoleScopeMentorParameter);
            ltiRequest.RolesAsString = parameters.GetString(LtiConstants.RolesParameter);
            ltiRequest.ToolConsumerInfoProductFamilyCode = parameters.GetString(LtiConstants.ToolConsumerInfoProductFamilyCodeParameter);
            ltiRequest.ToolConsumerInfoVersion = parameters.GetString(LtiConstants.ToolConsumerInfoVersionParameter);
            ltiRequest.ToolConsumerInstanceContactEmail = parameters.GetString(LtiConstants.ToolConsumerInstanceContactEmailParameter);
            ltiRequest.ToolConsumerInstanceDescription = parameters.GetString(LtiConstants.ToolConsumerInstanceDescriptionParameter);
            ltiRequest.ToolConsumerInstanceGuid = parameters.GetString(LtiConstants.ToolConsumerInstanceGuidParameter);
            ltiRequest.ToolConsumerInstanceName = parameters.GetString(LtiConstants.ToolConsumerInstanceNameParameter);
            ltiRequest.ToolConsumerInstanceUrl = parameters.GetString(LtiConstants.ToolConsumerInstanceUrlParameter);
            ltiRequest.Url = request.Url.ToString();
            ltiRequest.UserId = parameters.GetString(LtiConstants.UserIdParameter);
            ltiRequest.UserImage = parameters.GetString(LtiConstants.UserImageParameter);

            // Fill in the custom parameters and extensions
            foreach (var name in parameters.AdditionalParameters.AllKeys)
            {
                if (name.StartsWith("custom_") || name.StartsWith("ext_"))
                {
                    var value = parameters.AdditionalParameters[name];
                    ltiRequest.CustomParameters.Add(name, value);
                }
            }

            // Save the request for the provider
            _db.LtiInboundRequests.Add(ltiRequest);
            _db.SaveChanges();

            // Outcomes can live a long time to give the teacher enough
            // time to grade the assignment. So they are stored in a separate table.
            var lisOutcomeServiceUrl = ltiRequest.LisOutcomeServiceUrl;
            var lisResultSourcedid = ltiRequest.LisResultSourcedId;
            if (!string.IsNullOrWhiteSpace(lisOutcomeServiceUrl) && !string.IsNullOrWhiteSpace(lisResultSourcedid))
            {
                var outcome = _db.Outcomes.SingleOrDefault(o =>
                    o.ConsumerId == ltiRequest.ConsumerId
                    && o.LisResultSourcedId == lisResultSourcedid);

                if (outcome == null)
                {
                    outcome = new Outcome
                    {
                        ConsumerId = ltiRequest.ConsumerId,
                        LisResultSourcedId = lisResultSourcedid
                    };
                    _db.Outcomes.Add(outcome);
                    _db.SaveChanges(); // Assign OutcomeId;
                }
                outcome.ContextTitle = ltiRequest.ContextTitle;
                outcome.ServiceUrl = lisOutcomeServiceUrl;
                ltiRequest.OutcomeId = outcome.OutcomeId;
                _db.SaveChanges();
            }

            return ltiRequest;
        }

        private int GetConsumerId(OAuthParameters parameters, Uri requestUrl)
        {
            var consumer = _db.Consumers.SingleOrDefault(c => c.Key == parameters.ConsumerKey);
            if (consumer == null)
            {
                throw new LtiException("Invalid " + Constants.ConsumerKeyParameter);
            }

            var signatureBase = SignatureBase.Create(WebRequestMethods.Http.Post, requestUrl, parameters);
            var signatureProvider = new HmacSha1SigningProvider();
            var signature = signatureProvider.ComputeSignature(signatureBase, consumer.Secret, string.Empty);

            var oauthSignature = parameters.Signature;
            if (!oauthSignature.Equals(signature))
            {
                throw new LtiException("Invalid " + Constants.SignatureParameter,
                    new LtiException("Signature base string: " + signatureBase));
            }

            return consumer.ConsumerId;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}