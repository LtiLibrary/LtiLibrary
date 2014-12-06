using LtiLibrary.Owin.Security.Lti.Provider;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;

namespace LtiLibrary.Owin.Security.Lti
{
    /// <summary>
    /// OWIN middleware for authenticating users using LTI
    /// </summary>
    public class LtiAuthenticationMiddleware : AuthenticationMiddleware<LtiAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a <see cref="LtiAuthenticationMiddleware"/>
        /// </summary>
        /// <param name="next">The next middleware in the OWIN pipeline to invoke</param>
        /// <param name="app">The OWIN application</param>
        /// <param name="options">Configuration options for the middleware</param>
        public LtiAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, LtiAuthenticationOptions options)
            : base(next, options)
        {
            if (Options.Provider == null)
            {
                Options.Provider = new LtiAuthenticationProvider();
            }
            if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
            {
                options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
            }
        }

        /// <summary>
        /// Provides the <see cref="AuthenticationHandler"/> object for processing authentication-related requests.
        /// </summary>
        /// <returns>An <see cref="AuthenticationHandler"/> configured with the <see cref="LtiAuthenticationOptions"/> supplied to the constructor.</returns>
        protected override AuthenticationHandler<LtiAuthenticationOptions> CreateHandler()
        {
            return new LtiAuthenticationHandler();
        }
    }
}
