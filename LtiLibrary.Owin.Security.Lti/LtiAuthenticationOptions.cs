using LtiLibrary.Owin.Security.Lti.Provider;
using Microsoft.Owin.Security;

namespace LtiLibrary.Owin.Security.Lti
{
    /// <summary>
    /// Configuration options for <see cref="LtiAuthenticationMiddleware"/>
    /// </summary>
    public class LtiAuthenticationOptions : AuthenticationOptions
    {
        /// <summary>
        /// Initializes a new <see cref="LtiAuthenticationOptions"/>
        /// </summary>
        public LtiAuthenticationOptions() : base(Constants.DefaultAuthenticationType)
        {
            Description.Caption = AuthenticationType;
            AuthenticationMode = AuthenticationMode.Passive;
            ClaimType = Constants.DefaultClaimType;
        }

        /// <summary>
        /// Gets or sets the claim type to use for the <see cref="System.Security.Claims.Claim"/> that
        /// is used to record the LTI request.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ILtiAuthenticationProvider"/> used to handle authentication events.
        /// </summary>
        public ILtiAuthenticationProvider Provider { get; set; }

        /// <summary>
        /// Gets or sets the name of another authentication middleware which will be responsible for actually
        /// issuing a user <see cref="System.Security.Claims.ClaimsIdentity"/>.
        /// </summary>
        public string SignInAsAuthenticationType { get; set; }
   }
}
