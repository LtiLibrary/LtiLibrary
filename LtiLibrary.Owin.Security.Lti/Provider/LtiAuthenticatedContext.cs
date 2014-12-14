using LtiLibrary.Core.Lti1;
using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;
using System;

namespace LtiLibrary.Owin.Security.Lti.Provider
{
    public class LtiAuthenticatedContext : BaseContext<LtiAuthenticationOptions>
    {
        public LtiAuthenticatedContext(IOwinContext context, LtiAuthenticationOptions options, ILtiRequest ltiRequest) : base(context, options)
        {
            if (ltiRequest == null)
            {
                throw new ArgumentNullException("ltiRequest");
            }
            LtiRequest = ltiRequest;
        }

        /// <summary>
        /// Get the LTI request
        /// </summary>
        public ILtiRequest LtiRequest { get; private set; }

        /// <summary>
        /// Get or set a parameter telling the LtiAuthenticationHandler to redirect the request. Set Redirect
        /// to True to redirect the request to the original URL. This is useful if a new identity has been
        /// signed in that uses cookies. Default is False.
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}
