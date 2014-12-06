using LtiLibrary.Owin.Security.Lti.Provider;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System.Threading.Tasks;

namespace LtiLibrary.Owin.Security.Lti
{
    /// <summary>
    /// A per-request authentication handler for the OpenIdConnectAuthenticationMiddleware.
    /// </summary>
    public class LtiAuthenticationHandler : AuthenticationHandler<LtiAuthenticationOptions>
    {
        /// <summary>
        /// Normally invoked to process incoming authentication messages in 3-legged authentication
        /// schemes. But LTI is a one-legged authentication scheme, so all authentication messages
        /// are in the original post. This method is not used.
        /// </summary>
        /// <returns>Always returns null.</returns>
        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            return Task.FromResult<AuthenticationTicket>(null);
        }

        /// <summary>
        /// Handles LTI authentication requests.
        /// </summary>
        /// <returns>True if the request was handled, false if the next middleware should be invoked.</returns>
        public override async Task<bool> InvokeAsync()
        {
            // This is always invoked on each request. If this is an LTI basic launch request, then
            // tell the middleware to handle a response challenge using LTI authentication.
            if (await Request.IsAuthenticatedWithLti())
            {
                var ltiRequest = await Request.ParseRequestAsync();

                // Let the application supply the OAuth secret
                var authenticateContext = new LtiAuthenticateContext(Context, ltiRequest);
                await Options.Provider.Authenticate(authenticateContext);
                if (string.IsNullOrEmpty(authenticateContext.Secret))
                {
                    return false;
                }

                // Check the signature
                var signature = ltiRequest.GenerateSignature(authenticateContext.Secret);
                if (!ltiRequest.Signature.Equals(signature))
                {
                    return false;
                }

                // Let the application sign in the application user
                var authenticatedContext = new LtiAuthenticatedContext(Context, Options, ltiRequest);
                await Options.Provider.Authenticated(authenticatedContext);

                // We're done here, return to the original request
                Response.Redirect(Request.Uri.ToString());
                return true;
            }

            // Let the rest of the pipeline run.
            return false;
        }
    }
}
