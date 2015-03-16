using System.Collections.Specialized;
using System.Linq;
using LtiLibrary.Core.Lti1;

namespace LtiLibrary.AspNet.Lti1
{
    public static class LtiRequestExtensions
    {
        /// <summary>
        /// Return an LtiRequestViewModel suitable for creating an MVC view or AspNet page
        /// that auto-submits an LTI request.
        /// </summary>
        /// <param name="ltiRequest">The LtiRequest to submit.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to sign the request.</param>
        /// <returns></returns>
        public static LtiRequestViewModel GetViewModel(this LtiRequest ltiRequest, string consumerSecret)
        {
            // Create a copy of the parameters (getters should not change the object and this
            // getter changes the parameters to eliminate empty/null values and make custom
            // parameter substitutions)
            var parameters = new NameValueCollection(ltiRequest.Parameters);

            // Remove empty/null parameters
            foreach (var key in parameters.AllKeys.Where(key => string.IsNullOrWhiteSpace(parameters[key])))
            {
                parameters.Remove(key);
            }

            // Perform the custom variable substitutions
            ltiRequest.SubstituteCustomVariables(parameters);

            // Calculate the signature based on the substituted values
            var signature = ltiRequest.GenerateSignature(parameters, consumerSecret);

            return new LtiRequestViewModel
            {
                Action = ltiRequest.Url.ToString(),
                Fields = parameters,
                Signature = signature
            };
        }
    }
}
