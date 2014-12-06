using LtiLibrary.Lti1;
using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;
using System;

namespace LtiLibrary.Owin.Security.Lti.Provider
{
    public class LtiAuthenticateContext : BaseContext
    {
        public LtiAuthenticateContext(IOwinContext context, ILtiRequest ltiRequest) : base(context)
        {
            if (ltiRequest == null)
            {
                throw new ArgumentNullException("ltiRequest");
            }
            LtiRequest = ltiRequest;
        }

        public ILtiRequest LtiRequest { get; private set; }
        public string Secret { get; set; }
    }
}
