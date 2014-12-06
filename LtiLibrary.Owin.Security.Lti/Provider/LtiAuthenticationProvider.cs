using System;
using System.Threading.Tasks;

namespace LtiLibrary.Owin.Security.Lti.Provider
{
    public class LtiAuthenticationProvider : ILtiAuthenticationProvider
    {
        public LtiAuthenticationProvider()
        {
            OnAuthenticate = context => Task.FromResult<object>(null);
            OnAuthenticated = context => Task.FromResult<object>(null);
            OnGenerateUserName = context => Task.FromResult<object>(null);
        }

        public virtual Task Authenticate(LtiAuthenticateContext context)
        {
            return OnAuthenticate.Invoke(context);
        }

        public virtual Task Authenticated(LtiAuthenticatedContext context)
        {
            return OnAuthenticated.Invoke(context);
        }

        public virtual Task GenerateUserName(LtiGenerateUserNameContext context)
        {
            return OnGenerateUserName.Invoke(context);
        }

        public Func<LtiAuthenticateContext, Task> OnAuthenticate { get; set; }
        public Func<LtiAuthenticatedContext, Task> OnAuthenticated { get; set; }
        public Func<LtiGenerateUserNameContext, Task> OnGenerateUserName { get; set; }
    }
}
