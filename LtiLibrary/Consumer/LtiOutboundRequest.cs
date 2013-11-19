using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LtiLibrary.Common;
using LtiLibrary.Extensions;
using OAuth.Net.Common;
using OAuth.Net.Components;

namespace LtiLibrary.Consumer
{
    public class LtiOutboundRequest : LtiRequest
    {
        /// <summary>
        /// OAuth consumer key
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// OAuth consumer secret
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// Add the custom parameters from a string.
        /// </summary>
        /// <param name="parameters"></param>
        public void AddCustomParameters(string parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                var customParams = parameters.Split(new[] { ",", "\r\n", "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
                foreach (var customParam in customParams)
                {
                    var namevalue = customParam.Split(new[] { "=" },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (namevalue.Length == 2)
                    {
                        CustomParameters.Add(namevalue[0], namevalue[1]);
                    }
                }
            }
        }

        /// <summary>
        /// Add optional parameters that are specific to an LTI 1.1 request.
        /// </summary>
        /// <param name="parameters">The partially filled OAuthParameters object
        /// that is being used to collect the data.</param>
        private void AddCustomParametersToRequest(OAuthParameters parameters)
        {
            // LTI 1.1 supports custom parameter substitution
            if (CustomParameters.Count == 0) return;

            foreach (var key in CustomParameters.AllKeys)
            {
                // Per the LTI 1.x specs, custom parameter
                // names must be lowercase letters or numbers. Any other
                // character is replaced with an underscore.
                var value = SubstituteCustomValue(CustomParameters[key]);
                var name = Regex.Replace(key.ToLower(), "[^0-9a-zA-Z]", "_");
                if (!name.StartsWith("custom_")) name = string.Concat("custom_", name);
                parameters.AdditionalParameters.Add(name, value);
            }
        }

        /// <summary>
        /// Add the optional parameters for an LTI 1.x request. Although these are
        /// optional according to the specification, they are required to pass the
        /// certification tests.
        /// </summary>
        /// <param name="parameters">The partially filled OAuthParameters object
        /// that is being used to collect the launch data.</param>
        private void AddOptionalParametersToRequest(OAuthParameters parameters)
        {
            // Context: These next parameters further identify where the request coming from.
            // "Context" can be thought of as the course or class. If the current user is
            // enrolled in the course, then the context type is CourseSection, otherwise
            // the type is CourseTemplate.
            parameters.AddParameter(LtiConstants.ContextIdParameter, ContextId);
            parameters.AddParameter(LtiConstants.ContextLabelParameter, ContextLabel);
            parameters.AddParameter(LtiConstants.ContextTitleParameter, ContextTitle);
            parameters.AddParameter(LtiConstants.ContextTypeParameter, ContextType);
            parameters.AddParameter(LtiConstants.LisCourseOfferingSourcedId, LisCourseOfferingSourcedId);
            parameters.AddParameter(LtiConstants.LisCourseSectionSourceId, LisCourseSectionSourcedId);

            // Resource: These parameters identify the resource. In K-12, a resource is
            // equivalent to assignment and the resource_link_id must be unique to each
            // context_id (remember that context is equivalent to course or class).
            // Note that the title is recommend, but not required.
            parameters.AddHtmlParameter(LtiConstants.ResourceLinkDescriptionParameter, ResourceLinkDescription);
            parameters.AddParameter(LtiConstants.ResourceLinkIdParameter, ResourceLinkId);
            parameters.AddParameter(LtiConstants.ResourceLinkTitleParameter, ResourceLinkTitle);

            // Tool Consumer: These identify this consumer to the provider. In K-12, tools
            // such as LMS and Portal systems are typically  purchased by the district and
            // shared by multiple schools in the district. My advice is to use the district
            // identity of the tool here (e.g. "Hillsboro School District LMS"). These
            // parameters are recommended.
            parameters.AddParameter(LtiConstants.ToolConsumerInfoProductFamilyCodeParameter, ToolConsumerInfoProductFamilyCode);
            parameters.AddParameter(LtiConstants.ToolConsumerInfoVersionParameter, ToolConsumerInfoVersion);
            parameters.AddParameter(LtiConstants.ToolConsumerInstanceContactEmailParameter, ToolConsumerInstanceContactEmail);
            parameters.AddParameter(LtiConstants.ToolConsumerInstanceDescriptionParameter, ToolConsumerInstanceDescription);
            parameters.AddParameter(LtiConstants.ToolConsumerInstanceGuidParameter, ToolConsumerInstanceGuid);
            parameters.AddParameter(LtiConstants.ToolConsumerInstanceNameParameter, ToolConsumerInstanceName);
            parameters.AddParameter(LtiConstants.ToolConsumerInstanceUrlParameter, ToolConsumerInstanceUrl);

            // User: These parameters identify the user and their roles within the
            // context. These parameters are recommended.
            parameters.AddParameter(LtiConstants.LisPersonContactEmailPrimaryParameter, LisPersonEmailPrimary);
            parameters.AddParameter(LtiConstants.LisPersonNameFamilyParameter, LisPersonNameFamily);
            parameters.AddParameter(LtiConstants.LisPersonNameFullParameter, LisPersonNameFull);
            parameters.AddParameter(LtiConstants.LisPersonNameGivenParameter, LisPersonNameGiven);
            parameters.AddParameter(LtiConstants.LisPersonSourcedId, LisPersonSourcedId);
            parameters.AddParameter(LtiConstants.RolesParameter, Roles);
            parameters.AddParameter(LtiConstants.RoleScopeMentorParameter, RoleScopeMentor);
            parameters.AddParameter(LtiConstants.UserIdParameter, UserId);
            parameters.AddParameter(LtiConstants.UserImageParameter, UserImage);

            // Presentation: You can use launch_presentation_locale to send the preferred presentation
            // langauge, symbols, etc. I am sending the current UI culture (e.g. en-US).
            // This parameter is recommended.
            parameters.AddParameter(LtiConstants.LaunchPresentationLocaleParameter, LaunchPresentationLocale);
            parameters.AddParameter(LtiConstants.LaunchPresentationCssUrlParameter, LaunchPresentationCssUrl);
            parameters.AddParameter(LtiConstants.LaunchPresentationDocumentTargetParameter, LaunchPresentationDocumentTarget);
            parameters.AddParameter(LtiConstants.LaunchPresentationHeightParameter, LaunchPresentationHeight);
            parameters.AddParameter(LtiConstants.LaunchPresentationReturnUrlParameter, LaunchPresentationReturnUrl);
            parameters.AddParameter(LtiConstants.LaunchPresentationWidthParameter, LaunchPresentationWidth);

            // Basic Outcomes Service: These parameters tell the provider where to
            // send outcomes (if any) for this assignment. Only sent if the current
            // user is enrolled in the course.
            parameters.AddParameter(LtiConstants.LisOutcomeServiceUrlParameter, LisOutcomeServiceUrl);
            parameters.AddParameter(LtiConstants.LisResultSourcedIdParameter, LisResultSourcedId);
        }

        /// <summary>
        /// Calculate the data for a basic LTI 1.x request.
        /// </summary>
        /// <returns>An OAuthParameters object which includes the required paremters
        /// for an LTI 1.x request.</returns>
        /// <remarks>The description will be converted to plain text.</remarks>
        private void AddRequiredParametersToRequest(OAuthParameters parameters)
        {
            const string oauthCallback = "about:blank";
            const string oauthSignatureMethod = "HMAC-SHA1";
            const string oauthVersion = Constants.Version1_0;

            // First I calculate some values that I will need to sign the request
            // with OAuth.Net.
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            var nonce = Guid.NewGuid().ToString("N");

            parameters.Callback = oauthCallback;
            parameters.ConsumerKey = ConsumerKey;
            parameters.Nonce = nonce;
            parameters.SignatureMethod = oauthSignatureMethod;
            parameters.Timestamp = timestamp;
            parameters.Version = oauthVersion;

            // LTI Header: These identify the request as being an LTI request
            parameters.AdditionalParameters.Add(LtiConstants.LtiMessageTypeParameter, LtiMessageType);
            parameters.AdditionalParameters.Add(LtiConstants.LtiVersionParameter, LtiVersion);
        }

        public void AddRoles(IList<LtiRoles> roles)
        {
            foreach (var role in roles)
            {
                Roles.Add(role);
            }
        }

        public LtiOutboundRequestViewModel GetLtiRequestModel()
        {
            var parameters = new OAuthParameters();

            // Add required OAuth and LTI parameters
            AddRequiredParametersToRequest(parameters);

            // Add all the optional parameters
            AddOptionalParametersToRequest(parameters);

            // Add the custom parameters
            AddCustomParametersToRequest(parameters);

            // The LTI spec says to include the querystring parameters
            // when calculating the signature base string
            var uri = new Uri(Url);
            var querystring = HttpUtility.ParseQueryString(uri.Query);
            parameters.AdditionalParameters.Add(querystring);

            // Calculate the OAuth signature and send the data over to the view 
            // for rendering in the client browser. See Views/Assignment/Launch
            var signatureBase = SignatureBase.Create("POST", uri, parameters);

            // Now remove the querystring parameters so they are not sent twice
            // (once in the action URL and once in the form data)
            foreach (var name in querystring.AllKeys)
                parameters.AdditionalParameters.Remove(name);

            // Compute the signature and log info for debugging
            var signatureProvider = new HmacSha1SigningProvider();
            var signature = signatureProvider.ComputeSignature(signatureBase, ConsumerSecret, string.Empty);

            // Finally fill the LtiRequest
            return new LtiOutboundRequestViewModel
            {
                Action = uri.ToString(),
                Fields = HttpUtility.ParseQueryString(parameters.ToQueryStringFormat(), Encoding.UTF8),
                Signature = signature,
                Title = ResourceLinkTitle
            };
        }

        /// <summary>
        /// Substitute known custom value tokens. Per the LTI 1.1 spec, unknown
        /// tokens are ignored.
        /// </summary>
        /// <param name="value">Custom value to scan.</param>
        /// <returns>Custom value with the known tokens replaced by their
        /// current value.</returns>
        private string SubstituteCustomValue(string value)
        {
            // LTI User variables
            value = Regex.Replace(value, "\\$User.id", UserId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.username", UserName ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$User.org", UserOrg ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Person variables
            value = Regex.Replace(value, "\\$Person.address.country", LisPersonAddressCountry ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.locality", LisPersonAddressLocality ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.postcode", LisPersonAddressPostCode ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.statepr", LisPersonAddressStatePr ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street1", LisPersonAddressStreet1 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street2", LisPersonAddressStreet2 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street3", LisPersonAddressStreet3 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.street4", LisPersonAddressStreet4 ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.address.timezone", LisPersonAddressTimezone ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.email.personal", LisPersonEmailPersonal ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.email.primary", LisPersonEmailPrimary ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.full", LisPersonNameFull ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.family", LisPersonNameFamily ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.given", LisPersonNameGiven ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.middle", LisPersonNameMiddle ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.prefix", LisPersonNamePrefix ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.name.suffix", LisPersonNameSuffix ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.home", LisPersonPhoneHome ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.mobile", LisPersonPhoneMobile ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.primary", LisPersonPhonePrimary ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.phone.work", LisPersonPhoneWork ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.sms", LisPersonSms ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.sourcedId", LisPersonSourcedId ?? string.Empty, RegexOptions.IgnoreCase);
            value = Regex.Replace(value, "\\$Person.webaddress", LisPersonWebAddress ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Course Offering variables
            value = Regex.Replace(value, "\\$CourseOffering.sourcedId", LisCourseOfferingSourcedId ?? string.Empty, RegexOptions.IgnoreCase);

            // LIS Course Section variables
            value = Regex.Replace(value, "\\$CourseSection.sourcedId", LisCourseSectionSourcedId ?? string.Empty, RegexOptions.IgnoreCase);

            // Tool Consumer Profile
            value = Regex.Replace(value, "\\$ToolConsumerProfile.url", ToolConsumerProfileUrl ?? string.Empty, RegexOptions.IgnoreCase);

            return value;
        }
    }
}
