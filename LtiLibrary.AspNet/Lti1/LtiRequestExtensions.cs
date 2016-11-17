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
            return new LtiRequestViewModel
            {
                Action = ltiRequest.Url.ToString(),
                Fields = ltiRequest.Parameters
            };
        }
    }
}
