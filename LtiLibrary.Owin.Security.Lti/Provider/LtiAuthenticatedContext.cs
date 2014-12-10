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

        public ILtiRequest LtiRequest { get; set; }
    }
}
