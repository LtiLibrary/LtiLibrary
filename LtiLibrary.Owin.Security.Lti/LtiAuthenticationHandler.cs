using LtiLibrary.Owin.Security.Lti.Provider;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LtiLibrary.Owin.Security.Lti
{
    /// <summary>
    /// A per-request authentication handler for the OpenIdConnectAuthenticationMiddleware.
    /// </summary>
    public class LtiAuthenticationHandler : AuthenticationHandler<LtiAuthenticationOptions>
    {
        /// <summary>
        /// Redirect to an application URL that explains how to login.
        /// </summary>
        /// <returns>A task</returns>
        /// <remarks>
        /// This method is invoked if the user has manually asked to login with the LTI middleware.
        /// In an OAuth 2.0-style application, this method would redirect to the remote login
        /// interface to retrieve a token. But in LTI, the only way into this application is by
        /// logging into the Tool Consumer and then launching this application.
        /// </remarks>
        protected override Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return base.ApplyResponseChallengeAsync();
            }

            var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
            if (challenge == null)
            {
                return base.ApplyResponseChallengeAsync();
            }

            if (Options.ChallengeResultUrl.HasValue)
            {
                Response.Redirect(Options.ChallengeResultUrl.Value);
            }

            return base.ApplyResponseChallengeAsync();
        }

        /// <summary>
        /// Normally invoked to process incoming authentication messages in 3-legged authentication
        /// schemes. But LTI is a one-legged authentication scheme, so all authentication messages
        /// are in the original post. This method is only used to supply an <see cref="AuthenticationTicket"/>
        /// for the System.Web.Http.ApiController OWIN pipeline.
        /// </summary>
        /// <returns>Always returns null.</returns>
        protected async override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            if (await Request.IsAuthenticatedLtiRequestAsync())
            {
                var identity = new ClaimsIdentity(Options.AuthenticationType);
                return new AuthenticationTicket(identity, new AuthenticationProperties());
            }

            return null;
        }

        /// <summary>
        /// Handles LTI authentication requests.
        /// </summary>
        /// <returns>True if the request was handled, false if the next middleware in the pipeline
        /// should be invoked.</returns>
        public override async Task<bool> InvokeAsync()
        {
            if (await Request.IsAuthenticatedLtiRequestAsync())
            {
                var ltiRequest = await Request.ParseLtiRequestAsync();

                // Let the application validate the parameters
                var authenticateContext = new LtiAuthenticateContext(Context, ltiRequest);
                await Options.Provider.Authenticate(authenticateContext);

                // Let the application sign in the application user if required
                var authenticatedContext = new LtiAuthenticatedContext(Context, Options, ltiRequest);
                await Options.Provider.Authenticated(authenticatedContext);
                if (!string.IsNullOrEmpty(authenticatedContext.RedirectUrl))
                {
                    // Stop processing the current request and redirect to the new URL
                    Response.Redirect(authenticatedContext.RedirectUrl);
                    return true;
                }
            }

            // Continue processing the request
            return false;
        }
    }
}
