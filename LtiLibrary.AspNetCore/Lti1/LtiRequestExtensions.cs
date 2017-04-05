using LtiLibrary.NetCore.Lti1;

namespace LtiLibrary.AspNetCore.Lti1
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
            // Calculate the signature based on the substituted values
            var signature = ltiRequest.SubstituteCustomVariablesAndGenerateSignature(consumerSecret);

            return new LtiRequestViewModel
            {
                Action = ltiRequest.Url.ToString(),
                Fields = ltiRequest.Parameters,
                Signature = signature
            };
        }
    }
}
