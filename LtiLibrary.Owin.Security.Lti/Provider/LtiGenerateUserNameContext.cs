using System;
using LtiLibrary.Lti1;
using Microsoft.Owin;
using Microsoft.Owin.Security.Provider;

namespace LtiLibrary.Owin.Security.Lti.Provider
{
    public class LtiGenerateUserNameContext : BaseContext
    {
        public LtiGenerateUserNameContext(IOwinContext context, ILtiRequest ltiRequest) : base(context)
        {
            if (ltiRequest == null)
            {
                throw new ArgumentNullException("ltiRequest");
            }
            LtiRequest = ltiRequest;
        }

        public ILtiRequest LtiRequest { get; private set; }
        public string UserName { get; set; }
    }
}
